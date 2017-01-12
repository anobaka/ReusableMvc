using System.Collections.Generic;
using Anobaka.Multipipeline;
using Microsoft.AspNetCore.Http;

namespace Anobaka.ReusableMvc.Options
{
	public class ReusableMvcOptions : MultipipelineOptions
	{
		/// <summary>
		/// used for populating default static files stored in <see cref="HttpContext.Item"/>, default value is "ViewName"
		/// </summary>
		public string ViewNameHttpContextItemKey { get; set; } = "ViewName";
		/// <summary>
		/// must be set, see <see cref="PipelineOptions"/>
		/// </summary>
		public string ProjectBaseNameSpace { get; set; }
		/// <summary>
		/// used for finding all possible controllers for one request, default value is "{0}.Areas.{1}.Controllers"
		/// </summary>
		public IDictionary<IPipeline, ReusablePipelineOptions> PipelineOptions { get; set; }
	}
}