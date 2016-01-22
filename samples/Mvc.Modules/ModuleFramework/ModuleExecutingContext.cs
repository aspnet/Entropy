// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.ModuleFramework
{
    public class ModuleExecutingContext : FilterContext
    {
        public ModuleExecutingContext(ActionContext context, IList<IFilterMetadata> filters, MvcModule module)
            : base(context, filters)
        {
            Module = module;
        }

        public virtual MvcModule Module { get; protected set; }

        public virtual IActionResult Result { get; set; }
    }
}
