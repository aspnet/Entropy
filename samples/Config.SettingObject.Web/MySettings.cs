using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;

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
            string value;
            if (configuration.TryGet("RetryCount", out value))
            {
                RetryCount = int.Parse(value);
            }
            if (configuration.TryGet("DefaultAdBlock", out value))
            {
                DefaultAdBlock = value;
            }

            var items = new List<AdBlock>();
            foreach (var subConfig in configuration.GetSubKeys("AdBlock"))
            {
                var item = new AdBlock { Name = subConfig.Key };
                if (subConfig.Value.TryGet("Origin", out value))
                {
                    item.Origin = value;
                }
                if (subConfig.Value.TryGet("ProductCode", out value))
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