using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace EntropyTests.BuilderTests
{
    public class BuilderHelloWorldWebTests : E2ETestBase
    {
        public BuilderHelloWorldWebTests(ITestOutputHelper output)
            : base(output, "Builder.HelloWorld.Web", 5600)
        {
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            Assert.Equal("Hello world", responseText);
        }
    }
}
