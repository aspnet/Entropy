using System.Net.Http;
using System.Threading.Tasks;
using EntropyTests;
using Xunit;
using Xunit.Abstractions;

namespace FunctionalTests.LocalizationTests
{
    public class CustomResourceManagerTests : E2ETestBase
    {
        public CustomResourceManagerTests(ITestOutputHelper output)
            : base(output, "Localization.CustomResourceManager", 9200)
        {
        }

        protected override Task<HttpResponseMessage> GetResponse(HttpClient client)
        {
            return client.GetAsync("?culture=fr-FR&ui-culture=fr-FR");
        }

        protected override void AssertResponse(HttpResponseMessage response, string responseText)
        {
            Assert.Contains("<h1>ClassLib Bonjour from greeting service</h1>", responseText);
            Assert.Contains("<h1>Yelling (Outsidenamespace): BONJOUR OUTSIDE</h1>", responseText);
        }
    }
}
