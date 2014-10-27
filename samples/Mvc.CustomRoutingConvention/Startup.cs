using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;
using Microsoft.AspNet.Mvc;

namespace NamespaceRouting
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            // Set up application services
            app.UseServices(services =>
            {
                services.Configure<MvcOptions>(options =>
                {
                    options.ApplicationModelConventions.Add(new NameSpaceRoutingConvention());
                });

                // Add MVC services to the services container
                services.AddMvc();
            });

            // Add MVC to the request pipeline
            app.UseMvc();            
        }
    }
}
