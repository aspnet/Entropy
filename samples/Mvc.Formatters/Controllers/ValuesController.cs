using Microsoft.AspNetCore.Mvc;

namespace Mvc.Formatters.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpPost]
        public IActionResult Echo([FromBody]string value)
        {
            return Content(value);
        }
    }
}
