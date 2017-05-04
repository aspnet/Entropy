// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.AspNetCore.Testing.xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Entropy.FunctionalTests
{
    public abstract class E2ETestBase : LoggedTest
    {
        private string _siteName;

        protected E2ETestBase(ITestOutputHelper output, string siteName) : base(output)
        {
            _siteName = siteName;
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        [InlineData(ServerType.Kestrel, RuntimeArchitecture.x64, ApplicationType.Portable)]
        [InlineData(ServerType.Kestrel, RuntimeArchitecture.x64, ApplicationType.Standalone)]
        [InlineData(ServerType.Kestrel, RuntimeArchitecture.x86, ApplicationType.Standalone,
            Skip = "https://github.com/aspnet/Hosting/issues/949")]
        [InlineData(ServerType.WebListener, RuntimeArchitecture.x64, ApplicationType.Portable)]
        [InlineData(ServerType.WebListener, RuntimeArchitecture.x64, ApplicationType.Standalone)]
        [InlineData(ServerType.WebListener, RuntimeArchitecture.x86, ApplicationType.Standalone,
            Skip = "https://github.com/aspnet/Hosting/issues/949")]
        public Task WindowsOS(
            ServerType serverType,
            RuntimeArchitecture architecture,
            ApplicationType applicationType)
        {
            return RunTestAsync(serverType, architecture, applicationType);
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Windows)]
        [InlineData(ServerType.Kestrel, RuntimeArchitecture.x64, ApplicationType.Portable)]
        [InlineData(ServerType.Kestrel, RuntimeArchitecture.x64, ApplicationType.Standalone,
            Skip = "https://github.com/dotnet/cli/issues/6397")]
        [InlineData(ServerType.Nginx, RuntimeArchitecture.x64, ApplicationType.Portable)]
        [InlineData(ServerType.Nginx, RuntimeArchitecture.x64, ApplicationType.Standalone,
            Skip = "https://github.com/dotnet/cli/issues/6397")]
        public Task NonWindowsOS(
            ServerType serverType,
            RuntimeArchitecture architecture,
            ApplicationType applicationType)
        {
            return RunTestAsync(serverType, architecture, applicationType);
        }

        protected virtual Task<HttpResponseMessage> GetResponse(HttpClient client)
            => client.GetAsync(string.Empty);

        private async Task RunTestAsync(
            ServerType serverType,
            RuntimeArchitecture architecture,
            ApplicationType applicationType,
            [CallerMemberName] string testName = null)
        {
            testName = $"{GetType().Name}_{testName}_{serverType}_{architecture}_{applicationType}";

            using (StartLog(out var loggerFactory, testName))
            {
                await TestServices.RunSiteTest(
                    _siteName,
                    serverType,
                    architecture,
                    applicationType,
                    loggerFactory,
                    ValidateAsync);
            }
        }

        protected virtual async Task ValidateAsync(
            HttpClient httpClient,
            ILogger logger,
            CancellationToken token)
        {
            logger.LogInformation("Performing Request");
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
