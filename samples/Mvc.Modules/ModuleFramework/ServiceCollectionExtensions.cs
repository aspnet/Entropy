using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.ModuleFramework;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddModules(this IServiceCollection services)
        {
            services.AddTransient<IActionDescriptorProvider, ModuleActionDescriptorProvider>();
            services.AddTransient<IActionInvokerProvider, ModuleActionInvokerProvider>();
            services.AddTransient<IModuleFactory, ModuleFactory>();
        }
    }
}