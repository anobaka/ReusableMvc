using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Anobaka.ReusableMvc.StaticFiles
{
	public interface IStaticFilesFactory<out TStaticFiles>
	{
		TStaticFiles Resolve(IActionContextAccessor actionContextAccessor);
	}
}