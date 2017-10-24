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

        protected ReusableMvcOptions ReusableMvcOptions;

        protected ReusablePipeline(ReusablePipelineOptions options, ReusableMvcOptions reusableMvcOptions)
        {
            Id = options.Id;
            ParentId = options.ParentId;
            Name = options.Name;
            ReusableMvcOptions = reusableMvcOptions;
        }

        public abstract Task<bool> ResolveAsync(HttpContext ctx);

        public abstract Task ConfigurePipeline(IApplicationBuilder app);

        public virtual List<string> GetControllerFullnames(RouteContext routeContext)
        {
            var controllerName = routeContext.RouteData.Values["controller"].ToString();
            return new List<string> {$"{ProjectBaseNamespace}.Areas.{Name}.Controllers.{controllerName}Controller"};
        }

        public virtual List<string> GetViewLocations(ViewLocationExpanderContext viewLocationExpanderContext)
        {
            var pipelineName =
                (viewLocationExpanderContext.ActionContext.HttpContext.GetPipeline() as IReusablePipeline)?.Name;
            return new List<string> {$"/Views/{{1}}/{pipelineName}/{{0}}.cshtml".Replace("//", "/")};
        }

        public virtual List<string> GetSharedViewLocations(ViewLocationExpanderContext viewLocationExpanderContext)
        {
            var pipelineName =
                (viewLocationExpanderContext.ActionContext.HttpContext.GetPipeline() as IReusablePipeline)?.Name;
            return new List<string> {$"/Views/Shared/{pipelineName}/{{0}}.cshtml".Replace("//", "/")};
        }

        public virtual List<string> GetStaticFilesRelativeLocations(ActionContext actionContext)
        {
            var controllerName = (actionContext.RouteData.Values["controller"] as string)?.ToLower();
            var viewName = (actionContext.HttpContext.Items[ReusableMvcOptions.ViewNameHttpContextItemKey] as string)
                ?.ToLower();
            return new List<string> {$"{controllerName}/{Name}/{viewName}"};
        }
    }

    public abstract class ReusablePipeline<TOptions, TMvcOptions> : ReusablePipeline
        where TOptions : ReusablePipelineOptions where TMvcOptions : ReusableMvcOptions
    {
        protected ReusablePipeline(TOptions options, TMvcOptions reusableMvcOptions) : base(
            options, reusableMvcOptions)
        {
            Options = options;
        }

        protected new TMvcOptions ReusableMvcOptions;

        protected TOptions Options;
    }
}