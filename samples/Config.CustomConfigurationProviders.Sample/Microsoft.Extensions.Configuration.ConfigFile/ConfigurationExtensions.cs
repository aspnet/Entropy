// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public const string AppSettings = "appSettings";

        public static string GetAppSetting(this IConfiguration configuration, string name)
        {
            return configuration?.GetSection(AppSettings)[name];
        }
        
        /// <summary>
        /// Gets all of the configuration sections for a set of keys
        /// </summary>
        /// <param name="sectionNames">The names of the sections from the top-most level to lowest</param>
        public static ImmutableDictionary<string, IConfigurationSection> GetSection(this IConfiguration configuration, params string[] sectionNames)
        {
            if (sectionNames.Length == 0)
                return ImmutableDictionary<string, IConfigurationSection>.Empty;

            var fullKey = string.Join(ConfigurationPath.KeyDelimiter, sectionNames);

            return configuration?.GetSection(fullKey).GetChildren()?.ToImmutableDictionary(x => x.Key, x => x);
        }

        /// <summary>
        /// Given a set of keys, will join them and get the value at that level
        /// </summary>
        /// <param name="keys">Names of the sections from top-level to lowest level</param>
        /// <returns>The value of that key</returns>
        public static string GetValue(this IConfiguration configuration, params string[] keys)
        {
            if (keys.Length == 0)
            {
                throw new ArgumentException("Need to provide keys", nameof(keys));
            }

            var fullKey = string.Join(ConfigurationPath.KeyDelimiter, keys);

            return configuration?[fullKey];
        }
    }
}
