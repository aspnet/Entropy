using Microsoft.AspNet.Builder;

namespace Builder.HelloWorld.Web
{
    public class Startup
    {
        public void Configure(IBuilder app)
        {
            app.Run(async context =>
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Hello world");
            });
        }
    }
}
