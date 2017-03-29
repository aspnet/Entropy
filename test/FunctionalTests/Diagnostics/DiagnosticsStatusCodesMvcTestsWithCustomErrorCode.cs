// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EntropyTests.Diagnostics
{
    [Collection(SiteName)]
    public class DiagnosticsStatusCodesMvcTestsWithCustomErrorCode : E2ETestBase
    {
        private const string SiteName = "Diagnostics.StatusCodes.Mvc";

        public DiagnosticsStatusCodesMvcTestsWithCustomErrorCode(ITestOutputHelper output)
            : base(output, SiteName)
        {
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            Assert.Equal(HttpStatusCode.PaymentRequired, response.StatusCode);
        }

        protected override Task<HttpResponseMessage> GetResponse(HttpClient client)
        {
            return client.GetAsync("/errors/" + (int)HttpStatusCode.PaymentRequired);
        }
    }
}
