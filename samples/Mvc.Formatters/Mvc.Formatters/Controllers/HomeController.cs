using Microsoft.AspNetCore.Mvc;

namespace Mvc.Formatters.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
