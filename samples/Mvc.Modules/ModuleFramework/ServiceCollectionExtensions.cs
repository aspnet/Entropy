using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModuleFramework;

namespace Microsoft.Framework.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddModules(this IServiceCollection services)
        {
            services.AddTransient<INestedProvider<ActionDescriptorProviderContext>, ModuleActionDescriptorProvider>();
            services.AddTransient<INestedProvider<ActionInvokerProviderContext>, ModuleActionInvokerProvider>();
            services.AddTransient<IModuleFactory, ModuleFactory>();
        }
    }
}