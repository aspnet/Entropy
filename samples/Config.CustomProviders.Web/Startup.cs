using System.Threading.Tasks;
using Config.CustomProvider.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        var builder = new ConfigurationBuilder();
        builder.Add(new MyConfigProvider());
        var config = builder.Build();

        app.Run(async ctx =>
        {
            ctx.Response.ContentType = "text/plain";
            await DumpConfig(ctx.Response, config);
        });
    }

    public static void Main(string[] args)
    {
        var host = new WebHostBuilder()
            .UseDefaultConfiguration(args)
            .UseServer("Microsoft.AspNetCore.Server.Kestrel")
            .UseStartup<Startup>()
            .Build();

        host.Run();
    }

    private static async Task DumpConfig(HttpResponse response, IConfiguration config, string indentation = "")
    {
        foreach (var child in config.GetChildren())
        {
            await response.WriteAsync(indentation + "[" + child.Key + "] " + config[child.Key] + "\r\n");
            await DumpConfig(response, child, indentation + "  ");
        }
    }
}
