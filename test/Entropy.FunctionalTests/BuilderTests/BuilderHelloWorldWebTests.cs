// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace Entropy.FunctionalTests.BuilderTests
{
    public class BuilderHelloWorldWebTests : E2ETestBase
    {
        public BuilderHelloWorldWebTests(ITestOutputHelper output)
            : base(output, "Builder.HelloWorld.Web")
        {
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            Assert.Equal("Hello world", responseText);
        }
    }
}
