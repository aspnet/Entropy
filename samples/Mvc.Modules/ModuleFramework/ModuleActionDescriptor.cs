
using System;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Microsoft.AspNetCore.Mvc.ModuleFramework
{
    public class ModuleActionDescriptor : ActionDescriptor
    {
        public Type ModuleType { get; set; }

        public int Index { get; set; }
    }
}