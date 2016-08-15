using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;

namespace EntropyTests.ConfigTests
{
    public class ConfigSettingObjectWebTests
    {
        private const string SiteName = "Config.SettingObject.Web";

        [Theory]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:5800")]
        public async Task RunSite_AllPlatforms(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        //[InlineData(ServerType.WebListener, RuntimeFlavor.Clr, RuntimeArchitecture.x86, "http://localhost:5801")]
        [InlineData(ServerType.WebListener, RuntimeFlavor.Clr, RuntimeArchitecture.x64, "http://localhost:5802")]
        //[InlineData(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:5803")]
        [InlineData(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:5804")]
        //[InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x86, "http://localhost:5805")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x64, "http://localhost:5806")]
        //[InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:5807")]
        // Already covered by all platforms:
        //[InlineData(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:5808")]
        public async Task RunSite_WindowsOnly(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Windows)]
        //[InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x64, "http://localhost:5809")] // Disabled due to https://github.com/dotnet/corefx/issues/9012
        [InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:5810")]
        [InlineData(ServerType.Nginx, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:5811")]
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
                    string expectedText =
"Retry Count 42\r\n" +
"Default Ad Block House\r\n" +
"Ad Block Contoso Origin sql-789 Product Code contoso2014\r\n" +
"Ad Block House Origin blob-456 Product Code 123\r\n";

                    logger.LogResponseOnFailedAssert(response, responseText, () =>
                    {
                        Assert.Equal(expectedText, responseText);
                    });
                });
        }
    }
}
