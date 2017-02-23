using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EntropyTests.LocalizationTests
{
    public class LocalizationWebTests : E2ETestBase
    {
        public LocalizationWebTests(ITestOutputHelper output)
            : base(output, "Localization.StarterWeb", 6400)
        {
        }

        protected override async Task ValidateAsync(HttpClient httpClient, ILogger logger, CancellationToken token)
        {
            var response = await RetryHelper.RetryRequest(async () =>
            {
                return await httpClient.GetAsync(string.Empty);
            }, logger, token, retryCount: 30);

            var responseText = await response.Content.ReadAsStringAsync();
            Assert.Contains("<h2>Application uses</h2>", responseText);

            // ===== French =====
            response = await RetryHelper.RetryRequest(async () =>
            {
                return await httpClient.GetAsync("?culture=fr&ui-culture=fr");
            }, logger, token, retryCount: 30);

            responseText = await response.Content.ReadAsStringAsync();
            Assert.Contains("<h2>Utilisations d'application</h2>", responseText);
        }
    }
}
