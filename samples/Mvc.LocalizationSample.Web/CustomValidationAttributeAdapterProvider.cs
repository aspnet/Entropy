// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace Mvc.LocalizationSample.Web
{
    public class CustomValidationAttributeAdapterProvider
        : ValidationAttributeAdapterProvider, IValidationAttributeAdapterProvider
    {
        public CustomValidationAttributeAdapterProvider()
        {
        }

        IAttributeAdapter IValidationAttributeAdapterProvider.GetAttributeAdapter(
            ValidationAttribute attribute,
            IStringLocalizer stringLocalizer)
        {
            var adapter = base.GetAttributeAdapter(attribute, stringLocalizer);

            if (adapter == null)
            {
                var minLengthSix = attribute as MinLengthSixAttribute;
                if (minLengthSix != null)
                {
                    adapter = new MinLengthSixAttributeAdapter(minLengthSix, stringLocalizer);
                }
            }

            return adapter;
        }
    }
}