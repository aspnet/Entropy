using System.Threading.Tasks;
using Microsoft.AspNet.Server.Testing;
using Microsoft.AspNet.Testing.xunit;
using Xunit;

namespace EntropyTests.ConfigTests
{
    public class ConfigWalkingValuesWebTests
    {
        private const string SiteName = "Config.WalkingValues.Web";

        [Theory]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:5900")]
        public async Task RunSite_AllPlatforms(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        [InlineData(ServerType.WebListener, RuntimeFlavor.Clr, RuntimeArchitecture.x86, "http://localhost:5901")]
        [InlineData(ServerType.WebListener, RuntimeFlavor.Clr, RuntimeArchitecture.x64, "http://localhost:5902")]
        [InlineData(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:5903")]
        [InlineData(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:5904")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x86, "http://localhost:5905")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x64, "http://localhost:5906")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:5907")]
        // Already covered by all platforms:
        //[InlineData(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:5908")]
        public async Task RunSite_WindowsOnly(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Windows)]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Mono, RuntimeArchitecture.x64, "http://localhost:5909")]
        public async Task RunSite_NonWindowsOnly(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        private async Task RunSite(ServerType serverType, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await TestServices.RunSiteTest(
                SiteName,
                serverType,
                runtimeFlavor,
                architecture,
                applicationBaseUrl,
                async (httpClient, logger, token) =>
                {
                    var response = await RetryHelper.RetryRequest(async () =>
                    {
                        return await httpClient.GetAsync(string.Empty);
                    }, logger, token, retryCount: 30);

                    var responseText = await response.Content.ReadAsStringAsync();

                    logger.LogResponseOnFailedAssert(response, responseText, () =>
                    {
                        Assert.Contains("ValueOfKey-Data:Inventory:ConnectionString", responseText);
                        Assert.Contains("ValueOfKey-Data:Inventory:Provider", responseText);
                    });
                });
        }
    }
}

