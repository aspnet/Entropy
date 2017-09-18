using Microsoft.AspNetCore.Mvc;
using Mvc.FileUpload.Filters;
using Mvc.FileUpload.Models;

namespace Mvc.FileUpload.Controllers
{
    public class RequestFormLimitsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [RequestFormLimits(ValueCountLimit = 3)]
        [ValidateAntiForgeryToken]
        public IActionResult ActionSpecificLimits(User user)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = new
            {
                Name = user.Name,
                Age = user.Age,
                Zipcode = user.Zipcode
            };

            return Json(result);
        }

        // This uses application wide request form size limits
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplicationWideLimits(User user)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = new
            {
                Name = user.Name,
                Age = user.Age,
                Zipcode = user.Zipcode
            };

            return Json(result);
        }
    }
}
