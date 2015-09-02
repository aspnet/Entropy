using System.Linq;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.ModuleFramework;

namespace Mvc.Modules
{
    public class NameFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var names = context.HttpContext.Request.Query["name"];
            if (names != null && names.Any())
            {
                var module = (MvcModule)context.Controller;
                module.ViewData.Add("name", names.First());
            }
        }
    }
}