using System;
using System.Collections.Generic;
using System.Text;
using LazyMortal.Multipipeline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;

namespace LazyMortal.ReusableMvc.Pipelines
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReusablePipeline : IPipeline
    {
        /// <summary>
        /// Find current pipeline's controller for current request.
        /// </summary>
        string[] GetControllerFullnames(RouteContext routeContext);

        /// <summary>
        /// Find current pipeline's view for current request.
        /// </summary>
        string[] GetViewLocations(ViewLocationExpanderContext viewLocationExpanderContext);

        /// <summary>
        /// Find layout of this pipeline.
        /// </summary>
        string[] GetSharedViewLocations(ViewLocationExpanderContext viewLocationExpanderContext);

        /// <summary>
        /// Find current pipeline'static files for current request.
        /// </summary>
        string[] GetStaticFilesRelativeLocations(ActionContext actionContext, string viewName);
    }
}