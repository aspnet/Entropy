// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class ModuleContext : ActionContext
    {
        public ModuleContext(ActionContext context)
            : base(context)
        {
        }

        public new ModuleActionDescriptor ActionDescriptor
        {
            get { return (ModuleActionDescriptor)base.ActionDescriptor; }
            set { base.ActionDescriptor = value; }
        }
    }
}
