// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
// WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF
// TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR
// NON-INFRINGEMENT.
// See the Apache 2 License for the specific language governing
// permissions and limitations under the License.

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
        
        public IEnumerable<string> ProduceSubKeys(IEnumerable<string> earlierKeys, string prefix, string delimiter)
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