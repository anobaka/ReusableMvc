using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LazyMortal.Multipipeline;
using LazyMortal.ReusableMvc.Options;
using LazyMortal.ReusableMvc.StaticFiles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;

namespace LazyMortal.ReusableMvc.Pipelines
{
    /// <summary>
    /// Default abstract reusable pipeline, but not standard.
    /// </summary>
    public abstract class ReusablePipeline : IReusablePipeline
    {
        public string Id { get; }
        public string ParentId { get; }
        public string Name { get; }

        protected static readonly string ProjectBaseNamespace = Assembly.GetEntryAssembly().GetName().Name;

        protected ReusablePipelineOptions Options;

        protected ReusablePipeline(ReusablePipelineOptions options)
        {
            Id = options.Id;
            ParentId = options.ParentId;
            Name = options.Name;
            Options = options;
        }

        //public virtual void ChangeRouteDataToLocatedAction(RouteContext context)
        //{

        //}

        //public virtual void RestoreRouteDataAfterActionLocating(RouteContext routeContext)
        //{
            
        //}

        public abstract Task<bool> ResolveAsync(HttpContext ctx);

        public abstract Task ConfigurePipeline(IApplicationBuilder app);

        /// <inheritdoc />
        /// <summary>
        /// Default fullname is "{0}.Areas.{1}.Controllers.{2}".
        /// <para>{0} is for project's base namespace</para>
        /// <para>{1} is for controller's name trimed end with 'Controller'</para>
        /// <para>{2} is for pipeline's name</para>
        /// </summary>
        /// <param name="routeContext"></param>
        /// <returns></returns>
        public virtual string[] GetControllerFullnames(RouteContext routeContext)
        {
            var controllerName = routeContext.RouteData.Values["controller"].ToString();
            return new[] {$"{ProjectBaseNamespace}.Areas.{Name}.Controllers.{controllerName}Controller"};
        }

        /// <inheritdoc />
        /// <summary>
        /// Default location is "/Views/{1}{2}{0}.cshtml", the "//" will be replaced by "/" if the value is null.
        /// <para>{0} is for view's name.</para>
        /// <para>{1} is for controller's name.</para>
        /// <para>{2} is for pipeline's name.</para>
        /// </summary>
        /// <param name="viewLocationExpanderContext"></param>
        /// <returns></returns>
        public virtual string[] GetViewLocations(ViewLocationExpanderContext viewLocationExpanderContext)
        {
            return new[] {$"/Views/{{1}}/{Name}/{{0}}.cshtml".Replace("//", "/")};
        }

        /// <inheritdoc />
        /// <summary>
        /// Default location is "/Views/Shared/{1}/{0}.cshtml".
        /// <para>{0} is for view's name.</para>
        /// <para>{1} is for pipeline's name.</para>
        /// </summary>
        /// <param name="viewLocationExpanderContext"></param>
        /// <returns></returns>
        public virtual string[] GetSharedViewLocations(ViewLocationExpanderContext viewLocationExpanderContext)
        {
            return new[] {$"/Views/Shared/{Name}/{{0}}.cshtml".Replace("//", "/")};
        }

        /// <inheritdoc />
        /// <summary>
        /// default location is "{1}/{2}/{0}", the "//" will be replaced by "/" if the value is null.
        /// <para>{0} is for view's name.</para>
        /// <para>{1} is for controller's name.</para>
        /// <para>{2} is for pipeline's name in lower case.</para>
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public virtual string[] GetStaticFilesRelativeLocations(ActionContext actionContext, string viewName)
        {
            var controllerName = (actionContext.RouteData.Values["controller"] as string)?.ToLower();
            return new[] {$"{controllerName}/{Name}/{viewName}"};
        }
    }

    public abstract class ReusablePipeline<TOptions> : ReusablePipeline where TOptions : ReusablePipelineOptions
    {
        protected ReusablePipeline(TOptions options) : base(options)
        {
            Options = options;
        }

        protected new TOptions Options;
    }
}