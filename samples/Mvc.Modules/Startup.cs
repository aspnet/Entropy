using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;

namespace Mvc.Modules
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddModules();
        }

        public void Configure(IApplicationBuilder app)
        {
            var configuration = new ConfigurationSection();
            configuration.AddJsonFile("config.json");
            configuration.AddEnvironmentVariables();

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
