using Microsoft.AspNetCore.Mvc;

namespace Sample.Web.Areas.A.Controllers
{
    [Area("a")]
    [Route("a/[controller]")]
    public class HomeController : Controller
    {
        [HttpGet("bbb")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
