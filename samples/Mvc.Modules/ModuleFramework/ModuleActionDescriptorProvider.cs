using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class ModuleActionDescriptorProvider : IActionDescriptorProvider
    {
        private readonly IControllerAssemblyProvider _assemblyProvider;
        private readonly IServiceProvider _services;
        private readonly ITypeActivator _typeActivator;

        public ModuleActionDescriptorProvider(
            IControllerAssemblyProvider assemblyProvider,
            IServiceProvider services,
            ITypeActivator typeActivator)
        {
            _assemblyProvider = assemblyProvider;
            _services = services;
            _typeActivator = typeActivator;
        }

        public int Order { get { return 0; } }

        public void Invoke(ActionDescriptorProviderContext context, Action callNext)
        {
            foreach (var assembly in _assemblyProvider.CandidateAssemblies)
            {
                foreach (var type in assembly.GetExportedTypes())
                {
                    var typeInfo = type.GetTypeInfo();
                    if (typeInfo.IsClass &&
                        !typeInfo.IsAbstract &&
                        !typeInfo.ContainsGenericParameters &&
                        typeof(MvcModule).IsAssignableFrom(type) && 
                        type != typeof(MvcModule))
                    {
                        context.Results.AddRange(GetActions(type));
                    }
                }
            }

            callNext();
        }

        private IEnumerable<ActionDescriptor> GetActions(Type type)
        {
            var prototype = (MvcModule)_typeActivator.CreateInstance(_services, type);

            int i = 0;
            foreach (var action in prototype.Actions)
            {
                var filters = type.GetTypeInfo()
                    .GetCustomAttributes(inherit: true)
                    .OfType<IFilter>()
                    .Select(filter => new FilterDescriptor(filter, FilterScope.Controller))
                    .OrderBy(d => d, FilterDescriptorOrderComparer.Comparer)
                    .ToList();

                RouteDataActionConstraint pathConstraint;
                if (action.Path == "/")
                {
                    pathConstraint = new RouteDataActionConstraint("modulepath", RouteKeyHandling.DenyKey);
                }
                else
                {
                    pathConstraint = new RouteDataActionConstraint("modulepath", action.Path.Substring(1));
                }

                yield return new ModuleActionDescriptor()
                {
                    FilterDescriptors = filters,
                    Index = i++,
                    MethodConstraints = new List<HttpMethodConstraint>()
                    {
                        new HttpMethodConstraint(new [] { action.Verb }),
                    },
                    ModuleType = type,
                    Parameters = new List<ParameterDescriptor>(), // No Parameter support in this sample
                    RouteConstraints = new List<RouteDataActionConstraint>()
                    {
                        new RouteDataActionConstraint("module", "true"), // only match a 'module' route
                        pathConstraint,
                    }
                };
            }
        }
    }
}