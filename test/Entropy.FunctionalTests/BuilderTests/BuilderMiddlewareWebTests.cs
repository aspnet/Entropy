// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace EntropyTests.BuilderTests
{
    public class BuilderMiddlewareWebTests : E2ETestBase
    {
        public BuilderMiddlewareWebTests(ITestOutputHelper output) 
            : base(output, "Builder.Middleware.Web")
        {
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            Assert.Equal("Yo, middleware!\r\nThis request is a GET\r\n", responseText);
        }
    }
}
