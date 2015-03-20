using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;

namespace RazorPre
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}
