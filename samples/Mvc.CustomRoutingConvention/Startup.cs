using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;

namespace NamespaceRouting
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC services to the services container
            services.AddMvc(options =>
            {
                options.Conventions.Add(new NameSpaceRoutingConvention());
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            // Add MVC to the request pipeline
            app.UseMvc();            
        }
    }
}
