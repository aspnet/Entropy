using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;

namespace EntropyTests.LocalizationTests
{
    public class LocalizationWebTests
    {
        private const string SiteName = "Localization.StarterWeb";

        [Theory(Skip = "https://github.com/aspnet/Entropy/issues/186")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:6200")]
        public async Task RunSite_AllPlatforms(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x86, "http://localhost:6201", Skip = "x86 not supported yet")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x64, "http://localhost:6202", Skip = "https://github.com/aspnet/Entropy/issues/186")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:6203", Skip = "x86 not supported yet")]
        public async Task RunSite_WindowsOnly(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Windows)]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x64, "http://localhost:6204", Skip = "Disabled due to https://github.com/dotnet/corefx/issues/9012")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:6205", Skip = "https://github.com/aspnet/Entropy/issues/186")]
        [InlineData(ServerType.Nginx, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:6206", Skip = "https://github.com/aspnet/Entropy/issues/186")]
        public async Task RunSite_NonWindowsOnly(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await RunSite(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        private async Task RunSite(ServerType server, RuntimeFlavor runtimeFlavor, RuntimeArchitecture architecture, string applicationBaseUrl)
        {
            await TestServices.RunSiteTest(
                SiteName,
                server,
                runtimeFlavor,
                architecture,
                applicationBaseUrl,
                async (httpClient, logger, token) =>
                {
                    // ===== English =====
                    var response = await RetryHelper.RetryRequest(async () =>
                    {
                        return await httpClient.GetAsync(string.Empty);
                    }, logger, token, retryCount: 30);

                    var responseText = await response.Content.ReadAsStringAsync();

                    var headingIndex = responseText.IndexOf("<h2>Application uses</h2>");
                    Assert.True(headingIndex >= 0);

                    // ===== French =====
                    response = await RetryHelper.RetryRequest(async () =>
                    {
                        return await httpClient.GetAsync("?culture=fr&ui-culture=fr");
                    }, logger, token, retryCount: 30);

                    responseText = await response.Content.ReadAsStringAsync();

                    headingIndex = responseText.IndexOf("<h2>Utilisations d'application</h2>");
                    Assert.True(headingIndex >= 0);
                });
        }
    }
}
