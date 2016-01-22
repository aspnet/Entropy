using Microsoft.AspNetCore.Mvc;

namespace Diagnostics.StatusCodes.Mvc.Controllers
{
    public class ErrorsController : Controller
    {
        // This controller is called to generate response bodies for 400-599 status codes from
        // other locations in the app.
        [Route("errors/{id:int}")]
        public IActionResult Index(int id)
        {
            return new ObjectResult("Error page, status code: " + id) { StatusCode = id };
        }
    }
}