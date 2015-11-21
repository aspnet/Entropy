// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.Filters;

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class ModuleActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IModuleFactory _moduleFactory;
        private readonly IReadOnlyList<IFilterProvider> _filterProviders;

        public ModuleActionInvokerProvider(
            IEnumerable<IFilterProvider> filterProviders,
            IModuleFactory moduleFactory)
        {
            _moduleFactory = moduleFactory;
            _filterProviders = filterProviders.OrderBy(p => p.Order).ToList();
        }


        public int Order { get { return 0; } }

        public void OnProvidersExecuting(ActionInvokerProviderContext context)
        {
            var actionDescriptor = context.ActionContext.ActionDescriptor as ModuleActionDescriptor;

            if (actionDescriptor != null)
            {
                context.Result = new ModuleActionInvoker(
                    _filterProviders,
                    _moduleFactory,
                    context.ActionContext);
            }
        }

        public void OnProvidersExecuted(ActionInvokerProviderContext context)
        {
        }
    }
}