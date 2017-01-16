using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LazyMortal.Multipipeline;
using LazyMortal.Multipipeline.DecisionTree;
using LazyMortal.ReusableMvc.Options;
using LazyMortal.ReusableMvc.Pipelines;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace LazyMortal.ReusableMvc.StaticFiles
{
	public class DefaultStaticFilesFactory : IStaticFilesFactory<DefaultStaticFiles>
	{
		private readonly IOptions<ReusableMvcOptions> _options;
		private readonly IFileProvider _fileProvider;
		private readonly PipelineDecisionTree _pipelineDecisionTree;
		private readonly string _minifySuffix;
		private readonly IActionContextAccessor _actionContextAccessor;
		private readonly string _defaultFakePipelineName = Guid.NewGuid().ToString();

		/// <summary>
		/// pipelinename => controller => viewname => staticfiles:{controller}/({pipeline}/){view name}
		/// </summary>
		private readonly
			ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, DefaultStaticFiles>>>
			_defaultStaticFiles =
				new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, DefaultStaticFiles>>>();

		public DefaultStaticFilesFactory(IOptions<ReusableMvcOptions> options,
			IRazorViewEngineFileProviderAccessor fileProviderAccessor,
			PipelineDecisionTree pipelineDecisionTree, IHostingEnvironment env, IHttpContextAccessor httpContextAccessor,
			IActionContextAccessor actionContextAccessor)
		{
			_options = options;
			_pipelineDecisionTree = pipelineDecisionTree;
			_actionContextAccessor = actionContextAccessor;
			_minifySuffix = env.IsDevelopment() ? null : ".min";
			_fileProvider = fileProviderAccessor.FileProvider;
		}

		/// <summary>
		/// if the viewpath is a full or absolute path, it won't locate the location of static files.
		/// </summary>
		/// <returns></returns>
		public virtual DefaultStaticFiles Resolve()
		{
			var ctx = _actionContextAccessor.ActionContext;
			var pipeline = ctx.HttpContext.GetPipeline() as ReusablePipeline;
			//the dictionary can not use null as a key.
			var pipelineName = pipeline != null ? (pipeline.Name ?? pipeline.GetHashCode().ToString()) : _defaultFakePipelineName;
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
								pipelinePath.Select(p => string.Format(p.StaticFilesLocationTemplate, viewName, controllerName)));
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