using System.Collections.Generic;
using System.Linq;
using Anobaka.ReusableMvc.Options;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Anobaka.Multipipeline;
using Anobaka.Multipipeline.DecisionTree;

namespace Anobaka.ReusableMvc.Views
{
    /// <summary>
    /// 直接使用完整路径则不会进入ViewLocationExpander
    /// </summary>
    public class ResuableViewLocationExpander : IViewLocationExpander
    {
        private readonly IOptions<ReusableMvcOptions> _options;
        private readonly PipelineDecisionTree<ReusableMvcOptions> _pipelineDecisionTree;

        public ResuableViewLocationExpander(IOptions<ReusableMvcOptions> options,
            PipelineDecisionTree<ReusableMvcOptions> pipelineDecisionTree)
        {
            _options = options;
            _pipelineDecisionTree = pipelineDecisionTree;
        }

        /// <summary>
        /// 每次查找View都会调用该方法
        /// </summary>
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            if (context.IsMainPage)
            {
                context.ActionContext.HttpContext.Items[_options.Value.ViewNameHttpContextItemKey] = context.ViewName;
            }
        }

        /// <summary>
        /// 没有view路径缓存时调用该方法查找view
        /// 规则1：所有View路径大小写与Controller以及ViewName一致
        /// 规则2：路径查找规则：/Views/{Controller}(/{PipelineName1}>{BasePipelineName2}>{BasePipelineName3})/{ViewName}.cshtml
        /// </summary>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var pipeline = context.ActionContext.HttpContext.GetPipeline();
            if (!string.IsNullOrEmpty(pipeline?.Name))
            {
                var pipelinePath = _pipelineDecisionTree.GetPipelinePath(pipeline);
                var tmpViewLocations =
                    pipelinePath.Select(
                        p => string.IsNullOrEmpty(p.Name) ? "/Views/{1}/{0}.cshtml" : $"/Views/{{1}}/{p.Name}/{{0}}.cshtml").ToList();
                tmpViewLocations.Add("/Views/{1}/{0}.cshtml");
                tmpViewLocations.AddRange(pipelinePath.Where(t => !string.IsNullOrEmpty(t.Name))
                    .Select(t => $"/Views/Shared/{t.Name}/{{0}}.cshtml"));
                tmpViewLocations.Add("/Views/Shared/{0}.cshtml");
                viewLocations = tmpViewLocations.Distinct();
            }
            return viewLocations;
        }
    }
}