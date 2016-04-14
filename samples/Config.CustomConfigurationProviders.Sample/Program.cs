// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;

namespace Config.CustomConfigurationProviders.Web
{
    public class Program
    {
        /// <summary>
        /// Custom ConfigurationProvider demo program entry point
        /// </summary>
        public static void Main()
        {
            var builder = new ConfigurationBuilder()
                .AddConfigFile("Web.config");
            var configuration = builder.Build();

            Console.WriteLine("---------- AppSettings ----------");
            Console.WriteLine($"PreserveLoginUrl: [{configuration.GetAppSetting("PreserveLoginUrl")}]");
            Console.WriteLine($"ClientValidationEnabled: [{configuration.GetAppSetting("ClientValidationEnabled")}]");
            Console.WriteLine($"UnobtrusiveJavaScriptEnabled: [{configuration.GetAppSetting("UnobtrusiveJavaScriptEnabled")}]");

            Console.WriteLine("---------- ConnectionStrings ----------");
            foreach (var kvp in configuration.GetSection("ConnectionStrings").GetChildren())
            {
                Console.WriteLine($"{kvp.Key}: [{kvp.Value}]");
            }

            Console.WriteLine("---------- configNode:nestedNode ---------- ");
            foreach (var kvp in configuration.GetSection("configNode", "nestedNode"))
            {
                Console.WriteLine($"{kvp.Key}: [{kvp.Value.Value}]");
            }

            Console.WriteLine("---------- Specific Key ---------- ");
            var value = configuration.GetValue("sampleSection", "setting2");
            Console.WriteLine($"KEY: sampleSection:setting2, VALUE: {value}");

            Console.WriteLine("Press ENTER to exit program...");
            Console.ReadLine();
        }
    }
}
