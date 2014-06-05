namespace Microsoft.AspNet.Routing
{
    public static class RouteBuilderExtensions
    {
        public static void MapModuleRoute(this IRouteBuilder routes)
        {
            routes.MapRoute("moduleRoute", "module/{modulepath}", defaults: new { module = "true" });
        }
    }
}