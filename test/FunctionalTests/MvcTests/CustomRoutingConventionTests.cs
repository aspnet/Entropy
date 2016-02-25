// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace BuilderTests.MvcTests
{
    public class CustomRoutingConventionTests
    {
        [Fact]
        public async Task CustomRouting_NameSpaceRouting()
        {
            // Arrange
            var applicationEnvironment = new TestApplicationEnvironment();
            var builder = new WebHostBuilder()
                .UseStartup(typeof(NamespaceRouting.Startup))
                .ConfigureServices(services => services.AddSingleton<IApplicationEnvironment, TestApplicationEnvironment>());
            var client = new TestServer(builder).CreateClient();

            // Act
            var result = await client.GetAsync("MySite/Inventory/Products/List");

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var contentString = await result.Content.ReadAsStringAsync();
            Assert.Contains("Hello from ProductsController", contentString);
        }

        private class TestApplicationEnvironment : IApplicationEnvironment
        {
            private static readonly Assembly TestAssembly = typeof(NamespaceRouting.Startup).GetTypeInfo().Assembly;

            public string ApplicationBasePath { get; } = TestAssembly.Location;

            public string ApplicationName { get; set; } = TestAssembly.GetName().Name;

            public string ApplicationVersion { get; set; }

            public FrameworkName RuntimeFramework { get; set; }
        }
    }
}
