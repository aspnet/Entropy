// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.AspNetCore.Server.IntegrationTesting.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace EntropyTests
{
    public static class TestServices
    {
        public static string WorkingDirectory { get; }
#if NET46
            = AppDomain.CurrentDomain.BaseDirectory;
#else
            = AppContext.BaseDirectory;
#endif

        public static void LogResponseOnFailedAssert(this ILogger logger, HttpResponseMessage response, string responseText, Action assert)
        {
            try
            {
                assert();
            }
            catch (XunitException)
            {
                logger.LogWarning(response.ToString());
                if (!string.IsNullOrEmpty(responseText))
                {
                    logger.LogWarning(responseText);
                }
                throw;
            }
        }

        public static async Task RunSiteTest(
            string siteName,
            ServerType serverType,
            RuntimeFlavor runtimeFlavor,
            RuntimeArchitecture architecture,
            string applicationBaseUrl,
            ITestOutputHelper xunitLogger,
            Func<HttpClient, ILogger, CancellationToken, Task> validator)
        {
            var factory = new LoggerFactory()
                .AddConsole();
            if (xunitLogger != null)
            {
                factory.AddProvider(new XunitLoggerProvider(xunitLogger));
            }

            var logger = factory.CreateLogger(string.Format("{0}:{1}:{2}:{3}", siteName, serverType, runtimeFlavor, architecture));

            using (logger.BeginScope("RunSiteTest"))
            {
                var deploymentParameters = new DeploymentParameters(GetApplicationDirectory(siteName), serverType, runtimeFlavor, architecture)
                {
                    ApplicationBaseUriHint = TestUriHelper.BuildTestUri(applicationBaseUrl).ToString(),
                    SiteName = "HttpTestSite",
                    ServerConfigTemplateContent = serverType == ServerType.Nginx ? File.ReadAllText(Path.Combine(WorkingDirectory, "nginx.conf")) : string.Empty,
                    PublishApplicationBeforeDeployment = true,
                    TargetFramework = runtimeFlavor == RuntimeFlavor.Clr ? "net46" : "netcoreapp1.1",
                    ApplicationType = runtimeFlavor == RuntimeFlavor.Clr ? ApplicationType.Standalone : ApplicationType.Portable
                };

                using (var deployer = ApplicationDeployerFactory.Create(deploymentParameters, logger))
                {
                    var deploymentResult = deployer.Deploy();
                    var httpClientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpClientHandler)
                    {
                        BaseAddress = new Uri(deploymentResult.ApplicationBaseUri),
                        Timeout = TimeSpan.FromSeconds(10)
                    };

                    using (httpClient)
                    {
                        await validator(httpClient, logger, deploymentResult.HostShutdownToken);
                    }
                }
            }
        }

        public static string GetApplicationDirectory(string applicationName)
        {
            var solutionRoot = GetSolutionDirectory();
            return Path.Combine(solutionRoot, "samples", applicationName);
        }

        public static string GetSolutionDirectory()
        {
            var applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;
            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var solutionFile = new FileInfo(Path.Combine(directoryInfo.FullName, "Entropy.sln"));
                if (solutionFile.Exists)
                {
                    return directoryInfo.FullName;
                }

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Solution root could not be located using application root {applicationBasePath}.");
        }
    }
}
