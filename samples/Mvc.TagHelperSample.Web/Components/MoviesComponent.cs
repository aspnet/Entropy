// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using TagHelperSample.Web.Services;

namespace TagHelperSample.Web.Components
{
    [ViewComponent(Name = "Movies")]
    public class MoviesComponent : ViewComponent
    {
        private readonly MoviesService _moviesService;

        public MoviesComponent(MoviesService moviesService)
        {
            _moviesService = moviesService;
        }

        public IViewComponentResult Invoke(string movieName)
        {
            return Content(_moviesService.GetCriticsQuote(movieName));
        }
    }
}