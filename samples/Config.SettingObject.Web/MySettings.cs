using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Configuration;

namespace Config.SettingObject.Web
{
    public class MySettings
    {
        public MySettings()
        {
            RetryCount = 3;
            AdBlocks = new Dictionary<string, AdBlock>(StringComparer.OrdinalIgnoreCase);
        }

        public int RetryCount { get; set; }
        public string DefaultAdBlock { get; set; }
        public IDictionary<string, AdBlock> AdBlocks { get; private set; }

        public void Read(IConfiguration configuration)
        {
            var value = configuration["RetryCount"];
            if (!string.IsNullOrEmpty(value))
            {
                RetryCount = int.Parse(value);
            }
			value = configuration["DefaultAdBlock"];
            if (!string.IsNullOrEmpty(value))
            {
                DefaultAdBlock = value;
            }

            var items = new List<AdBlock>();
            foreach (var subConfig in configuration.GetSection("AdBlock").GetChildren())
            {
                var item = new AdBlock { Name = subConfig.Key };
				value = subConfig["Origin"];
                if (!string.IsNullOrEmpty(value))
                {
                    item.Origin = value;
                }
				value = subConfig["ProductCode"];
                if (!string.IsNullOrEmpty(value))
                {
                    item.ProductCode = value;
                }
                items.Add(item);
            }
            AdBlocks = items.ToDictionary(
                item => item.Name, 
                item => item,
                StringComparer.OrdinalIgnoreCase);
        }

        public class AdBlock
        {
            public string Name { get; set; }
            public string ProductCode { get; set; }
            public string Origin { get; set; }
        }
    }
}