// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace EntropyTests.Diagnostics
{
    [Collection(SiteName)]
    public class DiagnosticsStatusCodesMvcTestsForExistingPage : E2ETestBase
    {
        private const string SiteName = "Diagnostics.StatusCodes.Mvc";

        public DiagnosticsStatusCodesMvcTestsForExistingPage(ITestOutputHelper output)
            : base(output, SiteName)
        {
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            var expectedText = "Hello World, try /bob to get a 404";
            Assert.Equal(expectedText, responseText);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
