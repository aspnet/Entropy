using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics.Elm;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace ElmSampleApp
{
    /// <summary>
    /// Simple page that displays "Hello World". Navigate to localhost/foo to see the elm logs
    /// </summary>
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddSingleton<ElmStore>(); // add the store so the ElmLogger can write to it
            services.AddElm(options =>
            {
                options.Path = new PathString("/foo");  // defaults to "/Elm"
                options.Filter = (name, level) => level >= LogLevel.Information;
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory factory)
        {
            app.UseElmPage(); // Shows the logs at the specified path
            app.UseElmCapture(); // Adds the ElmLoggerProvider

            var logger = factory.CreateLogger<Startup>();
            using (logger.BeginScope("startup"))
            {
                logger.LogWarning("Starting up");
            }

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello world");
                using (logger.BeginScope("world"))
                {
                    logger.LogInformation("Hello world!");
                    logger.LogError("Mort");
                }
                // This will not get logged because the filter has been set to LogLevel.Information and above
                using (logger.BeginScope("verbose"))
                {
                    logger.LogVerbose("some verbose stuff");
                }
            });
            logger.LogInformation("This message is not in a scope");
        }
    }
}
