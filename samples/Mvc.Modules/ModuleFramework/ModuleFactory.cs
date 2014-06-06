using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Framework.DependencyInjection;
using System;

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class ModuleFactory : IModuleFactory
    {
        private readonly IServiceProvider _services;
        private readonly ITypeActivator _activator;

        public ModuleFactory(IServiceProvider services, ITypeActivator activator)
        {
            _services = services;
            _activator = activator;
        }

        public object CreateModule(ActionContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ModuleActionDescriptor;
            if (actionDescriptor == null)
            {
                throw new InvalidOperationException("Not a module.");
            }

            var module = _activator.CreateInstance(_services, actionDescriptor.ModuleType);

            Injector.InjectProperty(module, "ActionContext", context);

            var viewData = new ViewDataDictionary(
                _services.GetService<IModelMetadataProvider>(),
                context.ModelState);
            Injector.InjectProperty(module, "ViewData", viewData);

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