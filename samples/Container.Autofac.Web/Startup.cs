using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Autofac;

namespace Container.Autofac.Web
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<CallOne>().As<ICall>().SingleInstance();
            containerBuilder.RegisterType<CallTwo>().As<ICall>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<CallThree>().As<ICall>().InstancePerDependency();

            AutofacRegistration.Populate(containerBuilder, Enumerable.Empty<IServiceDescriptor>(), app.ApplicationServices);
            var container = containerBuilder.Build();

            app.UseServices(container.Resolve<IServiceProvider>());

            app.UseMiddleware(typeof(MyMiddleware));
            app.UseMiddleware(typeof(MyMiddleware));

            app.Run(async context =>
                await context.Response.WriteAsync("---------- Done\r\n"));
        }
    }

    public class MyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<ICall> _calls;

        public MyMiddleware(RequestDelegate next, IEnumerable<ICall> calls)
        {
            _next = next;
            _calls = calls;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            await context.Response.WriteAsync("---------- MyMiddleware ctor\r\n");
            await context.Response.WriteAsync(_calls.Aggregate("", (a, b) => a + b.Text + "\r\n"));

            var requestContainerCalls = context.RequestServices.GetService<IEnumerable<ICall>>();
            await context.Response.WriteAsync("---------- context.RequestServices\r\n");
            await context.Response.WriteAsync(requestContainerCalls.Aggregate("", (a, b) => a + b.Text + "\r\n"));

            await _next(context);
        }
    }

    public interface ICall
    {
        string Text { get; }
    }

    public abstract class CallBase : ICall
    {
        private static int _lastNumber;
        private readonly int _number;

        public CallBase()
        {
            _number = Interlocked.Increment(ref _lastNumber);
        }

        public string Text
        {
            get { return GetType().Name + "[" + _number + "]"; }
        }
    }

    public class CallOne : CallBase
    {
    }

    public class CallTwo : CallBase
    {
    }

    public class CallThree : CallBase
    {
    }
}
