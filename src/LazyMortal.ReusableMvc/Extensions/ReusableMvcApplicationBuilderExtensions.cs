using System;
using System.Linq;
using System.Text.RegularExpressions;
using LazyMortal.Multipipeline;
using LazyMortal.Multipipeline.DecisionTree;
using LazyMortal.ReusableMvc.Options;
using LazyMortal.ReusableMvc.Pipelines;
using LazyMortal.ReusableMvc.Routes;
using LazyMortal.ReusableMvc.Views;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LazyMortal.ReusableMvc.Extensions
{
	public static class ReusableMvcApplicationBuilderExtensions
	{
		/// <summary>
		/// <para>Default route is {area}/{controller=Home}/{action=Index}/{id?}, and the default route is {controller=Home}/{action=Index}/{id?}.</para>
		/// <para>Use <see cref="UseReusableMvc"/> for more route configurations.</para>
		/// </summary>
		/// <param name="app"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseReusableMvcWithDefaultRoute(this IApplicationBuilder app)
		{
			var options = app.ApplicationServices.GetRequiredService<IOptions<ReusableMvcOptions>>();
			return app.UseReusableMvc(routes =>
			{
				routes.MapRoute(
					name: "reusable",
					template: $"{{{options.Value.PipelineNameRouteDataKey}}}/{{controller=Home}}/{{action=Index}}/{{id?}}");
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="app"></param>
		/// <param name="configureRoutes">configure the routes</param>
		/// <returns></returns>
		public static IApplicationBuilder UseReusableMvc(this IApplicationBuilder app, Action<IRouteBuilder> configureRoutes)
		{
			app.ApplicationServices.GetRequiredService<IOptions<RazorViewEngineOptions>>()
				.Value.ViewLocationExpanders.Add(app.ApplicationServices.GetRequiredService<IReusableViewLocationExpander>());

			foreach (
				var reusablePipeline in
				app.ApplicationServices.GetRequiredService<PipelineCollectionAccessor>()
					.Pipelines.Cast<ReusablePipeline>()
					.Distinct())
			{
				reusablePipeline.ViewLocationTemplate =
					Regex.Replace(reusablePipeline.ViewLocationTemplate.Replace("{2}", reusablePipeline.Name), @"/{2,}", "/");
				reusablePipeline.SharedViewLocationTemplate =
					Regex.Replace(reusablePipeline.SharedViewLocationTemplate.Replace("{1}",
						reusablePipeline.Name), @"/{2,}", "/");
				reusablePipeline.StaticFilesLocationTemplate =
					Regex.Replace(reusablePipeline.StaticFilesLocationTemplate.Replace("{2}",
						reusablePipeline.Name.ToLower()), @"/{2,}", "/");
				reusablePipeline.ControllerFullNameTemplate =
					Regex.Replace(reusablePipeline.ControllerFullNameTemplate.Replace("{2}",
						reusablePipeline.Name), @"\.{2,}", ".");
			}

			app.UseMvc(routes =>
			{
				routes.DefaultHandler = routes.ServiceProvider.GetRequiredService<IReusableRouter>();
				configureRoutes(routes);
			});

			return app;
		}
	}
}