using System;
using LazyMortal.Multipipeline;
using LazyMortal.ReusableMvc.Options;
using LazyMortal.ReusableMvc.Routes;
using LazyMortal.ReusableMvc.StaticFiles;
using LazyMortal.ReusableMvc.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace LazyMortal.ReusableMvc.Extensions
{
	public static class ReusableMvcServiceCollectionExtensions
	{
		public static IServiceCollection AddReusableMvcWithDefaultStaticFiles(this IServiceCollection services,
			Action<ReusableMvcOptions> reusableMvcOptionsAction = null, Action<MvcOptions> mvcSetupOptions = null)
		{
			return services.AddReusableMvc<DefaultStaticFiles, DefaultStaticFilesFactory>(reusableMvcOptionsAction,
				mvcSetupOptions);
		}

		public static IServiceCollection AddReusableMvc<TStaticFiles, TStaticFilesFactory>(this IServiceCollection services,
			Action<ReusableMvcOptions> reusableMvcOptionsAction = null, Action<MvcOptions> mvcSetupOptions = null)
			where TStaticFilesFactory : class, IStaticFilesFactory<TStaticFiles> where TStaticFiles : class
		{
			services.Configure(reusableMvcOptionsAction ?? (t => { }));

			services.AddMultipipeline();

			services.TryAddSingleton<TStaticFilesFactory>();
			services.TryAddSingleton<IReusableViewLocationExpander, DefaultResuableViewLocationExpander>();
			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.TryAddScoped(t => t.GetRequiredService<TStaticFilesFactory>().Resolve());
			services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
			services.TryAddSingleton<IReusableRouter, DefaultReusableRouteHandler>();

			services.AddMvc(mvcSetupOptions ?? (t => { }));
			return services;
		}
	}
}