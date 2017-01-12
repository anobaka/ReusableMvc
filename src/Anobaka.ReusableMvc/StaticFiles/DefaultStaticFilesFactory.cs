using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Anobaka.Multipipeline;
using Anobaka.Multipipeline.DecisionTree;
using Anobaka.ReusableMvc.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Anobaka.ReusableMvc.StaticFiles
{
	public class DefaultStaticFilesFactory : IStaticFilesFactory<DefaultStaticFiles>
	{
		private readonly IOptions<ReusableMvcOptions> _options;
		private readonly IFileProvider _fileProvider;
		private readonly PipelineDecisionTree<ReusableMvcOptions> _pipelineDecisionTree;
		private readonly string _minifySuffix;

		private readonly
			ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, DefaultStaticFiles>>>
			_defaultStaticFiles =
				new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, DefaultStaticFiles>>>();

		public DefaultStaticFilesFactory(IOptions<ReusableMvcOptions> options,
			IRazorViewEngineFileProviderAccessor fileProviderAccessor,
			PipelineDecisionTree<ReusableMvcOptions> pipelineDecisionTree, IHostingEnvironment env)
		{
			_options = options;
			_pipelineDecisionTree = pipelineDecisionTree;
			_minifySuffix = env.IsDevelopment() ? null : ".min";
			_fileProvider = fileProviderAccessor.FileProvider;
		}

		/// <summary>
		/// DefaultStaticFiles查找规则pipeline => {controller}/({pipeline}/){view name}
		/// 如果指定了完整的ViewPath，则无法使用对应的静态资源
		/// </summary>
		/// <param name="actionContextAccessor"></param>
		/// <returns></returns>
		public DefaultStaticFiles Resolve(IActionContextAccessor actionContextAccessor)
		{
			var ctx = actionContextAccessor.ActionContext;
			var pipeline = ctx.HttpContext.GetPipeline();
			var pipelineName = pipeline?.Name ?? "Null";
			var controllerName = (ctx.RouteData.Values["controller"] as string)?.ToLower();
			var viewName = (ctx.HttpContext.Items[_options.Value.ViewNameHttpContextItemKey] as string)?.ToLower();
			if (string.IsNullOrEmpty(viewName))
			{
				throw new ArgumentNullException(viewName,
					"can not inject default static files if using specific view path as viewName");
			}
			var defaultStaticFiles = _defaultStaticFiles.GetOrAdd(pipelineName,
					t => new ConcurrentDictionary<string, ConcurrentDictionary<string, DefaultStaticFiles>>())
				.GetOrAdd(controllerName, t => new ConcurrentDictionary<string, DefaultStaticFiles>())
				.GetOrAdd(viewName,
					t =>
					{
						var defaultStaticFileLocations = new List<string>();
						if (pipeline != null)
						{
							var pipelinePath = _pipelineDecisionTree.GetPipelinePath(pipeline);
							defaultStaticFileLocations.AddRange(
								pipelinePath.Select(
									p =>
										string.Format(_options.Value.PipelineOptions[pipeline].StaticFilesLocationTemplate, viewName, controllerName,
											pipeline.Name)));
						}
                        defaultStaticFileLocations.Add($"{controllerName}/{viewName}");
                        var f = new DefaultStaticFiles();
						foreach (var l in defaultStaticFileLocations.Select(t1 => t1.Replace("//", "/").ToLower()))
						{
							if (string.IsNullOrEmpty(f.Css) && _fileProvider.GetFileInfo($"wwwroot/css/{l}{_minifySuffix}.css").Exists)
							{
								f.Css = $"/css/{l}{_minifySuffix}.css";
							}
							if (string.IsNullOrEmpty(f.Js) && _fileProvider.GetFileInfo($"wwwroot/js/{l}{_minifySuffix}.js").Exists)
							{
								f.Js = $"/js/{l}{_minifySuffix}.js";
							}
						}
						return f;
					});
			return defaultStaticFiles;
		}
	}
}