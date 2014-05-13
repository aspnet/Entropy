using Config.SettingObject.Web;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.ConfigurationModel;

public class Startup
{
    public void Configure(IBuilder app)
    {
        var config = new Configuration
        {
            new MemoryConfigurationSource
            {
                {"MySettings:RetryCount", "42"},
                {"MySettings:DefaultAdBlock", "House"},
                {"MySettings:AdBlock:House:ProductCode", "123"},
                {"MySettings:AdBlock:House:Origin", "blob-456"},
                {"MySettings:AdBlock:Contoso:ProductCode", "contoso2014"},
                {"MySettings:AdBlock:Contoso:Origin", "sql-789"},
            }
        };

        var mySettings = new MySettings();
        mySettings.Read(config.GetSubKey("MySettings"));

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
}
