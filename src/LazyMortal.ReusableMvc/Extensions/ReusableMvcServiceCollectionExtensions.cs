using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LazyMortal.Multipipeline;
using LazyMortal.ReusableMvc.Options;
using LazyMortal.ReusableMvc.Pipelines;
using LazyMortal.ReusableMvc.Routes;
using LazyMortal.ReusableMvc.StaticFiles;
using LazyMortal.ReusableMvc.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace LazyMortal.ReusableMvc.Extensions
{
    public static class ReusableMvcServiceCollectionExtensions
    {
        public static IServiceCollection AddDefaultReusableMvcComponents(this IServiceCollection services,
            Action<MultipipelineOptions> multipipelineOptionsAction = null,
            Action<ReusableMvcOptions> reusableMvcOptionsAction = null)
        {
            return services
                .AddReusableMvcComponents<DefaultStaticFiles, DefaultStaticFilesFactory,
                    DefaultResuableViewLocationExpander, DefaultReusableRouteHandler>(multipipelineOptionsAction,
                    reusableMvcOptionsAction);
        }

        public static IServiceCollection AddReusableMvcComponents<TStaticFiles, TStaticFilesFactory,
            TRusableViewLocationExpander, TReusableRouter>(this IServiceCollection services,
            Action<MultipipelineOptions> multipipelineOptionsAction = null,
            Action<ReusableMvcOptions> reusableMvcOptionsAction = null)
            where TStaticFilesFactory : class, IStaticFilesFactory<TStaticFiles>
            where TStaticFiles : class
            where TReusableRouter : class, IReusableRouter
            where TRusableViewLocationExpander : class, IReusableViewLocationExpander
        {
            services.Configure(reusableMvcOptionsAction ?? (t => { }));

            services.TryAddSingleton<TStaticFilesFactory>();
            services.TryAddSingleton<IReusableViewLocationExpander, TRusableViewLocationExpander>();
            services.TryAddScoped(t => t.GetRequiredService<TStaticFilesFactory>().Resolve());
            services.TryAddSingleton<IReusableRouter, TReusableRouter>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            return services;
        }
    }
}