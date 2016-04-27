using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Owin.IAppBuilderBridge
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAppBuilder(appBuilder =>
            {
                // Some components will have dependencies that you need to populate in the IAppBuilder.Properties.
                // Here's one example that maps the data protection infrastructure.
                appBuilder.SetDataProtectionProvider(app);

                appBuilder.Run(context =>
                {
                    return context.Response.WriteAsync("Hello from IAppBuilder middleware.");
                });
            });
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}

