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
	/// <inheritdoc />
	/// <summary>
	/// Default view locator.
	/// </summary>
	public class DefaultResuableViewLocationExpander : IReusableViewLocationExpander
	{
		private readonly IOptions<ReusableMvcOptions> _options;
		private readonly PipelineDecisionTree _pipelineDecisionTree;

	    /// <inheritdoc />
	    public DefaultResuableViewLocationExpander(IOptions<ReusableMvcOptions> options,
			PipelineDecisionTree pipelineDecisionTree)
		{
			_options = options;
			_pipelineDecisionTree = pipelineDecisionTree;
		}

		///<inheritdoc cref="IViewLocationExpander.PopulateValues"/>
		public virtual void PopulateValues(ViewLocationExpanderContext context)
		{
			if (context.IsMainPage)
			{
				context.ActionContext.HttpContext.Items[_options.Value.ViewNameHttpContextItemKey] = context.ViewName;
			}
		}

		/// <inheritdoc />
		/// <summary>
		/// <para>The cases of controller and view part in view location are case sensitive, and same as the original name.</para>
		/// <para>e.g. The part of 'ProductController' in view location is 'Product'</para>
		/// <para>e.g. The part of view name 'Partial/Index' in view location is 'Partial/Index'</para>
		/// </summary>
		public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,
			IEnumerable<string> viewLocations)
		{
		    if (context.ActionContext.HttpContext.GetPipeline() is IReusablePipeline pipeline)
			{
				var pipelinePath = _pipelineDecisionTree.GetPipelinePath(pipeline);
				//main view
				var tmpViewLocations = pipelinePath.SelectMany(p => p.GetViewLocations(context)).ToList();
			    if (!string.IsNullOrEmpty(_options.Value.DefaultViewLocation))
			    {
			        tmpViewLocations.Add(_options.Value.DefaultViewLocation);
                }
				//shared view
				tmpViewLocations.AddRange(pipelinePath.SelectMany(t => t.GetSharedViewLocations(context)));
			    if (!string.IsNullOrEmpty(_options.Value.DefaultLayoutLocation))
			    {
			        tmpViewLocations.Add(_options.Value.DefaultLayoutLocation);
                }
				viewLocations = tmpViewLocations.Concat(viewLocations).Distinct();
            }
			return viewLocations;
		}
	}
}