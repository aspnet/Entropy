// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Entropy.FunctionalTests
{
    public class SampleTestFixture<TStartup> : IDisposable
    {
        private readonly TestServer _server;

        public SampleTestFixture()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(GetApplicationPath())
                .ConfigureServices(InitializeServices)
                .UseStartup(typeof(TStartup));

            _server = new TestServer(builder);

            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");
        }

        public HttpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }

        private static string GetApplicationPath()
        {
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;
            var applicationName = startupAssembly.GetName().Name;
            return TestServices.GetApplicationDirectory(applicationName);
        }

        private static void InitializeServices(IServiceCollection services)
        {
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;

            // Inject a custom application part manager. Overrides AddMvcCore() because that uses TryAdd().
            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(startupAssembly));

            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            manager.FeatureProviders.Add(new ViewComponentFeatureProvider());

            services.AddSingleton(manager);
        }
    }
}
