using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Testing;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;

namespace EntropyTests.ContainerTests
{
    public class ContainerFallbackWebTests
    {
        private const string SiteName = "Container.Fallback.Web";

        [Theory]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:6000")]
        public async Task RunSite_AllPlatforms(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        //[InlineData(ServerType.WebListener, RuntimeFlavor.Clr, RuntimeArchitecture.x86, "http://localhost:6001")]
        [InlineData(ServerType.WebListener, RuntimeFlavor.Clr, RuntimeArchitecture.x64, "http://localhost:6002")]
        //[InlineData(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:6003")]
        [InlineData(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:6004")]
        //[InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x86, "http://localhost:6005")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x64, "http://localhost:6006")]
        //[InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:6007")]
        // Already covered by all platforms:
        //[InlineData(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:6008")]
        public async Task RunSite_WindowsOnly(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Windows)]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x64, "http://localhost:6009")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:6010")]
        public async Task RunSite_NonWindowsOnly(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        private async Task RunSite(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            // calltwo and callthree in the expression below 
            // increment on every call.
            // The test verifies that everything else stays the same 
            // and that the two values that change are increasing in value
            string responseMatcher =
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

            await TestServices.RunSiteTest(
                SiteName,
                server,
                runtimeFlavor,
                architecture,
                applicationBaseUrl,
                async (httpClient, logger, token) =>
                {
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
                });
        }
    }
}
