namespace Anobaka.ReusableMvc.Options
{
	public class ReusablePipelineOptions
	{
		/// <summary>
		/// used for finding current pipeline's controller for current request.
		/// <para>{0} is for project's base namespace</para>
		/// <para>{1} is for pipeline's name</para>
		/// <para>{2} is for controller's name</para>
		/// <para>default value is "{0}.Areas.{1}.Controllers.{2}"</para>
		/// </summary>
		public string ControllerFullNameTemplate { get; set; } = "{0}.Areas.{1}.Controllers.{2}";

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
		/// <para>{2} is for pipeline's name</para>
		/// <para>default value is "{1}/{2}/{0}", the "//" will be replaced by "/" if the value is null</para>
		/// </summary>
		public string StaticFilesLocationTemplate { get; set; } = "{1}/{2}/{0}";
	}
}