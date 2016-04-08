using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

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
            .UseDefaultHostingConfiguration(args)
            .UseServer("Owin.Nowin.HelloWorld")
            .UseIISIntegration()
            .UseStartup<Startup>()
            .Build();

        host.Run();
    }
}
