using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;

namespace Config.CustomSource.Web
{
    public class MyConfigSource : IConfigurationSource
    {
        public bool TryGet(string key, out string value)
        {
            switch (key)
            {
                case "Hardcoded:1:Caption":
                    value = "One";
                    return true;
                case "Hardcoded:2:Caption":
                    value = "Two";
                    return true;
            }
            value = null;
            return false;
        }

        public void Load()
        {
            // no loading or reloading, this source is all hardcoded
        }
        
        public IEnumerable<string> ProduceConfigurationSections(IEnumerable<string> earlierKeys, string prefix, string delimiter)
        {
            // TODO: ProduceSubKeys method signature is pretty bad

            if (prefix == "" && delimiter == ":")
            {
                return earlierKeys.Concat(new[] { "Hardcoded" });
            }
            if (prefix == "Hardcoded:" && delimiter == ":")
            {
                return earlierKeys.Concat(new[] { "1", "2" });
            }
            if (prefix == "Hardcoded:1:" || prefix == "Hardcoded:2:")
            {
                return earlierKeys.Concat(new[] { "Caption" });
            }
            return earlierKeys;
        }

        public void Set(string key, string value)
        {
        }
    }
}