// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Mvc.LocalizationSample.Web.Models
{
    public class User
    {
        [Display(Name = "User Name")]
        [MinLength(6, ErrorMessage = "Invalid {0}. {0} minimum length is {1}")]
        public string Name { get; set; }

        [Display(Name = "Product")]
        public Product Product { get; set; }
    }
}
