using System;
using Anobaka.Multipipeline.DecisionTree;
using Anobaka.ReusableMvc.Options;
using Anobaka.ReusableMvc.Views;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Anobaka.ReusableMvc.Extensions
{
	public static class ReusableMvcApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseReusableMvc(this IApplicationBuilder app)
		{
			return app.UseReusableMvc(t =>
			{
				t.ApplicationServices.GetRequiredService<IOptions<RazorViewEngineOptions>>()
					.Value.ViewLocationExpanders.Add(
						new ResuableViewLocationExpander(app.ApplicationServices.GetRequiredService<IOptions<ReusableMvcOptions>>(),
							t.ApplicationServices.GetRequiredService<PipelineDecisionTree<ReusableMvcOptions>>()));
			});
		}

		public static IApplicationBuilder UseReusableMvc(this IApplicationBuilder app,
			Action<IApplicationBuilder> configureAction)
		{
			configureAction(app);
			app.UseMvc(routes =>
			{
				routes.MapReusableRoute(
					name: "reusable",
					template: "{area}/{controller=Home}/{action=Index}/{id?}");
				//todo: trying to remove this.
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			return app;
		}
	}
}