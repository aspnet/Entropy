// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace Entropy.FunctionalTests.ConfigTests
{
    public class ConfigSettingObjectWebTests : E2ETestBase
    {
        public ConfigSettingObjectWebTests(ITestOutputHelper output)
            : base(output, "Config.SettingObject.Web")
        {
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            var expectedText =
"Retry Count 42\r\n" +
"Default Ad Block House\r\n" +
"Ad Block Contoso Origin sql-789 Product Code contoso2014\r\n" +
"Ad Block House Origin blob-456 Product Code 123\r\n";

            Assert.Equal(expectedText, responseText);
        }
    }
}
