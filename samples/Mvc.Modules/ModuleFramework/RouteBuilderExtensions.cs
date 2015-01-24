using Microsoft.AspNet.Routing;

namespace Microsoft.AspNet.Builder
{
    public static class RouteBuilderExtensions
    {
        public static void MapModuleRoute(this IRouteBuilder routes)
        {
            routes.MapRoute("moduleRoute", "module/{modulepath}", defaults: new { module = "true" });
        }
    }
}