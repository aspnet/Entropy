// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class ModuleActionInvoker : FilterActionInvoker
    {
        private readonly ModuleActionDescriptor _descriptor;
        private readonly IModuleFactory _moduleFactory;
        private MvcModule _module;

        public ModuleActionInvoker(
            ActionContext actionContext,
            IActionBindingContextProvider bindingContextProvider,
            INestedProviderManager<FilterProviderContext> filterProvider,
            IModuleFactory moduleFactory,
            ModuleActionDescriptor descriptor)
            : base(actionContext, bindingContextProvider, filterProvider)
        {
            _descriptor = descriptor;
            _moduleFactory = moduleFactory;
        }

        public override Task InvokeAsync()
        {
            _module = (MvcModule)_moduleFactory.CreateModule(ActionContext);
            ActionContext.Controller = _module;

            return base.InvokeAsync();
        }

        protected override Task<IActionResult> InvokeActionAsync(ActionExecutingContext actionExecutingContext)
        {
            var actionInfo = _module.Actions[_descriptor.Index];

            var value = actionInfo.Func();

            var actionResult = CreateActionResult(value);
            return Task.FromResult(actionResult);
        }

        private IActionResult CreateActionResult(object value)
        {
            // optimize common path
            var actionResult = value as IActionResult;
            if (actionResult != null)
            {
                return actionResult;
            }
            else if (value == null)
            {
                return new NoContentResult();
            }
            else
            {
                return new ObjectResult(value);
            }
        }
    }
}