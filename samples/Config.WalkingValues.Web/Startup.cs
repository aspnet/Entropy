using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        var builder = new ConfigurationBuilder();
        builder.AddIniFile("Config.Sources.ini");
        builder.AddEnvironmentVariables();
        var config = builder.Build();

        app.Run(async ctx =>
        {
            ctx.Response.ContentType = "text/plain";
            await DumpConfig(ctx.Response, config);
        });
    }

    private static async Task DumpConfig(HttpResponse response, IConfiguration config, string indentation = "")
    {
        foreach (var child in config.GetChildren())
        {
            await response.WriteAsync(indentation + "[" + child.Key + "] " + config[child.Key] + "\r\n");
            await DumpConfig(response, child, indentation + "  ");
        }
    }

    public static void Main(string[] args)
    {
        var host = new WebHostBuilder()
            .UseDefaultHostingConfiguration(args)
            .UseServer("Microsoft.AspNetCore.Server.Kestrel")
            .UseStartup<Startup>()
            .Build();

        host.Run();
    }
}
