// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Localization.CustomResourceManager
{
    /// <summary>
    /// An <see cref="IStringLocalizerFactory"/> that creates instances of <see cref="ResourceManagerStringLocalizer"/>
    /// and will properly handle the resources of ClassLibraries.
    /// </summary>
    public class ClassLibraryStringLocalizerFactory : ResourceManagerStringLocalizerFactory
    {
        private IReadOnlyDictionary<string, string> _resourcePathMappings;

        public ClassLibraryStringLocalizerFactory(
            IOptions<LocalizationOptions> localizationOptions,
            IOptions<ClassLibraryLocalizationOptions> classLibraryLocalizationOptions)
                : base(localizationOptions)
        {
            _resourcePathMappings = classLibraryLocalizationOptions.Value.ResourcePaths;
        }

        protected override string GetResourcePrefix(TypeInfo typeInfo)
        {
            var assemblyName = typeInfo.Assembly.GetName().Name;
            return GetResourcePrefix(typeInfo, assemblyName, GetResourcePath(assemblyName));
        }

        protected override string GetResourcePrefix(TypeInfo typeInfo, string baseNamespace, string resourcesRelativePath)
        {
            var assemblyName = new AssemblyName(typeInfo.Assembly.FullName);
            return base.GetResourcePrefix(typeInfo, baseNamespace, GetResourcePath(assemblyName.Name));
        }

        private string GetResourcePath(string assemblyName)
        {
            string resourcePath;
            if (!_resourcePathMappings.TryGetValue(assemblyName, out resourcePath))
            {
                throw new KeyNotFoundException("Attempted to access an assembly which doesn't have a resourcePath set.");
            }

            if (!string.IsNullOrEmpty(resourcePath))
            {
                resourcePath = resourcePath.Replace(Path.AltDirectorySeparatorChar, '.')
                    .Replace(Path.DirectorySeparatorChar, '.') + ".";
            }

            return resourcePath;
        }
    }
}
