using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.Run(context =>
        {
            context.Response.ContentType = "text/plain";
            return context.Response.WriteAsync("Hello World");
        });
    }

    public static void Main(string[] args)
    {
        var host = new WebHostBuilder()
            .UseDefaultConfiguration(args)
            .UseStartup<Startup>()
            .Build();

        host.Run();
    }
}
