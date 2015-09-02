
using System;
using Microsoft.AspNet.Mvc.Actions;

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class ModuleActionDescriptor : ActionDescriptor
    {
        public Type ModuleType { get; set; }

        public int Index { get; set; }
    }
}