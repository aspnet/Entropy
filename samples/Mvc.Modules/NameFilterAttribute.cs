// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModuleFramework;

namespace Mvc.Modules
{
    public class NameFilterAttribute : Attribute, IModuleFilter
    {
        public void OnModuleExecuted(ModuleExecutedContext context)
        {
        }

        public void OnModuleExecuting(ModuleExecutingContext context)
        {
            var names = context.HttpContext.Request.Query["name"];
            if (names.Any())
            {
                var module = context.Module;
                module.ViewData.Add("name", names.First());
            }
        }
    }
}