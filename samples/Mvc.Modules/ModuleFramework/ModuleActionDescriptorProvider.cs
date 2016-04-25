using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.ModuleFramework
{
    public class ModuleActionDescriptorProvider : IActionDescriptorProvider
    {
        private readonly ApplicationPartManager _applicationPartManager;
        private readonly IServiceProvider _services;

        public ModuleActionDescriptorProvider(
            ApplicationPartManager applicationPartManager,
            IServiceProvider services)
        {
            _applicationPartManager = applicationPartManager;
            _services = services;
        }

        public int Order { get { return 0; } }

        public void OnProvidersExecuting(ActionDescriptorProviderContext context)
        {
            foreach (var part in _applicationPartManager.ApplicationParts.OfType<IApplicationPartTypeProvider>())
            {
                foreach (var typeInfo in part.Types)
                {
                    var type = typeInfo.AsType();
                    if (typeInfo.IsClass &&
                        !typeInfo.IsAbstract &&
                        !typeInfo.ContainsGenericParameters &&
                        typeof(MvcModule).IsAssignableFrom(type) &&
                        type != typeof(MvcModule))
                    {
                        foreach (var action in GetActions(type))
                        {
                            context.Results.Add(action);
                        }
                    }
                }
            }
        }

        public void OnProvidersExecuted(ActionDescriptorProviderContext context)
        {
        }

        private IEnumerable<ActionDescriptor> GetActions(Type type)
        {
            var prototype = (MvcModule)ActivatorUtilities.CreateInstance(_services, type);

            int i = 0;
            foreach (var action in prototype.Actions)
            {
                var filters = type.GetTypeInfo()
                    .GetCustomAttributes(inherit: true)
                    .OfType<IFilterMetadata>()
                    .Select(filter => new FilterDescriptor(filter, FilterScope.Controller))
                    .OrderBy(d => d, FilterDescriptorOrderComparer.Comparer)
                    .ToList();

                yield return new ModuleActionDescriptor()
                {
                    FilterDescriptors = filters,
                    Index = i++,
                    ActionConstraints = new List<IActionConstraintMetadata>()
                    {
                        new HttpMethodActionConstraint(new [] { action.Verb }),
                    },
                    ModuleType = type,
                    Parameters = new List<ParameterDescriptor>(), // No Parameter support in this sample
                    RouteValues =
                    {
                        { "module", "true"}, // only match a 'module' route
                        { "modulepath", action.Path == "/" ? string.Empty : action.Path.Substring(1)}
                    }
                };
            }
        }
    }
}