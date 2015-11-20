using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class ModuleActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IModuleFactory _moduleFactory;
        private readonly IReadOnlyList<IFilterProvider> _filterProviders;
        private readonly IServiceProvider _serviceProvider;
        private readonly IReadOnlyList<IInputFormatter> _inputFormatters;
        private readonly IReadOnlyList<IOutputFormatter> _outputFormatters;
        private readonly IReadOnlyList<IModelBinder> _modelBinders;
        private readonly IReadOnlyList<IModelValidatorProvider> _modelValidatorProviders;
        private readonly IReadOnlyList<IValueProviderFactory> _valueProviderFactories;
        private readonly IActionBindingContextAccessor _actionBindingContextAccessor;
        private readonly IOptions<MvcOptions> _optionsAccessor;
        private readonly ILogger _logger;
        private readonly DiagnosticSource _diagnostic;

        public ModuleActionInvokerProvider(
            IModuleFactory moduleFactory,
            IEnumerable<IFilterProvider> filterProviders,
            IReadOnlyList<IInputFormatter> inputFormatters,
            IReadOnlyList<IOutputFormatter> outputFormatters,
            IReadOnlyList<IModelBinder> modelBinders,
            IReadOnlyList<IModelValidatorProvider> modelValidatorProviders,
            IReadOnlyList<IValueProviderFactory> valueProviderFactories,
            IActionBindingContextAccessor actionBindingContextAccessor,
            IOptions<MvcOptions> optionsAccessor,
            ILogger logger,
            DiagnosticSource diagnostic,
            IServiceProvider serviceProvider)
        {
            _moduleFactory = moduleFactory;
            _filterProviders = filterProviders.OrderBy(p => p.Order).ToList();
            _inputFormatters = inputFormatters;
            _outputFormatters = outputFormatters;
            _modelBinders = modelBinders;
            _modelValidatorProviders = modelValidatorProviders;
            _valueProviderFactories = valueProviderFactories;
            _actionBindingContextAccessor = actionBindingContextAccessor;
            _optionsAccessor = optionsAccessor;
            _logger = logger;
            _diagnostic = diagnostic;
            _serviceProvider = serviceProvider;
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