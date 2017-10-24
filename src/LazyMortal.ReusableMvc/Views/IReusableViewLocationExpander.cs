using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace LazyMortal.ReusableMvc.Views
{
	/// <inheritdoc />
	/// <summary>
	/// You can replace the default view location expander by overriding the injected implementation like services.AddSingleton&lt;IReusableViewLocationExpander, MyImplementation&gt;
	/// </summary>
	public interface IReusableViewLocationExpander : IViewLocationExpander
	{
	}
}