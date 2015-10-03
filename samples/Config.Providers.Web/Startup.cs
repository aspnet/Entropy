using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        var builder = new ConfigurationBuilder();
        builder.AddIniFile("Config.Providers.ini");
        builder.AddEnvironmentVariables();
        var config = builder.Build();

        app.Run(async ctx =>
        {
            ctx.Response.ContentType = "text/plain";

            Func<String, String> formatKeyValue = key => "[" + key + "] " + config[key] + "\r\n\r\n";
            await ctx.Response.WriteAsync(formatKeyValue("Services:One.Two"));
            await ctx.Response.WriteAsync(formatKeyValue("Services:One.Two:Six"));
            await ctx.Response.WriteAsync(formatKeyValue("Data:DefaultConnecection:ConnectionString"));
            await ctx.Response.WriteAsync(formatKeyValue("Data:DefaultConnecection:Provider"));
            await ctx.Response.WriteAsync(formatKeyValue("Data:Inventory:ConnectionString"));
            await ctx.Response.WriteAsync(formatKeyValue("Data:Inventory:Provider"));
            await ctx.Response.WriteAsync(formatKeyValue("PATH"));
            await ctx.Response.WriteAsync(formatKeyValue("COMPUTERNAME"));
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
}
