using System.Linq;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace EntropyTests.BuilderTests
{
    public class BuilderFilteringWebTests : E2ETestBase
    {
        public BuilderFilteringWebTests(ITestOutputHelper output)
            : base(output, "Builder.Filtering.Web", 5500)
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
