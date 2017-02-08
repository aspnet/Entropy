// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EntropyTests;
using Xunit;

namespace FunctionalTests.MvcTests
{
    public class MvcAjaxAntiforgeryAuthWebTest : IClassFixture<SampleTestFixture<Antiforgery.MvcWithAuthAndAjax.Startup>>
    {
        public MvcAjaxAntiforgeryAuthWebTest(SampleTestFixture<Antiforgery.MvcWithAuthAndAjax.Startup> fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task AuthenticationRunBeforeAntiforgery()
        {
            // Arrange & Act
            var response = await Client.PostAsync("/Home/Antiforgery", content: null);

            // Assert
            // Authentication should run before Antiforgery
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("/Account/Login", response.Headers.Location.PathAndQuery);
        }
    }
}
