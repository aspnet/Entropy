using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder
{
    public static class RouteBuilderExtensions
    {
        public static void MapModuleRoute(this IRouteBuilder routes)
        {
            routes.MapRoute("moduleRoute", "module/{modulepath}", defaults: new { module = "true" });
        }
    }
}