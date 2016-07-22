// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Mvc.GenericControllers.Controllers
{
    public class HomeController : Controller
    {
        private readonly IActionDescriptorCollectionProvider _actions;

        public HomeController(IActionDescriptorCollectionProvider actions)
        {
            _actions = actions;
        }

        public IActionResult Index()
        {
            // This is just here for demonstration purposes, it returns a JSON dump of some data
            // about all of the controllers and actions in the application.
            var actions = _actions.ActionDescriptors.Items.Select(a =>
            {
                return new
                {
                    DisplayName = a.DisplayName,
                    RouteValues = a.RouteValues,
                };
            });
            return Ok(actions.ToArray());
        }
    }
}
