// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Configuration.ConfigFile
{
    public class ConfigFileConfigurationSource : IConfigurationSource
    {
        public ILogger Logger { get; set; }
        public string Configuration { get; set; }
        public bool LoadFromFile { get; set; }
        public bool Optional { get; set; }
        public IEnumerable<IConfigurationParser> Parsers { get; set; }

        public ConfigFileConfigurationSource(string configuration, bool loadFromFile, bool optional, params IConfigurationParser[] parsers)
            : this(configuration, loadFromFile, optional, null, parsers)
        { }

        public ConfigFileConfigurationSource(string configuration, bool loadFromFile, bool optional, ILogger logger, params IConfigurationParser[] parsers)
        {
            LoadFromFile = loadFromFile;
            Configuration = configuration;
            Optional = optional;
            Logger = logger;

            var parsersToUse = new List<IConfigurationParser> {
                new KeyValueParser(),
                new KeyValueParser("name", "connectionString")
            };

            parsersToUse.AddRange(parsers);

            Parsers = parsersToUse.ToArray();
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ConfigFileConfigurationProvider(Configuration, LoadFromFile, Optional, Logger, Parsers);
        }
    }
}
