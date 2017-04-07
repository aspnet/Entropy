// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace Entropy.FunctionalTests.BuilderTests
{
    public class BuilderFilteringWebTests : E2ETestBase
    {
        public BuilderFilteringWebTests(ITestOutputHelper output)
            : base(output, "Builder.Filtering.Web")
        {
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            Assert.Equal("Before\r\nHello world\r\nAfter\r\n", responseText);

            var customHeaderText = response.Headers.GetValues("CustomHeader").Single();
            Assert.Equal("My Header", customHeaderText);
        }
    }
}
