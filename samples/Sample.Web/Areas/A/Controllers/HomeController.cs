using Microsoft.AspNetCore.Mvc;

namespace Sample.Web.Areas.A.Controllers
{
	[Area("a")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
