// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;

namespace Mvc.GenericControllers.Controllers
{
    // Even though 'sprocket' is one of the entity types in the system, we won't use
    // GenericController<Sprocket> - this class takes precedence.
    public class SprocketController : Controller
    {
        public IActionResult Index()
        {
            return Content($"Hello from a non-generic {GetType().Name}.");
        }
    }
}
