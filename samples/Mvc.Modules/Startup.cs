using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;

namespace Mvc.Modules
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            var configuration = new Configuration();
            configuration.AddJsonFile("config.json");
            configuration.AddEnvironmentVariables();

            app.UseServices(services =>
            {
                services.AddMvc();
                services.AddModules();
            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapModuleRoute();

                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
