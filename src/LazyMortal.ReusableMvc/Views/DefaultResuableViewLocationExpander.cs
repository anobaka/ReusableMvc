using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using LazyMortal.ReusableMvc.Options;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using LazyMortal.Multipipeline;
using LazyMortal.Multipipeline.DecisionTree;
using LazyMortal.ReusableMvc.Pipelines;

namespace LazyMortal.ReusableMvc.Views
{
	/// <summary>
	/// 直接使用完整路径则不会进入ViewLocationExpander
	/// </summary>
	public class DefaultResuableViewLocationExpander : IReusableViewLocationExpander
	{
		private readonly IOptions<ReusableMvcOptions> _options;
		private readonly PipelineDecisionTree _pipelineDecisionTree;

		public DefaultResuableViewLocationExpander(IOptions<ReusableMvcOptions> options,
			PipelineDecisionTree pipelineDecisionTree)
		{
			_options = options;
			_pipelineDecisionTree = pipelineDecisionTree;
		}

		/// <summary>
		/// 每次查找View都会调用该方法
		/// </summary>
		public virtual void PopulateValues(ViewLocationExpanderContext context)
		{
			if (context.IsMainPage)
			{
				context.ActionContext.HttpContext.Items[_options.Value.ViewNameHttpContextItemKey] = context.ViewName;
			}
		}

		/// <summary>
		/// 没有view路径缓存时调用该方法查找view
		/// 规则1：所有View路径大小写与Controller以及ViewName一致
		/// </summary>
		public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,
			IEnumerable<string> viewLocations)
		{
			var pipeline = context.ActionContext.HttpContext.GetPipeline() as ReusablePipeline;
			if (pipeline != null)
			{
				var pipelinePath = _pipelineDecisionTree.GetPipelinePath(pipeline);
				//main view
				var tmpViewLocations = pipelinePath.Select(p => p.ViewLocationTemplate).ToList();
				tmpViewLocations.Add(_options.Value.DefaultViewLocation);
				//shared view
				tmpViewLocations.AddRange(pipelinePath.Select(t => t.SharedViewLocationTemplate));
				tmpViewLocations.Add(_options.Value.DefaultLayoutLocation);
				viewLocations = tmpViewLocations.Concat(viewLocations).Distinct();
			}
			return viewLocations;
		}
	}
}