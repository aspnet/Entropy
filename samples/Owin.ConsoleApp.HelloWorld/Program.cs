using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nowin;

namespace Owin.ConsoleApp.HelloWorld
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    using CreateMiddleware = Func<
          Func<IDictionary<string, object>, Task>,
          Func<IDictionary<string, object>, Task>
        >;
    using AddMiddleware = Action<Func<
          Func<IDictionary<string, object>, Task>,
          Func<IDictionary<string, object>, Task>
        >>;

    public class Program
    {
#pragma warning disable CS1998
        static AppFunc notFound = async env => env["owin.ResponseStatusCode"] = 404;
#pragma warning restore CS1998

        public static void Main(string[] args)
        {
            // List.Add is same signature as AddMiddleware
            IList<CreateMiddleware> list = new List<CreateMiddleware>();
            Configure(list.Add);

            // Now chain middleware together in reverse order
            AppFunc app = list.Reverse().Aggregate(notFound, (next, middleware) => middleware(next));

            var server = ServerBuilder.New().SetAddress(IPAddress.Any).SetPort(5000).SetOwinApp(app);
            using (server.Start())
            {
                Console.WriteLine("Listening on port 5000. Enter to exit.");
                Console.ReadLine();
            }
        }

        public static void Configure(AddMiddleware build)
        {
            var services = new ServiceCollection();
            services.AddSingleton(new DiagnosticLogger("Owin Diagnostic Logger: "));

            // adding vNext component in OWIN pipline
            build.UseBuilder(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.RequestServices.GetService<DiagnosticLogger>().Log("Returning Hello World");
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Hello World!");
                });
            }, services.BuildServiceProvider());
        }

        public class DiagnosticLogger
        {
            private readonly string _prefix;

            public DiagnosticLogger(string prefix)
            {
                _prefix = prefix;
            }

            public void Log(string message)
            {
                Console.WriteLine(_prefix + message);
            }
        }
    }
}
