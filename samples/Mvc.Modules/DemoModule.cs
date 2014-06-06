using Microsoft.AspNet.Mvc.ModuleFramework;

namespace Mvc.Modules
{
    [NameFilter]
    public class DemoModule : MvcModule
    {
        public DemoModule()
        {
            Get["/foo"] = () => string.Format("Hello, {0}!", ViewData["name"] ?? "World");
        }
    }
}