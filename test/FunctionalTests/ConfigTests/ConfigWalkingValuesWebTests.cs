using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace EntropyTests.ConfigTests
{
    public class ConfigWalkingValuesWebTests : E2ETestBase
    {
        public ConfigWalkingValuesWebTests(ITestOutputHelper output)
            : base(output, "Config.WalkingValues.Web", 5900)
        {
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            Assert.Contains("ValueOfKey-Data:Inventory:ConnectionString", responseText);
            Assert.Contains("ValueOfKey-Data:Inventory:Provider", responseText);
        }
    }
}

