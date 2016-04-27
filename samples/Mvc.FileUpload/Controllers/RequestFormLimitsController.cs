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

        // Set the request form size limits *before* the antiforgery token validation filter is executed so that the
        // limits are honored when the antiforgery validation filter tries to read the form. These form size limits
        // only apply to this action.
        [HttpPost]
        [RequestFormSizeLimit(keyCountLimit: 3, Order = 1)]
        [ValidateAntiForgeryToken(Order = 2)]
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
