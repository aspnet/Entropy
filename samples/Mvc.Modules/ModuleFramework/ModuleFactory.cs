using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Mvc.ModuleFramework
{
    public class ModuleFactory : IModuleFactory
    {
        private readonly IServiceProvider _services;

        public ModuleFactory(IServiceProvider services)
        {
            _services = services;
        }

        public object CreateModule(ActionContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ModuleActionDescriptor;
            if (actionDescriptor == null)
            {
                throw new InvalidOperationException("Not a module.");
            }

            var module = ActivatorUtilities.CreateInstance(_services, actionDescriptor.ModuleType);
            return module;
        }

        public void ReleaseModule(object module)
        {
            var disposable = module as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}