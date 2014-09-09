using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Security.Cookies;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using NamespaceRouting.Models;
using Microsoft.AspNet.Mvc;

namespace NamespaceRouting
{
    public class Startup
    {
        public void Configure(IBuilder app)
        {
            // Set up application services
            app.UseServices(services =>
            {
                services.SetupOptions<MvcOptions>(options =>
                {
                    options.ApplicationModelConventions.Add(new NameSpaceRoutingConvention());
                });

                // Add MVC services to the services container
                services.AddMvc();
            });

            // Enable Browser Link support
            app.UseBrowserLink();

            // Add cookie-based authentication to the request pipeline
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = ClaimsIdentityOptions.DefaultAuthenticationType,
                LoginPath = new PathString("/Account/Login"),
            });

            // Add MVC to the request pipeline
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "api",
                    template: "{controller}/{id?}");
            });
        }
    }
}
