using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class ModuleActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IModuleFactory _moduleFactory;
        private readonly IReadOnlyList<IFilterProvider> _filterProviders;
        private readonly IServiceProvider _serviceProvider;
        private readonly IReadOnlyList<IInputFormatter> _inputFormatters;
        private readonly IReadOnlyList<IModelBinder> _modelBinders;
        private readonly IReadOnlyList<IModelValidatorProvider> _modelValidatorProviders;
        private readonly IReadOnlyList<IValueProviderFactory> _valueProviderFactories;
        private readonly IScopedInstance<ActionBindingContext> _actionBindingContextAccessor;

        public ModuleActionInvokerProvider(
            IModuleFactory moduleFactory,
            IEnumerable<IFilterProvider> filterProviders,
            IReadOnlyList<IInputFormatter> inputFormatters,
            IReadOnlyList<IModelBinder> modelBinders,
            IReadOnlyList<IModelValidatorProvider> modelValidatorProviders,
            IReadOnlyList<IValueProviderFactory> valueProviderFactories,
            IScopedInstance<ActionBindingContext> actionBindingContextAccessor,
            IServiceProvider serviceProvider)
        {
            _moduleFactory = moduleFactory;
            _filterProviders = filterProviders.OrderBy(p => p.Order).ToList();
            _inputFormatters = inputFormatters;
            _modelBinders = modelBinders;
            _modelValidatorProviders = modelValidatorProviders;
            _valueProviderFactories = valueProviderFactories;
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
                    _inputFormatters,
                    _modelBinders,
                    _modelValidatorProviders,
                    _valueProviderFactories,
                    _actionBindingContextAccessor);
            }
        }

        public void OnProvidersExecuted(ActionInvokerProviderContext context)
        {
        }
    }
}