using Microsoft.AspNetCore.Mvc;

namespace Sample.Web.Areas.A.Controllers
{
    [Area("a")]
    [Route("[area]/[controller]")]
    public class HomeController : Controller
    {
        [HttpGet("bbb")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
