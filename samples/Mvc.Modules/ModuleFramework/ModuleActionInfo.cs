using System;

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class ModuleActionInfo
    {
        public Func<object> Func { get; set; }

        public string Path { get; set; }

        public string Verb { get; set; }
    }
}