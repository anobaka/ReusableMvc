using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LazyMortal.ReusableMvc.StaticFiles
{
	public interface IStaticFilesFactory<out TStaticFiles>
	{
		/// <summary>
		/// to find all files' location in <see cref="TStaticFiles"/>
		/// todo: use a parameter to reduce the cost of AsyncLocal
		/// </summary>
		/// <returns></returns>
		TStaticFiles Resolve();
	}
}