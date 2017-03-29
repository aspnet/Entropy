// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
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

        protected E2ETestBase(ITestOutputHelper output, string siteName)
        {
            _output = output;
            _siteName = siteName;
        }

#if NETCOREAPP1_1
        [Fact]
        public Task KestrelX64CoreCLR()
        {
            return RunTestAsync(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64);
        }

        [ConditionalFact(Skip = "Skipped until https://github.com/aspnet/Hosting/issues/949")]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task KestrelX86CoreCLRWindows()
        {
            return RunTestAsync(ServerType.Kestrel, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64);
        }

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task KestrelX64CLRWindows()
        {
            return RunTestAsync(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x64);
        }

        [ConditionalFact(Skip = "Skipped until https://github.com/aspnet/Hosting/issues/949")]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task KestrelX86CLRWindows()
        {
            return RunTestAsync(ServerType.Kestrel, RuntimeFlavor.Clr, RuntimeArchitecture.x86);
        }

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task WebListenerX64CoreCLRWindows()
        {
            return RunTestAsync(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64);
        }

        [ConditionalFact(Skip = "Skipped until https://github.com/aspnet/Hosting/issues/949")]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task WebListenerX86CoreCLRWindows()
        {
            return RunTestAsync(ServerType.WebListener, RuntimeFlavor.CoreClr, RuntimeArchitecture.x86);
        }

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task WebListenerX64CLRWindows()
        {
            return RunTestAsync(ServerType.WebListener, RuntimeFlavor.Clr, RuntimeArchitecture.x64);
        }

        [ConditionalFact(Skip = "Skipped until https://github.com/aspnet/Hosting/issues/949")]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public Task WebListenerX86CLRWindows()
        {
            return RunTestAsync(ServerType.WebListener, RuntimeFlavor.Clr, RuntimeArchitecture.x86);
        }

        [ConditionalFact]
        [OSSkipCondition(OperatingSystems.Windows)]
        public Task NgnixX64NonWindows()
        {
            return RunTestAsync(ServerType.Nginx, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64);
        }
#elif NET46
// E2E tests only need to be defined for one TFM.
#else
#error NETCOREAPP1_1 is no longer defined. Update the TFMs in this file.
#endif

        protected virtual Task<HttpResponseMessage> GetResponse(HttpClient client)
            => client.GetAsync(string.Empty);

        private Task RunTestAsync(
            ServerType serverType,
            RuntimeFlavor runtimeFlavor,
            RuntimeArchitecture architecture)
        {
            var applicationBaseUrl = $"http://localhost:{GetNextPort()}";
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

        private static int GetNextPort()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // Let the OS assign the next available port. Unless we cycle through all ports
                // on a test run, the OS will always increment the port number when making these calls.
                // This prevents races in parallel test runs where a test is already bound to
                // a given port, and a new test is able to bind to the same port due to port
                // reuse being enabled by default by the OS.
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                return ((IPEndPoint)socket.LocalEndPoint).Port;
            }
        }
    }
}
