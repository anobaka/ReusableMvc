using System.Collections.Generic;
using LazyMortal.Multipipeline;
using Microsoft.AspNetCore.Http;

namespace LazyMortal.ReusableMvc.Options
{
	public class ReusableMvcOptions
	{
		/// <summary>
		/// used for populating default static files stored in <see cref="HttpContext.Item"/>, default value is "ViewName"
		/// </summary>
		public string ViewNameHttpContextItemKey { get; set; } = "ViewName";

		public string PipelineNameRouteDataKey { get; set; } = "area";
		public string DefaultViewLocation { get; set; } = "/Views/{1}/{0}.cshtml";
		public string DefaultLayoutLocation { get; set; } = "/Views/Shared/{0}.cshtml";
	}
}