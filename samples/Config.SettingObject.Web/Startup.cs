// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
// WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF
// TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR
// NON-INFRINGEMENT.
// See the Apache 2 License for the specific language governing
// permissions and limitations under the License.

using Config.SettingObject.Web;
using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.ConfigurationModel;
using Microsoft.AspNet.ConfigurationModel.Sources;

public class Startup
{
    public void Configuration(IBuilder app)
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
