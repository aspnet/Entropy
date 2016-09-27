// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Mvc.LocalizationSample.Web
{
    public class MinLengthSixAttribute : ValidationAttribute
    {
        public MinLengthSixAttribute()
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
        }

        public override bool IsValid(object value)
        {
            var stringValue = value as string;
            if (stringValue == null)
            {
                return false;
            }

            return stringValue.Length >= 6;
        }
    }
}
