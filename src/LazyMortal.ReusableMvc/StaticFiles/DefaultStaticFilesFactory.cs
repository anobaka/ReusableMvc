using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LazyMortal.Multipipeline;
using LazyMortal.Multipipeline.DecisionTree;
using LazyMortal.ReusableMvc.Options;
using LazyMortal.ReusableMvc.Pipelines;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        private readonly string _defaultFakePipelineId = Guid.NewGuid().ToString();
        private readonly IHostingEnvironment _env;

        /// <summary>
        /// pipeline + controller + viewname => staticfiles:{controller}/({pipeline}/){view name}
        /// </summary>
        private readonly ConcurrentDictionary<string, DefaultStaticFiles> _defaultStaticFiles =
            new ConcurrentDictionary<string, DefaultStaticFiles>();

        public DefaultStaticFilesFactory(IOptions<ReusableMvcOptions> options,
            IRazorViewEngineFileProviderAccessor fileProviderAccessor,
            PipelineDecisionTree pipelineDecisionTree, IHostingEnvironment env,
            IActionContextAccessor actionContextAccessor)
        {
            _options = options;
            _pipelineDecisionTree = pipelineDecisionTree;
            _actionContextAccessor = actionContextAccessor;
            _minifySuffix = env.IsDevelopment() ? null : ".min";
            _fileProvider = fileProviderAccessor.FileProvider;
            _env = env;
        }

        /// <inheritdoc />
        /// <summary>
        /// if the viewpath is a full or absolute path, it won't locate the location of static files.
        /// </summary>
        /// <returns></returns>
        public virtual DefaultStaticFiles Resolve()
        {
            var ctx = _actionContextAccessor.ActionContext;
            var pipeline = ctx.HttpContext.GetPipeline() as IReusablePipeline;
            //the dictionary can not use null as a key.
            var pipelineId = pipeline?.Id ?? _defaultFakePipelineId;
            var controllerName = (ctx.RouteData.Values["controller"] as string)?.ToLower();
            var viewName = (ctx.HttpContext.Items[_options.Value.ViewNameHttpContextItemKey] as string)?.ToLower();
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentNullException(viewName,
                    "Can not inject default static files if using specific view path as viewName");
            }
            var key = $"{pipelineId}->{controllerName}->{viewName}";
            return _env.IsDevelopment()
                ? GetDefaultStaticFiles(ctx, pipeline)
                : _defaultStaticFiles.GetOrAdd(key, t => GetDefaultStaticFiles(ctx, pipeline));
        }

        /// <summary>
        /// Find default static files by context and pipeline.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        protected virtual DefaultStaticFiles GetDefaultStaticFiles(ActionContext actionContext,
            IReusablePipeline pipeline)
        {
            var controllerActionDescriptor = actionContext.ActionDescriptor as ControllerActionDescriptor;
            List<string> noStaticFileTypes = null;
            if (controllerActionDescriptor != null)
            {
                var methodAttributes = controllerActionDescriptor.MethodInfo
                    .GetCustomAttributes<NoDefaultStaticFilesAttribute>()?.ToList();
                if (methodAttributes?.Any() == true)
                {
                    noStaticFileTypes = methodAttributes.Where(t => t.FileTypes != null).SelectMany(t => t.FileTypes)
                        .ToList();
                }
                else
                {
                    var controllerAttributes = controllerActionDescriptor.ControllerTypeInfo
                        .GetCustomAttributes<NoDefaultStaticFilesAttribute>()?.ToList();
                    if (controllerAttributes?.Any() == true)
                    {
                        noStaticFileTypes = controllerAttributes.Where(t => t.FileTypes != null)
                            .SelectMany(t => t.FileTypes).ToList();
                    }
                }
            }
            var controllerName = (actionContext.RouteData.Values["controller"] as string)?.ToLower();
            var viewName = (actionContext.HttpContext.Items[_options.Value.ViewNameHttpContextItemKey] as string)
                ?.ToLower();
            var defaultStaticFileLocations = new List<string>();
            if (pipeline != null)
            {
                var pipelinePath = _pipelineDecisionTree.GetPipelinePath(pipeline);
                defaultStaticFileLocations.AddRange(pipelinePath.SelectMany(p =>
                    p.GetStaticFilesRelativeLocations(actionContext, viewName)));
            }
            if (!string.IsNullOrEmpty(_options.Value.DefaultStaticFileLocation))
            {
                defaultStaticFileLocations.Add($"{controllerName}/{viewName}");
            }
            var f = new DefaultStaticFiles();
            var fileTypes = new[] {"css", "js"};
            foreach (var type in fileTypes)
            {
                foreach (var l in defaultStaticFileLocations.Select(t1 => t1.Replace("//", "/").ToLower()))
                {
                    if (noStaticFileTypes?.Contains(type) != true &&
                        _fileProvider.GetFileInfo($"wwwroot/{type}/{l}{_minifySuffix}.{type}").Exists)
                    {
                        f[type] = $"/{type}/{l}{_minifySuffix}.{type}";
                        break;
                    }
                }
            }
            return f;
        }
    }
}