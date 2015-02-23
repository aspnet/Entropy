// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ModelBinding;
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
            IReadOnlyList<IFilterProvider> filterProviders,
            IModuleFactory moduleFactory,
            ModuleActionDescriptor descriptor,
            IInputFormattersProvider inputFormatterProvider,
            IModelBinderProvider modelBinderProvider,
            IModelValidatorProviderProvider modelValidatorProviderProvider,
            IValueProviderFactoryProvider valueProviderFactoryProvider,
            IScopedInstance<ActionBindingContext> actionBindingContextAccessor)
            : base(
                  actionContext,
                  filterProviders,
                  inputFormatterProvider,
                  modelBinderProvider,
                  modelValidatorProviderProvider,
                  valueProviderFactoryProvider,
                  actionBindingContextAccessor)
        {
            _descriptor = descriptor;
            _moduleFactory = moduleFactory;
        }

        protected override object CreateInstance()
        {
            _module = (MvcModule)_moduleFactory.CreateModule(ActionContext);
            return _module;
        }

        protected override void ReleaseInstance(object instance)
        {
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

        protected override Task<IDictionary<string, object>> GetActionArgumentsAsync(
            ActionContext context,
            ActionBindingContext actionBindingContext)
        {
            return Task.FromResult<IDictionary<string, object>>(
                new Dictionary<string, object>(StringComparer.Ordinal));
        }
    }
}