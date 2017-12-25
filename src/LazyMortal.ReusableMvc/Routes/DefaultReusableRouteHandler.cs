using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LazyMortal.Multipipeline;
using LazyMortal.Multipipeline.DecisionTree;
using LazyMortal.ReusableMvc.Options;
using LazyMortal.ReusableMvc.Pipelines;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace LazyMortal.ReusableMvc.Routes
{
	public class DefaultReusableRouteHandler : IReusableRouter
	{
		private readonly IRouter _target;
		private readonly IDictionary<string, IEnumerable<string>> _controllerActions;
		private readonly PipelineDecisionTree _pipelineDecisionTree;
		//private readonly string _projectBaseNameSpace;
		private readonly IOptions<ReusableMvcOptions> _options;

		private readonly
			ConcurrentDictionary<IPipeline, ConcurrentDictionary<string, ConcurrentDictionary<string, IPipeline>>>
			_pipelineActions =
				new ConcurrentDictionary<IPipeline, ConcurrentDictionary<string, ConcurrentDictionary<string, IPipeline>>>();

		public DefaultReusableRouteHandler(MvcRouteHandler target, IActionDescriptorCollectionProvider actionProvider,
			PipelineDecisionTree pipelineDecisionTree, IOptions<ReusableMvcOptions> options)
		{
			_target = target;
			_pipelineDecisionTree = pipelineDecisionTree;
			_options = options;
            //_controllerActionUris =
            //    actionProvider.ActionDescriptors.Items.Cast<ControllerActionDescriptor>()
            //        .GroupBy(t => t.ControllerTypeInfo.FullName)
            //        .ToDictionary(t => t.Key,
            //            t => t.Select(x => (x.AttributeRouteInfo != null
            //                ? x.AttributeRouteInfo.Template
            //                : $"{x.ControllerName}/{x.ActionName}").Trim('/')), StringComparer.OrdinalIgnoreCase);
            _controllerActions =
                actionProvider.ActionDescriptors.Items.Cast<ControllerActionDescriptor>()
                    .GroupBy(t => t.ControllerTypeInfo.FullName)
                    .ToDictionary(t => t.Key,
                        t => t.Select(x => x.ActionName), StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Change the route values temporarily if current url meets the rules of parents of current pipeline for locating the action, and restore them after.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task RouteAsync(RouteContext context)
		{
			var pipeline = context.HttpContext.GetPipeline() as ReusablePipeline;
			if (pipeline != null)
			{
				ChangeRouteDataToLocatedAction(context, pipeline);
			}
			try
			{
				return _target.RouteAsync(context);
			}
			finally
			{
				if (!string.IsNullOrEmpty(pipeline?.Name))
				{
					RestoreRouteData(context, pipeline);
				}
			}
		}

		public VirtualPathData GetVirtualPath(VirtualPathContext context)
		{
			return null;
		}

		public virtual void ChangeRouteDataToLocatedAction(RouteContext context, ReusablePipeline pipeline)
		{
		    //var uri = context.HttpContext.Request.Path.Value.Trim('/');
			var controllerName = context.RouteData.Values["controller"].ToString();
		    var actionName = context.RouteData.Values["action"].ToString();
			var targetPipeline = _pipelineActions.GetOrAdd(pipeline,
					t => new ConcurrentDictionary<string, ConcurrentDictionary<string, IPipeline>>(StringComparer.OrdinalIgnoreCase))
				.GetOrAdd(controllerName, t => new ConcurrentDictionary<string, IPipeline>(StringComparer.OrdinalIgnoreCase))
				.GetOrAdd(actionName, t1 =>
				{
					var pipelinePath = _pipelineDecisionTree.GetPipelinePath(pipeline);
					foreach (var p in pipelinePath)
					{
						var fullControllerNames = p.GetControllerFullnames(context);
					    foreach (var fullControllerName in fullControllerNames)
					    {
					        if (_controllerActions.TryGetValue(fullControllerName, out var actions))
					        {
					            if (actions.Contains(actionName, StringComparer.OrdinalIgnoreCase))
					            {
					                return p;
					            }
					        }
					    }
					}
					return null;
				});
            //todo: set route data values by pipeline self.
			if (string.IsNullOrEmpty(targetPipeline?.Name))
			{
				context.RouteData.Values.Remove(_options.Value.PipelineNameRouteDataKey);
			}
			else
			{
				context.RouteData.Values[_options.Value.PipelineNameRouteDataKey] = targetPipeline.Name.ToLower();
			}
		}

		public virtual void RestoreRouteData(RouteContext context, ReusablePipeline pipeline)
		{
		    //todo: set route data values by pipeline self.
            context.RouteData.Values[_options.Value.PipelineNameRouteDataKey] = pipeline.Name.ToLower();
		}
	}
}