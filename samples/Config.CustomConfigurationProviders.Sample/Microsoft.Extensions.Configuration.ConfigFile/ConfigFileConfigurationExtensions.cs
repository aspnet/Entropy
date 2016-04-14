// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Extensions.Configuration.ConfigFile;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigFileConfigurationExtensions
    {
        /// <summary>
        /// Adds configuration values of a *.config file to the ConfigurationBuilder
        /// </summary>
        /// <param name="builder">Builder to add configuration values to</param>
        /// <param name="configContents">Contents of *.config file</param>
        /// <param name="parsers">Additional parsers to use to parse the config contents</param>
        public static IConfigurationBuilder AddConfiguration(this IConfigurationBuilder builder, string configContents, params IConfigurationParser[] parsers)
        {
            if (configContents == null)
            {
                throw new ArgumentNullException(nameof(configContents));
            }
            else if (string.IsNullOrEmpty(configContents))
            {
                throw new ArgumentException("Contents for configuration cannot be empty.", nameof(configContents));
            }

            return builder.Add(new ConfigFileConfigurationSource(configContents, false, false, parsers));
        }

        /// <summary>
        /// Adds configuration values for a *.config file to the ConfigurationBuilder
        /// </summary>
        /// <param name="builder">Builder to add configuration values to</param>
        /// <param name="path">Path to *.config file</param>
        public static IConfigurationBuilder AddConfigFile(this IConfigurationBuilder builder, string path)
        {
            return builder.AddConfigFile(path, optional: false);
        }

        /// <summary>
        /// Adds configuration values for a *.config file to the ConfigurationBuilder
        /// </summary>
        /// <param name="builder">Builder to add configuration values to</param>
        /// <param name="path">Path to *.config file</param>
        /// <param name="optional">true if file is optional; false otherwise</param>
        /// <param name="parsers">Additional parsers to use to parse the config file</param>
        public static IConfigurationBuilder AddConfigFile(this IConfigurationBuilder builder, string path, bool optional, params IConfigurationParser[] parsers)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            else if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path for configuration cannot be null/empty.", nameof(path));
            }

            if (!optional && !File.Exists(path))
            {
                throw new FileNotFoundException($"Could not find configuration file. File: [{path}]", path);
            }

            return builder.Add(new ConfigFileConfigurationSource(path, true, optional, parsers));
        }
    }
}
