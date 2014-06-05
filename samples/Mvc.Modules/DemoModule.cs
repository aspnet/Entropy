
namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class DemoModule : MvcModule
    {
        public DemoModule()
        {
            Get["/foo"] = () => "Hello, world!";
        }
    }
}