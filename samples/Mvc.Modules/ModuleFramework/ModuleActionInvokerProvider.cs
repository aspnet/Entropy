using System;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    /// <summary>
    /// Summary description for ModuleActionInvokerProvider
    /// </summary>
    public class ModuleActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IModuleFactory _moduleFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IActionBindingContextProvider _bindingProvider;
        private readonly INestedProviderManager<FilterProviderContext> _filterProvider;

        public ModuleActionInvokerProvider(
            IModuleFactory moduleFactory,
            IActionBindingContextProvider bindingProvider,
            INestedProviderManager<FilterProviderContext> filterProvider,
            IServiceProvider serviceProvider)
        {
            _moduleFactory = moduleFactory;
            _bindingProvider = bindingProvider;
            _filterProvider = filterProvider;
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
                    _bindingProvider,
                    _filterProvider,
                    _moduleFactory,
                    actionDescriptor);
            }

            callNext();
        }
    }
}