// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace EntropyTests.ConfigTests
{
    public class ConfigWalkingValuesWebTests : E2ETestBase
    {
        public ConfigWalkingValuesWebTests(ITestOutputHelper output)
            : base(output, "Config.WalkingValues.Web")
        {
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            Assert.Contains("ValueOfKey-Data:Inventory:ConnectionString", responseText);
            Assert.Contains("ValueOfKey-Data:Inventory:Provider", responseText);
        }
    }
}

