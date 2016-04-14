using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Builder.Filtering.Web
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Use(next => context => FilterAsync(context, next));

            app.Run(HelloWorldAsync);
        }

        public async Task FilterAsync(HttpContext context, RequestDelegate next)
        {
            var body = context.Response.Body;
            var buffer = new MemoryStream();
            context.Response.Body = buffer;

            try
            {
                context.Response.Headers["CustomHeader"] = "My Header";

                await context.Response.WriteAsync("Before\r\n");
                await next(context);
                await context.Response.WriteAsync("After\r\n");

                buffer.Position = 0;
                await buffer.CopyToAsync(body);
            }
            finally
            {
                context.Response.Body = body;
            }
        }

        public async Task HelloWorldAsync(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("Hello world\r\n");
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                // We set the server by name before default args so that command line arguments can override it.
                // This is used to allow deployers to choose the server for testing.
                .UseServer("Microsoft.AspNetCore.Server.Kestrel")
                .UseDefaultHostingConfiguration(args)
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}

