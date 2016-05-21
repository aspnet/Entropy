using System.Collections.Generic;
using System.Linq;
using Config.SettingObject.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        var builder = new ConfigurationBuilder();
        List<KeyValuePair<string, string>> data = new Dictionary<string, string>
        {
            { "MySettings:RetryCount", "42"},
            { "MySettings:DefaultAdBlock", "House"},
            { "MySettings:AdBlock:House:ProductCode", "123"},
            { "MySettings:AdBlock:House:Origin", "blob-456"},
            { "MySettings:AdBlock:Contoso:ProductCode", "contoso2014"},
            { "MySettings:AdBlock:Contoso:Origin", "sql-789"},
        }.ToList();
        builder.Add(new MemoryConfigurationSource { InitialData = data });
        var config = builder.Build();

        var mySettings = new MySettings();
        mySettings.Read(config.GetSection("MySettings"));

        app.Run(async ctx =>
        {
            ctx.Response.ContentType = "text/plain";

            await ctx.Response.WriteAsync(string.Format("Retry Count {0}\r\n", mySettings.RetryCount));
            await ctx.Response.WriteAsync(string.Format("Default Ad Block {0}\r\n", mySettings.DefaultAdBlock));
            foreach (var adBlock in mySettings.AdBlocks.Values)
            {
                await ctx.Response.WriteAsync(string.Format(
                    "Ad Block {0} Origin {1} Product Code {2}\r\n", 
                    adBlock.Name, adBlock.Origin, adBlock.ProductCode));                
            }
        });
    }

    public static void Main(string[] args)
    {
        var config = new ConfigurationBuilder().AddCommandLine(args).Build();
        
        var host = new WebHostBuilder()
            // We set the server by name before default args so that command line arguments can override it.
            // This is used to allow deployers to choose the server for testing.
            .UseKestrel()
            .UseConfiguration(config)
            .UseIISIntegration()
            .UseStartup<Startup>()
            .Build();

        host.Run();
    }
}

