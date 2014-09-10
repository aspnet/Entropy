using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace Builder.HelloWorld.Web
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Hello world");
            });
        }
    }
}
