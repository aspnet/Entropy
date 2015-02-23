using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class ModuleActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IModuleFactory _moduleFactory;
        private readonly IReadOnlyList<IFilterProvider> _filterProviders;
        private readonly IServiceProvider _serviceProvider;
        private readonly IInputFormattersProvider _inputFormattersProvider;
        private readonly IModelBinderProvider _modelBinderProvider;
        private readonly IModelValidatorProviderProvider _modelValidatorProviderProvider;
        private readonly IValueProviderFactoryProvider _valueProviderFactoryProvider;
        private readonly IScopedInstance<ActionBindingContext> _actionBindingContextAccessor;

        public ModuleActionInvokerProvider(
            IModuleFactory moduleFactory,
            IEnumerable<IFilterProvider> filterProviders,
            IInputFormattersProvider inputFormattersProvider,
            IModelBinderProvider modelBinderProvider,
            IModelValidatorProviderProvider modelValidatorProviderProvider,
            IValueProviderFactoryProvider valueProviderFactoryProvider,
            IScopedInstance<ActionBindingContext> actionBindingContextAccessor,
            IServiceProvider serviceProvider)
        {
            _moduleFactory = moduleFactory;
            _filterProviders = filterProviders.OrderBy(p => p.Order).ToList();
            _inputFormattersProvider = inputFormattersProvider;
            _modelBinderProvider = modelBinderProvider;
            _modelValidatorProviderProvider = modelValidatorProviderProvider;
            _valueProviderFactoryProvider = valueProviderFactoryProvider;
            _actionBindingContextAccessor = actionBindingContextAccessor;
            _serviceProvider = serviceProvider;
        }


        public int Order { get { return 0; } }

        public void OnProvidersExecuting(ActionInvokerProviderContext context)
        {
            var actionDescriptor = context.ActionContext.ActionDescriptor as ModuleActionDescriptor;

            if (actionDescriptor != null)
            {
                context.Result = new ModuleActionInvoker(
                    context.ActionContext,
                    _filterProviders,
                    _moduleFactory,
                    actionDescriptor,
                    _inputFormattersProvider,
                    _modelBinderProvider, 
                    _modelValidatorProviderProvider,
                    _valueProviderFactoryProvider,
                    _actionBindingContextAccessor);
            }
        }

        public void OnProvidersExecuted(ActionInvokerProviderContext context)
        {
        }
    }
}