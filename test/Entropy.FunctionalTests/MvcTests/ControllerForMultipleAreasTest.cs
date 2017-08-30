// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;

namespace Entropy.FunctionalTests
{
    public class ControllerForMultipleAreasTest : IClassFixture<SampleTestFixture<ControllerForMultipleAreasSample.Startup>>
    {
        public ControllerForMultipleAreasTest(SampleTestFixture<ControllerForMultipleAreasSample.Startup> fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task ProductsArea()
        {
            // Arrange & Act
            var response = await Client.GetAsync("/Products/Home/Index");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Products View", content);
        }

        [Fact]
        public async Task ServicesArea()
        {
            // Arrange & Act
            var response = await Client.GetAsync("/Services/Home/Index");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Services View", content);
        }

        [Fact]
        public async Task ManageArea()
        {
            // Arrange & Act
            var response = await Client.GetAsync("/Manage/Home/Index");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Manage View", content);
        }
    }
}
