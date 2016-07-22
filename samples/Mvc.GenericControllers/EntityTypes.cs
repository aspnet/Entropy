// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;
using Mvc.GenericControllers.Models;

namespace Mvc.GenericControllers
{
    // This is fake for the sample. Assume these were coming from a DbContext or from a configuration
    // of some kind.
    public static class EntityTypes
    {
        public static IReadOnlyList<TypeInfo> Types => new List<TypeInfo>()
        {
            typeof(Sprocket).GetTypeInfo(),
            typeof(Widget).GetTypeInfo(),
        };
    }
}
