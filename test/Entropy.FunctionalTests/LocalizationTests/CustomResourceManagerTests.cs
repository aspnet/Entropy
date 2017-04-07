// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Entropy.FunctionalTests.LocalizationTests
{
    public class CustomResourceManagerTests : E2ETestBase
    {
        public CustomResourceManagerTests(ITestOutputHelper output)
            : base(output, "Localization.CustomResourceManager")
        {
        }

        protected override Task<HttpResponseMessage> GetResponse(HttpClient client)
        {
            return client.GetAsync("?culture=fr-FR&ui-culture=fr-FR");
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            Assert.Contains("<h1>ClassLib Bonjour from greeting service</h1>", responseText);
            Assert.Contains("<h1>Yelling (Outsidenamespace): BONJOUR OUTSIDE</h1>", responseText);
        }
    }
}
