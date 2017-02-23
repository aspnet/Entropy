using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.AspNetCore.Testing.xunit;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EntropyTests
{
    public abstract class E2ETestBase
    {
        private string _siteName;
        private ITestOutputHelper _output;

        protected E2ETestBase(ITestOutputHelper output, string siteName, int basePort)
        {
            _output = output;
            _siteName = siteName;
            BasePort = basePort;
        }

        public int BasePort { get; }

#if NETCOREAPP1_1
        [Fact]
        public Task KestrelX64CoreCLR()
        {
            return RunTestAsync(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, BasePort);
        }

        [ConditionalFact(Skip = "Skipped until https://github.com/aspnet/Hosting/issues/949")]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task KestrelX86CoreCLRWindows()
        {
            return RunTestAsync(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, BasePort + 1);
        }

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task KestrelX64CLRWindows()
        {
            return RunTestAsync(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x64, BasePort + 2);
        }

        [ConditionalFact(Skip = "Skipped until https://github.com/aspnet/Hosting/issues/949")]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task KestrelX86CLRWindows()
        {
            return RunTestAsync(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x86, BasePort + 3);
        }

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task WebListenerX64CoreCLRWindows()
        {
            return RunTestAsync(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, BasePort + 4);
        }

        [ConditionalFact(Skip = "Skipped until https://github.com/aspnet/Hosting/issues/949")]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task WebListenerX86CoreCLRWindows()
        {
            return RunTestAsync(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86, BasePort + 5);
        }

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task WebListenerX64CLRWindows()
        {
            return RunTestAsync(ServerType.WebListener, RuntimeFlavor.Clr, RuntimeArchitecture.x64, BasePort + 6);
        }

        [ConditionalFact(Skip = "Skipped until https://github.com/aspnet/Hosting/issues/949")]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task WebListenerX86CLRWindows()
        {
            return RunTestAsync(ServerType.WebListener, RuntimeFlavor.Clr, RuntimeArchitecture.x86, BasePort + 7);
        }

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Windows)]
        public Task NgnixX64NonWindows()
        {
            return RunTestAsync(ServerType.Nginx, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64, BasePort + 8);
        }
#elif NET452
// E2E tests only need to be defined for one TFM.
#else
#error NETCOREAPP1_1 is no longer defined. Update the TFMs in this file.
#endif

        protected virtual Task<HttpResponseMessage> GetResponse(HttpClient client)
            => client.GetAsync(string.Empty);

        private Task RunTestAsync(
            ServerType serverType,
            RuntimeFlavor runtimeFlavor,
            RuntimeArchitecture architecture,
            int port)
        {
            var applicationBaseUrl = $"http://localhost:{port}";
            return TestServices.RunSiteTest(
                _siteName,
                serverType,
                runtimeFlavor,
                architecture,
                applicationBaseUrl,
                _output,
                ValidateAsync);
        }

        protected virtual async Task ValidateAsync(
            HttpClient httpClient,
            ILogger logger,
            CancellationToken token)
        {
            var response = await RetryHelper.RetryRequest(() =>
            {
                return GetResponse(httpClient);
            }, logger, token, retryCount: 30);

            var responseText = await response.Content.ReadAsStringAsync();

            logger.LogResponseOnFailedAssert(response, responseText, () =>
            {
                AssertResponse(response, responseText);
            });
        }

        protected virtual void AssertResponse(HttpResponseMessage response, string responseText)
        {
            throw new NotImplementedException("Must be overriden in derived types.");
        }
    }
}
