// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;

namespace BuilderTests.MvcTests
{
    public class CustomRoutingConventionTests
    {
        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        public async Task CustomRouting_NameSpaceRouting()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .UseStartup(typeof(NamespaceRouting.Startup));
            var client = new TestServer(builder).CreateClient();

            // Act
            var result = await client.GetAsync("MySite/Inventory/Products/List");

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var contentString = await result.Content.ReadAsStringAsync();
            Assert.Contains("Hello from ProductsController", contentString);
        }
    }
}
