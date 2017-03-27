using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyMortal.Multipipeline;
using LazyMortal.ReusableMvc.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace LazyMortal.ReusableMvc.Pipelines
{
	public abstract class ReusablePipeline : IPipeline
	{
		/// <inheritDoc/>
		public abstract string Name { get; set; }
		/// <inheritDoc/>
		public abstract Task<bool> ResolveAsync(HttpContext ctx);
		/// <inheritDoc/>
		public abstract Task ConfigurePipeline(IApplicationBuilder app);

		/// <summary>
		/// used for finding current pipeline's controller for current request.
		/// <para>{0} is for project's base namespace</para>
		/// <para>{1} is for controller's name trimed end with 'Controller'</para>
		/// <para>{2} is for pipeline's name</para>
		/// <para>default value is "{0}.Areas.{1}.Controllers.{2}"</para>
		/// </summary>
		public string ControllerFullNameTemplate { get; set; } = "{0}.Areas.{2}.Controllers.{1}Controller";

		/// <summary>
		/// used for finding current pipeline's view for current request
		/// <para>{0} is for view's name</para>
		/// <para>{1} is for controller's name</para>
		/// <para>{2} is for pipeline's name</para>
		/// <para>default value is "/Views/{1}{2}{0}.cshtml", the "//" will be replaced by "/" if the value is null</para>
		/// </summary>
		public string ViewLocationTemplate { get; set; } = "/Views/{1}/{2}/{0}.cshtml";

		/// <summary>
		/// used for finding current pipeline'sstatic files for current request
		/// <para>{0} is for view's name</para>
		/// <para>{1} is for controller's name</para>
		/// <para>{2} is for pipeline's name.ToLower()</para>
		/// <para>default value is "{1}/{2}/{0}", the "//" will be replaced by "/" if the value is null</para>
		/// </summary>
		public string StaticFilesLocationTemplate { get; set; } = "{1}/{2}/{0}";

		/// <summary>
		/// used for finding layout of this pipeline
		/// <para>{0} is for view's name</para>
		/// <para>{1} is for pipeline's name</para>
		/// </summary>
		public string SharedViewLocationTemplate { get; set; } = "/Views/Shared/{1}/{0}.cshtml";
	}
}