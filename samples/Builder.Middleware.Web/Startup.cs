using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Builder.Middleware.Web
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseXHttpHeaderOverride();
            app.UseMiddleware(typeof(MyMiddleware), "Yo");
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseDefaultHostingConfiguration(args)
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }

    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseXHttpHeaderOverride(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware(typeof(XHttpHeaderOverrideMiddleware));
        }
    }

    public class XHttpHeaderOverrideMiddleware
    {
        private readonly RequestDelegate _next;

        public XHttpHeaderOverrideMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var headerValue = httpContext.Request.Headers["X-HTTP-Method-Override"];
            var queryValue = httpContext.Request.Query["X-HTTP-Method-Override"];

            if (!string.IsNullOrEmpty(headerValue))
            {
                httpContext.Request.Method = headerValue;
            }
            else if (!string.IsNullOrEmpty(queryValue))
            {
                httpContext.Request.Method = queryValue;
            }

            return _next.Invoke(httpContext);
        }
    }

    public class MyMiddleware
    {
        private RequestDelegate _next;
        private string _greeting;
        private IServiceProvider _services;

        public MyMiddleware(RequestDelegate next, string greeting, IServiceProvider services)
        {
            _next = next;
            _greeting = greeting;
            _services = services;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await httpContext.Response.WriteAsync(_greeting + ", middleware!\r\n");
            await httpContext.Response.WriteAsync("This request is a " + httpContext.Request.Method + "\r\n");
        }
    }
}

