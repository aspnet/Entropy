// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EntropyTests;
using Xunit;

namespace FunctionalTests.MvcTests
{
    public class GenericControllersTest : IClassFixture<SampleTestFixture<Mvc.GenericControllers.Startup>>
    {
        public GenericControllersTest(SampleTestFixture<Mvc.GenericControllers.Startup> fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task NonGenericController_SprocketControllerDefined_TakesPrecedence()
        {
            // Arrange & Act
            var response = await Client.GetAsync("http://localhost/Sprocket");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Hello from a non-generic SprocketController.", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task GenericController_NoWidgetControllerDefined_FallsBackToGeneric()
        {
            // Arrange & Act
            var response = await Client.GetAsync("http://localhost/Widget");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Hello from a generic Widget controller.", await response.Content.ReadAsStringAsync());
        }
    }
}
