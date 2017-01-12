using System;
using Anobaka.ReusableMvc.Options;
using Anobaka.ReusableMvc.StaticFiles;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anobaka.ReusableMvc.Extensions
{
	public static class ReusableMvcServiceCollectionExtensions
	{
		public static IServiceCollection AddReusableMvc(this IServiceCollection services,
			Action<ReusableMvcOptions> reusableMvcOptionsAction)
		{
			return services.AddReusableMvc<DefaultStaticFiles, DefaultStaticFilesFactory>(reusableMvcOptionsAction);
		}

		public static IServiceCollection AddReusableMvc<TStaticFiles, TStaticFilesFactory>(this IServiceCollection services,
			Action<ReusableMvcOptions> reusableMvcOptionsAction = null)
			where TStaticFilesFactory : class, IStaticFilesFactory<TStaticFiles> where TStaticFiles : class
		{
			reusableMvcOptionsAction = reusableMvcOptionsAction ?? (t => { });
			services.Configure(reusableMvcOptionsAction);
			services.TryAddSingleton<TStaticFilesFactory>();
			services.TryAddScoped(
				t => t.GetRequiredService<TStaticFilesFactory>().Resolve(t.GetRequiredService<IActionContextAccessor>()));
			services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
			return services;
		}
	}
}