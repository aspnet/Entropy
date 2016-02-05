// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.PlatformAbstractions;

namespace Mvc.RenderViewToString
{
    public class Program
    {
        public static void Main()
        {
            // Initialize the necessary services
            var services = new ServiceCollection();
            ConfigureDefaultServices(services, customApplicationBasePath: null);

            // Add a custom service that is used in the view.
            services.AddSingleton<EmailReportGenerator>();

            var provider = services.BuildServiceProvider();
            var helper = provider.GetRequiredService<RazorViewToStringRenderer>();

            var model = new EmailViewModel
            {
                UserName = "User",
                SenderName = "Sender",
                UserData1 = 1,
                UserData2 = 2
            };
            var emailContent = helper.RenderViewToString("EmailTemplate", model);
            Console.WriteLine(emailContent);
            Console.ReadLine();
        }

        private static void ConfigureDefaultServices(IServiceCollection services, string customApplicationBasePath)
        {
            var applicationEnvironment = PlatformServices.Default.Application;
            services.AddSingleton(applicationEnvironment);
            services.AddSingleton<IHostingEnvironment>(new HostingEnvironment
            {
                WebRootFileProvider = new PhysicalFileProvider(customApplicationBasePath ?? applicationEnvironment.ApplicationBasePath)
            });
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(new PhysicalFileProvider(customApplicationBasePath ?? applicationEnvironment.ApplicationBasePath));
            });
            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddLogging();
            services.AddMvc();
            services.AddSingleton<RazorViewToStringRenderer>();
        }
    }
}
