using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anobaka.Multipipeline;
using Anobaka.Multipipeline.DecisionTree;
using Anobaka.ReusableMvc.Options;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Anobaka.ReusableMvc.Routes
{
	public class ReusableRouteHandler : IRouter
	{
		private readonly IRouter _target;
		private readonly IDictionary<string, IEnumerable<string>> _controllerActionNames;
		private readonly ReusableMvcOptions _options;
		private readonly PipelineDecisionTree<ReusableMvcOptions> _pipelineDecisionTree;

		private readonly
			ConcurrentDictionary<IPipeline, ConcurrentDictionary<string, ConcurrentDictionary<string, IPipeline>>>
			_pipelineActions =
				new ConcurrentDictionary<IPipeline, ConcurrentDictionary<string, ConcurrentDictionary<string, IPipeline>>>();

		public ReusableRouteHandler(IRouter target, IActionDescriptorCollectionProvider actionProvider,
			IOptions<ReusableMvcOptions> options, PipelineDecisionTree<ReusableMvcOptions> pipelineDecisionTree)
		{
			_target = target;
			_pipelineDecisionTree = pipelineDecisionTree;
			_options = options.Value;
			_controllerActionNames =
				actionProvider.ActionDescriptors.Items.Cast<ControllerActionDescriptor>()
					.GroupBy(t => t.ControllerTypeInfo.FullName)
					.ToDictionary(t => t.Key, t => t.Select(t1 => t1.ActionName), StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// todo: 暂时不支持跨层级调用/Attribute路由
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public Task RouteAsync(RouteContext context)
		{
			var pipeline = context.HttpContext.GetPipeline();
			if (!string.IsNullOrEmpty(pipeline?.Name))
			{
				var actionName = context.RouteData.Values["action"].ToString();
				var controllerName = context.RouteData.Values["controller"].ToString();
				var targetPipeline = _pipelineActions.GetOrAdd(pipeline,
						t => new ConcurrentDictionary<string, ConcurrentDictionary<string, IPipeline>>(StringComparer.OrdinalIgnoreCase))
					.GetOrAdd(controllerName, t => new ConcurrentDictionary<string, IPipeline>(StringComparer.OrdinalIgnoreCase))
					.GetOrAdd(actionName, t1 =>
					{
						var pipelinePath = _pipelineDecisionTree.GetPipelinePath(pipeline);
						foreach (var p in pipelinePath)
						{
							var fullControllerName =
								string.Format(_options.PipelineOptions[p].ControllerFullNameTemplate, _options.ProjectBaseNameSpace, p.Name,
									$"{controllerName}Controller");
							IEnumerable<string> actionNames;
							if (_controllerActionNames.TryGetValue(fullControllerName, out actionNames))
							{
								if (actionNames.Contains(actionName, StringComparer.OrdinalIgnoreCase))
								{
									return p;
								}
							}
						}
						return null;
					});
				if (string.IsNullOrEmpty(targetPipeline?.Name))
				{
					context.RouteData.Values.Remove("area");
				}
				else
				{
					context.RouteData.Values["area"] = targetPipeline.Name.ToLower();
				}
			}
			try
			{
				return _target.RouteAsync(context);
			}
			finally
			{
				if (!string.IsNullOrEmpty(pipeline?.Name))
				{
					context.RouteData.Values["area"] = pipeline.Name.ToLower();
				}
			}
		}

		public VirtualPathData GetVirtualPath(VirtualPathContext context)
		{
			return null;
		}
	}
}