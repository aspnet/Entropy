using System.Threading.Tasks;
using EntropyTests;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;

namespace FunctionalTests.LocalizationTests
{
    public class CustomResourceManagerTests
    {
        private const string SiteName = "Localization.CustomResourceManager";

        [Theory]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, "http://localhost:9200", Skip = "https://github.com/aspnet/Entropy/issues/186")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, "http://localhost:9201", Skip = "x86 not supported yet")]
        public async Task CustomResourceManagerFactory_ClassLibrarysReadCorrectly_CoreClr(
            ServerType server,
            RuntimeFlavor runtimeFlavor,
            RuntimeArchitecture architecture,
            string applicationBaseUrl)
        {
            await CustomResourceManagerFactory_ClassLibrarysReadCorrectly(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x64, "http://localhost:9202")]
        [InlineData(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x86, "http://localhost:9203", Skip = "x86 not supported yet")]
        public async Task CustomResourceManagerFactory_ClassLibrarysReadCorrectly_Clr(
            ServerType server,
            RuntimeFlavor runtimeFlavor,
            RuntimeArchitecture architecture,
            string applicationBaseUrl)
        {
            await CustomResourceManagerFactory_ClassLibrarysReadCorrectly(server, runtimeFlavor, architecture, applicationBaseUrl);
        }

        public Task CustomResourceManagerFactory_ClassLibrarysReadCorrectly(
            ServerType server,
            RuntimeFlavor runtimeFlavor,
            RuntimeArchitecture architecture,
            string applicationBaseUrl)
        {
            return TestServices.RunSiteTest(
                SiteName,
                server,
                runtimeFlavor,
                architecture,
                applicationBaseUrl,
                async (httpClient, logger, token) =>
                {
                    var response = await RetryHelper.RetryRequest(async () =>
                    {
                        return await httpClient.GetAsync("?culture=fr-FR&ui-culture=fr-FR");
                    }, logger, token, retryCount: 30);

                    var responseText = await response.Content.ReadAsStringAsync();

                    Assert.Contains("<h1>ClassLib Bonjour from greeting service</h1>", responseText);
                    Assert.Contains("<h1>Yelling (Outsidenamespace): BONJOUR OUTSIDE</h1>", responseText);
                });
        }
    }
}
