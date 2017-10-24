using System.Collections.Generic;
using LazyMortal.Multipipeline;
using Microsoft.AspNetCore.Http;

namespace LazyMortal.ReusableMvc.Options
{
	public class ReusableMvcOptions
	{
		/// <summary>
		/// The key in <see cref="HttpContext.Items"/> is used for transporting current viewname for finding default static files in , default value is "ViewName"
		/// </summary>
		public string ViewNameHttpContextItemKey { get; set; } = "ViewName";

		/// <summary>
		/// The default template key for route to generating urls.
		/// </summary>
		public string PipelineNameRouteDataKey { get; set; } = "area";

		/// <summary>
		/// The default view location template is: "/Views/{1}/{0}.cshtml"
		/// </summary>
		public string DefaultViewLocation { get; set; } = "/Views/{1}/{0}.cshtml";

		/// <summary>
		/// The default layout view location template is: "/Views/Shared/{0}.cshtml"
		/// </summary>
		public string DefaultLayoutLocation { get; set; } = "/Views/Shared/{0}.cshtml";
        /// <summary>
        /// The default static file location template is: "{1}/{0}.cshtml"
        /// </summary>
	    public string DefaultStaticFileLocation { get; set; } = "{1}/{0}";
	}
}