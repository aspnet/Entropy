using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Config.CustomProvider.Web
{
    public class MyConfigSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder) {
            return new MyConfigProvider();
        }
    }
}