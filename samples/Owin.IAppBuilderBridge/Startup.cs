using Microsoft.AspNet.Builder;

namespace Owin.IAppBuilderBridge
{
    public class Startup
    {
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
    }
}
