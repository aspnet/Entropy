using System;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class ModuleActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IModuleFactory _moduleFactory;
        private readonly INestedProviderManager<FilterProviderContext> _filterProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IInputFormattersProvider _inputFormattersProvider;
        private readonly IModelBinderProvider _modelBinderProvider;
        private readonly IModelValidatorProviderProvider _modelValidatorProviderProvider;
        private readonly IValueProviderFactoryProvider _valueProviderFactoryProvider;
        private readonly IScopedInstance<ActionBindingContext> _actionBindingContextAccessor;

        public ModuleActionInvokerProvider(
            IModuleFactory moduleFactory,
            INestedProviderManager<FilterProviderContext> filterProvider,
            IInputFormattersProvider inputFormattersProvider,
            IModelBinderProvider modelBinderProvider,
            IModelValidatorProviderProvider modelValidatorProviderProvider,
            IValueProviderFactoryProvider valueProviderFactoryProvider,
            IScopedInstance<ActionBindingContext> actionBindingContextAccessor,
            IServiceProvider serviceProvider)
        {
            _moduleFactory = moduleFactory;
            _filterProvider = filterProvider;
            _inputFormattersProvider = inputFormattersProvider;
            _modelBinderProvider = modelBinderProvider;
            _modelValidatorProviderProvider = modelValidatorProviderProvider;
            _valueProviderFactoryProvider = valueProviderFactoryProvider;
            _actionBindingContextAccessor = actionBindingContextAccessor;
            _serviceProvider = serviceProvider;
        }


        public int Order { get { return 0; } }

        public void Invoke(ActionInvokerProviderContext context, Action callNext)
        {
            var actionDescriptor = context.ActionContext.ActionDescriptor as ModuleActionDescriptor;

            if (actionDescriptor != null)
            {
                context.Result = new ModuleActionInvoker(
                    context.ActionContext,
                    _filterProvider,
                    _moduleFactory,
                    actionDescriptor,
                    _inputFormattersProvider,
                    _modelBinderProvider, 
                    _modelValidatorProviderProvider,
                    _valueProviderFactoryProvider,
                    _actionBindingContextAccessor);
            }

            callNext();
        }
    }
}