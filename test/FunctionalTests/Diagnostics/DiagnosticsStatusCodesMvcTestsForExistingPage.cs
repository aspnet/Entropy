using System.Net;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace EntropyTests.Diagnostics
{
    [Collection(SiteName)]
    public class DiagnosticsStatusCodesMvcTestsForExistingPage : E2ETestBase
    {
        private const string SiteName = "Diagnostics.StatusCodes.Mvc";

        public DiagnosticsStatusCodesMvcTestsForExistingPage(ITestOutputHelper output)
            : base(output, SiteName, 6100)
        {
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            var expectedText = "Hello World, try /bob to get a 404";
            Assert.Equal(expectedText, responseText);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
