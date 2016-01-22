using Microsoft.AspNetCore.Mvc;

namespace Diagnostics.StatusCodes.Mvc.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public string Index()
        {
            return "Hello World, try /bob to get a 404";
        }
    }
}
