using System.Threading;
using Autofac;
using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.DependencyInjection.Autofac;
using Microsoft.AspNet.RequestContainer;

namespace Container.Autofac.Web
{
    public class Startup
    {
        public void Configuration(IBuilder app)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<CallOne>().As<ICall>().SingleInstance();
            containerBuilder.RegisterType<CallTwo>().As<ICall>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<CallThree>().As<ICall>().InstancePerDependency();

            // TODO - overflow from host services
            containerBuilder.RegisterGeneric(typeof(ContextAccessor<>)).As(typeof(IContextAccessor<>)).InstancePerLifetimeScope();
            containerBuilder.RegisterType<TypeActivator>().As<ITypeActivator>().InstancePerDependency();

            AutofacRegistration.Populate(containerBuilder, app.ServiceProvider, Enumerable.Empty<IServiceDescriptor>());
            var container = containerBuilder.Build();

            app.UseContainer(container.Resolve<IServiceProvider>());

            app.UseMiddleware(typeof(MyMiddleware));
            app.UseMiddleware(typeof(MyMiddleware));

            app.Run(async context => context.Response.WriteAsync("---------- Done\r\n"));
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
