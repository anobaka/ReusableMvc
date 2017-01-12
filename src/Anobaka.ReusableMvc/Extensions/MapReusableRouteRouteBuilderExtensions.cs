using Anobaka.Multipipeline.DecisionTree;
using Anobaka.ReusableMvc.Options;
using Anobaka.ReusableMvc.Routes;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Anobaka.ReusableMvc.Extensions
{
	/// <summary>
	/// todo: consider to use 'MapAreaRoute' instead of this.
	/// area constraint should be '?' instead of ':exist', or the response will be 404 when there is no controller matched.
	/// </summary>
	public static class MapReusableRouteRouteBuilderExtensions
	{
		/// <summary>
		/// Adds a route to the <see cref="IRouteBuilder"/> with the specified name and template.
		/// </summary>
		/// <param name="routeBuilder">The <see cref="IRouteBuilder"/> to add the route to.</param>
		/// <param name="name">The name of the route.</param>
		/// <param name="template">The URL pattern of the route.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public static IRouteBuilder MapReusableRoute(
			this IRouteBuilder routeBuilder,
			string name,
			string template)
		{
			MapReusableRoute(routeBuilder, name, template, defaults: null);
			return routeBuilder;
		}

		/// <summary>
		/// Adds a route to the <see cref="IRouteBuilder"/> with the specified name, template, and default values.
		/// </summary>
		/// <param name="routeBuilder">The <see cref="IRouteBuilder"/> to add the route to.</param>
		/// <param name="name">The name of the route.</param>
		/// <param name="template">The URL pattern of the route.</param>
		/// <param name="defaults">
		/// An object that contains default values for route parameters. The object's properties represent the names
		/// and values of the default values.
		/// </param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public static IRouteBuilder MapReusableRoute(
			this IRouteBuilder routeBuilder,
			string name,
			string template,
			object defaults)
		{
			return MapReusableRoute(routeBuilder, name, template, defaults, constraints: null);
		}

		/// <summary>
		/// Adds a route to the <see cref="IRouteBuilder"/> with the specified name, template, default values, and
		/// constraints.
		/// </summary>
		/// <param name="routeBuilder">The <see cref="IRouteBuilder"/> to add the route to.</param>
		/// <param name="name">The name of the route.</param>
		/// <param name="template">The URL pattern of the route.</param>
		/// <param name="defaults">
		/// An object that contains default values for route parameters. The object's properties represent the names
		/// and values of the default values.
		/// </param>
		/// <param name="constraints">
		/// An object that contains constraints for the route. The object's properties represent the names and values
		/// of the constraints.
		/// </param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public static IRouteBuilder MapReusableRoute(
			this IRouteBuilder routeBuilder,
			string name,
			string template,
			object defaults,
			object constraints)
		{
			return MapReusableRoute(routeBuilder, name, template, defaults, constraints, dataTokens: null);
		}

		public static IRouteBuilder MapReusableRoute(
			this IRouteBuilder routeBuilder,
			string name,
			string template,
			object defaults,
			object constraints,
			object dataTokens)
		{
			if (routeBuilder.DefaultHandler == null)
			{
				throw new RouteCreationException($"A default handler must be set on the {nameof(IRouteBuilder)}.");
			}

			var inlineConstraintResolver = routeBuilder
				.ServiceProvider
				.GetRequiredService<IInlineConstraintResolver>();

			var options = routeBuilder.ServiceProvider.GetRequiredService<IOptions<ReusableMvcOptions>>();
			var actionProvider = routeBuilder.ServiceProvider.GetRequiredService<IActionDescriptorCollectionProvider>();
			var pipelineDecisionTree = routeBuilder.ServiceProvider.GetRequiredService<PipelineDecisionTree<ReusableMvcOptions>>();
			routeBuilder.Routes.Add(new Route(
				new ReusableRouteHandler(routeBuilder.DefaultHandler, actionProvider, options, pipelineDecisionTree),
				name,
				template,
				new RouteValueDictionary(defaults),
				new RouteValueDictionary(constraints),
				new RouteValueDictionary(dataTokens),
				inlineConstraintResolver));

			return routeBuilder;
		}
	}
}