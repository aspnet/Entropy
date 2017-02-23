using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace EntropyTests.BuilderTests
{
    public class BuilderMiddlewareWebTests : E2ETestBase
    {
        public BuilderMiddlewareWebTests(ITestOutputHelper output) 
            : base(output, "Builder.Middleware.Web", 6700)
        {
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            Assert.Equal("Yo, middleware!\r\nThis request is a GET\r\n", responseText);
        }
    }
}
