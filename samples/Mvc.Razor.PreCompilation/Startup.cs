using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;

namespace RazorPre
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseServices(services =>
            {
                services.AddMvc();
            });

            app.UseMvc();
        }
    }
}
