// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Mvc.LocalizationSample.Web.Models
{
    public class Product
    {
        [Display(Name = "Product Name")]
        [Required(ErrorMessage = "{0} is invalid")]
        public string ProductName { get; set; }

        [Display(Name = "Description")]
        [MinLengthSix(ErrorMessage = "{0} must be at least 6 characters")]
        public string ProductDescription { get; set; }

        [Display(Name = "Comment")]
        [MaxLengthTwo]
        public string ProductComment { get; set; }
    }
}
