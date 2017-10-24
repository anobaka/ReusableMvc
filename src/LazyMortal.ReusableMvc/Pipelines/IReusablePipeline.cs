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
    public interface IReusablePipeline : IPipeline
    {
        /// <summary>
        /// used for finding current pipeline's controller for current request.
        /// <para>{0} is for project's base namespace</para>
        /// <para>{1} is for controller's name trimed end with 'Controller'</para>
        /// <para>{2} is for pipeline's name</para>
        /// <para>default value is "{0}.Areas.{1}.Controllers.{2}"</para>
        /// </summary>
        List<string> GetControllerFullnames(RouteContext routeContext);
        //public string ControllerFullNameTemplate { get; set; } = "{0}.Areas.{2}.Controllers.{1}Controller";

        /// <summary>
        /// used for finding current pipeline's view for current request
        /// <para>{0} is for view's name</para>
        /// <para>{1} is for controller's name</para>
        /// <para>{2} is for pipeline's name</para>
        /// <para>default value is "/Views/{1}{2}{0}.cshtml", the "//" will be replaced by "/" if the value is null</para>
        /// </summary>
        List<string> GetViewLocations(ViewLocationExpanderContext viewLocationExpanderContext);

        //public string ViewLocationTemplate { get; set; } = "/Views/{1}/{2}/{0}.cshtml";
        /// <summary>
        /// used for finding layout of this pipeline
        /// <para>{0} is for view's name</para>
        /// <para>{1} is for pipeline's name</para>
        /// </summary>
        List<string> GetSharedViewLocations(ViewLocationExpanderContext viewLocationExpanderContext);

        //public string SharedViewLocationTemplate { get; set; } = "/Views/Shared/{1}/{0}.cshtml";
        /// <summary>
        /// used for finding current pipeline'sstatic files for current request
        /// <para>{0} is for view's name</para>
        /// <para>{1} is for controller's name</para>
        /// <para>{2} is for pipeline's name.ToLower()</para>
        /// <para>default value is "{1}/{2}/{0}", the "//" will be replaced by "/" if the value is null</para>
        /// </summary>
        List<string> GetStaticFilesRelativeLocations(ActionContext actionContext);
        
        //public string StaticFilesLocationTemplate { get; set; } = "{1}/{2}/{0}";
    }
}