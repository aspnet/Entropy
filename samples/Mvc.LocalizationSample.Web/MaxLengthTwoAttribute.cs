// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Mvc.LocalizationSample.Web
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class MaxLengthTwoAttribute : SelfContainedValidationAttribute
    {
        private const string _errorMessage = "MaxLengthTwo";

        public MaxLengthTwoAttribute()
        {
            ErrorMessage = _errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            var result = base.IsValid(value, validationContext);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                result.ErrorMessage = GetErrorMessage(validationContext);
            }

            return result;
        }

        public override bool IsValid(object value)
        {
            var stringValue = value as string;
            if (stringValue == null)
            {
                return false;
            }

            return stringValue.Length < 2;
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");

            var errorMessage = GetErrorMessage(context);

            MergeAttribute(context.Attributes, "data-val-maxlength", errorMessage);
            MergeAttribute(context.Attributes, "data-val-maxlength-max", "2");
        }

        private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }
    }
}
