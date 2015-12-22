using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Server.Testing;
using Microsoft.AspNet.Testing.xunit;
using Xunit;

namespace EntropyTests.BuilderTests
{
    public class BuilderFilteringWebTests
    {
        private const string SiteName = "Builder.Filtering.Web";

        [Theory]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:5500")]
        public async Task RunSite_AllPlatforms(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        [InlineData(ServerType.WebListener, RuntimeFlavor.Clr, RuntimeArchitecture.x86, "http://localhost:5501")]
        [InlineData(ServerType.WebListener, RuntimeFlavor.Clr, RuntimeArchitecture.x64, "http://localhost:5502")]
        [InlineData(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:5503")]
        [InlineData(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:5504")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x86, "http://localhost:5505")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x64, "http://localhost:5506")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:5507")]
        // Already covered by all platforms:
        //[InlineData(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:5508")]
        public async Task RunSite_WindowsOnly(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Windows)]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Mono, RuntimeArchitecture.x64, "http://localhost:5509")]
        public async Task RunSite_NonWindowsOnly(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        private async Task RunSite(ServerType serverType, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await TestServices.RunSiteTest(SiteName, serverType, runtimeFlavor, architecture, applicationBaseUrl,
                async (httpClient, logger, token) =>
                {
                    var response = await RetryHelper.RetryRequest(async () =>
                    {
                        return await httpClient.GetAsync(string.Empty);
                    }, logger, token, retryCount: 30);

                    var responseText = await response.Content.ReadAsStringAsync();

                    logger.LogResponseOnFailedAssert(response, responseText, () =>
                    {
                        Assert.Equal("Before\r\nHello world\r\nAfter\r\n", responseText);

                        var customHeaderText = response.Headers.GetValues("CustomHeader").Single();
                        Assert.Equal("My Header", customHeaderText);
                    });
                });
        }
    }
}
