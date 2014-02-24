using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Middleware.Web
{
    public class Startup
    {
        public void Configuration(IBuilder app)
        {
            app.Use(typeof(MyMiddleware), "Yo");
        }
    }

    public static class BuilderExtensions
    {
        public static IBuilder Use(this IBuilder builder, Type middleware, params object[] args)
        {
            return builder.Use(next =>
            {
                var typeActivator = builder.ServiceProvider.GetService<ITypeActivator>();
                var instance = typeActivator.CreateInstance(middleware, new[] { next }.Concat(args).ToArray());
                return (RequestDelegate)Delegate.CreateDelegate(typeof(RequestDelegate), instance, "Invoke");
            });
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

        public async Task Invoke(HttpContext context)
        {
            await context.Response.WriteAsync(_greeting + ", middleware!");
        }
    }
}
