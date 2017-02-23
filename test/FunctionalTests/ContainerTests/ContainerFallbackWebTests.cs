using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EntropyTests.ContainerTests
{
    public class ContainerFallbackWebTests : E2ETestBase
    {
        public ContainerFallbackWebTests(ITestOutputHelper output)
            : base(output, "Container.Fallback.Web", 6000)
        {
        }

        protected override async Task ValidateAsync(HttpClient httpClient, ILogger logger, CancellationToken token)
        {
            // calltwo and callthree in the expression below 
            // increment on every call.
            // The test verifies that everything else stays the same 
            // and that the two values that change are increasing in value
            var responseMatcher =
@"---------- MyMiddleware ctor\r\n" +
@"CallOne\[1\]\r\n" +
@"CallTwo\[2\]\r\n" +
@"CallThree\[3\]\r\n" +
@"---------- context\.RequestServices\r\n" +
@"CallOne\[1\]\r\n" +
@"CallTwo\[(?<calltwo>\d+)\]\r\n" +
@"CallThree\[(?<callthree>\d+)\]\r\n" +
@"---------- Done\r\n";

            var responseRegex = new Regex(responseMatcher, RegexOptions.Compiled | RegexOptions.Multiline);


            // ===== First call =====
            var response = await RetryHelper.RetryRequest(async () =>
            {
                return await httpClient.GetAsync(string.Empty);
            }, logger, token, retryCount: 30);

            var responseText = await response.Content.ReadAsStringAsync();

            var match = responseRegex.Match(responseText);
            Assert.True(match.Success);

            var callTwo1 = int.Parse(match.Groups["calltwo"].Value);
            var callThree1 = int.Parse(match.Groups["callthree"].Value);

            // ===== Second call =====
            response = await RetryHelper.RetryRequest(async () =>
            {
                return await httpClient.GetAsync(string.Empty);
            }, logger, token, retryCount: 30);

            responseText = await response.Content.ReadAsStringAsync();
            logger.LogResponseOnFailedAssert(response, responseText, () =>
            {
                match = responseRegex.Match(responseText);
                Assert.True(match.Success);

                var callTwo2 = int.Parse(match.Groups["calltwo"].Value);
                var callThree2 = int.Parse(match.Groups["callthree"].Value);

                Assert.True(callTwo1 < callTwo2);
                Assert.True(callThree1 < callThree2);
            });
        }
    }
}
