using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyMortal.Multipipeline;
using LazyMortal.ReusableMvc.Pipelines;
using Microsoft.AspNetCore.Routing;

namespace LazyMortal.ReusableMvc.Routes
{
	/// <summary>
	/// Only <see cref="ReusablePipeline"/> affect the behavior of routing, and putting pipeline in parameters here is for reducing the cost of cast/as.
	/// You can replace the default <see cref="DefaultReusableRouteHandler"/> by adding services.AddSingleton&lt;IReusableRouter, MyReusableRouter>
	/// </summary>
	public interface IReusableRouter : IRouter
	{
		/// <summary>
		/// Temporary change current route data for invoking the action located by reusable rules, and then the route data will be restored.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="pipeline"></param>
		void ChangeRouteDataToLocatedAction(RouteContext context, ReusablePipeline pipeline);

		/// <summary>
		/// Restore the route data.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="pipeline"></param>
		void RestoreRouteData(RouteContext context, ReusablePipeline pipeline);
	}
}