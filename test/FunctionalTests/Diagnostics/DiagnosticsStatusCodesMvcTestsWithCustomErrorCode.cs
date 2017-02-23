using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EntropyTests.Diagnostics
{
    [Collection(SiteName)]
    public class DiagnosticsStatusCodesMvcTestsWithCustomErrorCode : E2ETestBase
    {
        private const string SiteName = "Diagnostics.StatusCodes.Mvc";

        public DiagnosticsStatusCodesMvcTestsWithCustomErrorCode(ITestOutputHelper output)
            : base(output, SiteName, 6300)
        {
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            Assert.Equal(HttpStatusCode.PaymentRequired, response.StatusCode);
        }

        protected override Task<HttpResponseMessage> GetResponse(HttpClient client)
        {
            return client.GetAsync("/errors/" + (int)HttpStatusCode.PaymentRequired);
        }
    }
}
