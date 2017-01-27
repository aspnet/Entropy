// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Mvc.DynamicCorsPolicyProvider
{
    public class DynamicCorsPolicyProvider : ICorsPolicyProvider
    {
        private readonly IFileProvider _fileProvider;
        private readonly ILogger<DynamicCorsPolicyProvider> _logger;
        private Dictionary<string, CorsPolicy> _policies;
        private readonly DynamicCorsPolicyProviderOptions _options;

        public DynamicCorsPolicyProvider(
            IFileProvider fileProvider,
            ILogger<DynamicCorsPolicyProvider> logger,
            IOptions<DynamicCorsPolicyProviderOptions> options)
        {
            _fileProvider = fileProvider;
            _logger = logger;
            _options = options.Value;

            if (string.IsNullOrEmpty(_options.SettingsFileName))
            {
                throw new ArgumentException("The settings file name cannot be null or empty", nameof(_options.SettingsFileName));
            }

            // Initialize the policies the first time this policy provider is instantiated (singleton)
            BuildPolicies();

            // Register a file change callback to refresh these policies whenever a change is made
            RegisterCallback();
        }

        public Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            return Task.FromResult(_policies[policyName]);
        }

        private void RegisterCallback()
        {
            _fileProvider.Watch(_options.SettingsFileName)
                .RegisterChangeCallback((state) =>
                {
                    _logger.LogInformation("CORS settings changed. Updating policies...");
                    BuildPolicies();

                    // Re-register callback
                    RegisterCallback();
                }, state: null);
        }

        private void BuildPolicies()
        {
            string content;
            using (var streamReader = new StreamReader(_fileProvider.GetFileInfo(_options.SettingsFileName).CreateReadStream()))
            {
                content = streamReader.ReadToEnd();
            }
            var policySettings = JsonConvert.DeserializeObject<IEnumerable<PolicySetting>>(content);

            var temp = new Dictionary<string, CorsPolicy>(StringComparer.OrdinalIgnoreCase);
            foreach (var policySetting in policySettings)
            {
                var corsPolicyBuilder = new CorsPolicyBuilder();
                if (policySetting.AllowedOrigins == null || policySetting.AllowedOrigins.Length == 0)
                {
                    corsPolicyBuilder.AllowAnyOrigin();
                }
                else
                {
                    corsPolicyBuilder.WithOrigins(policySetting.AllowedOrigins);
                }

                if (policySetting.AllowedHeaders == null || policySetting.AllowedHeaders.Length == 0)
                {
                    corsPolicyBuilder.AllowAnyHeader();
                }
                else
                {
                    corsPolicyBuilder.WithHeaders(policySetting.AllowedHeaders);
                }

                temp.Add(policySetting.Name, corsPolicyBuilder.Build());
            }

            // Delay 'updating' the policies as late as possible to prevent corrupting ongoing requests from
            // reading bad policy settings
            _policies = temp;

            _logger.LogInformation("Updated CORS policies");
        }

        private class PolicySetting
        {
            public string Name { get; set; }
            public string[] AllowedOrigins { get; set; }
            public string[] AllowedHeaders { get; set; }
        }
    }
}
