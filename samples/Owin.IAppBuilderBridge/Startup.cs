using Microsoft.AspNet.Builder;

namespace Owin.IAppBuilderBridge
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseAppBuilder(appBuilder =>
            {
                appBuilder.SetDataProtectionProvider(app);
                appBuilder.Run(context =>
                {
                    return context.Response.WriteAsync("Hello from IAppBuilder middleware.");
                });
            });
        }
    }
}
