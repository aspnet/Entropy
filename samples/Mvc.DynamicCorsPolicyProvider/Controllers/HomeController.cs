// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Mvc.DynamicCorsPolicyProvider.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("Home.Index");
        }

        [EnableCors("policy1")]
        public IActionResult Contact()
        {
            return Content("Hello");
        }
    }
}
