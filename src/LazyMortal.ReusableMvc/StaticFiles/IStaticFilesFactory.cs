using LazyMortal.ReusableMvc.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LazyMortal.ReusableMvc.StaticFiles
{
	/// <summary>
	/// You can replace the <see cref="DefaultStaticFilesFactory"/> and <see cref="DefaultStaticFiles"/> by overriding the injected implementation like services.AddServices&lt;IStaticFilesFactory, MyStaticFileFactory> or use <see cref="ReusableMvcServiceCollectionExtensions.AddReusableMvc&lt;TStaticFiles, TStaticFilesFactory&gt;"/>
	/// </summary>
	/// <typeparam name="TStaticFiles"></typeparam>
	public interface IStaticFilesFactory<out TStaticFiles>
	{
		/// <summary>
		/// <para>to find all files' location in <see cref="TStaticFiles"/></para>
		/// <para>todo: use a parameter to reduce the cost of AsyncLocal</para>
		/// </summary>
		/// <returns></returns>
		TStaticFiles Resolve();
	}
}