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
		void ChangeRouteDataToLocateAction(RouteContext context, ReusablePipeline pipeline);

		void RestoreRouteData(RouteContext context, ReusablePipeline pipeline);
	}
}