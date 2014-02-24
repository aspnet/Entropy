using Microsoft.AspNet.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Builder.Filtering.Web
{
    public class Startup
    {
        public void Configuration(IBuilder app)
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
    }
}
