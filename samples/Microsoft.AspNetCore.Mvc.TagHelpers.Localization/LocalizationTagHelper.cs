// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.TagHelpers
{
    [HtmlTargetElement("*", Attributes = "asp-loc", TagStructure = TagStructure.Unspecified)]
    [HtmlTargetElement("*", Attributes = "asp-loc-*", TagStructure = TagStructure.Unspecified)]
    public class LocalizationTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-localizer")]
        public IHtmlLocalizer Localizer { get; set; }

        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var localizer = Localizer ?? GetViewLocalizer();

            var aspLocAttr = output.Attributes["asp-loc"];

            if (aspLocAttr != null)
            {
                var resourceKey = aspLocAttr.ValueStyle == HtmlAttributeValueStyle.Minimized
                    ? (await output.GetChildContentAsync()).GetContent()
                    : aspLocAttr.Value.ToString();
                output.Content.SetHtmlContent(localizer.GetHtml(resourceKey));
                output.Attributes.Remove(aspLocAttr);
            }

            var localizeAttributes = output.Attributes.Where(attr => attr.Name.StartsWith("asp-loc-", StringComparison.OrdinalIgnoreCase)).ToList();

            for (var i = 0; i < localizeAttributes.Count; i++)
            {
                var attribute = localizeAttributes[i];
                var attributeToLocalizeIndex = output.Attributes.IndexOfName(attribute.Name.Substring("asp-loc-".Length));
                if (attributeToLocalizeIndex != -1)
                {
                    var attributeToLocalize = output.Attributes[attributeToLocalizeIndex];
                    var resourceKey = attribute.ValueStyle == HtmlAttributeValueStyle.Minimized
                        ? attributeToLocalize.Value.ToString()
                        : attribute.Value.ToString();
                    var value = localizer.GetHtml(resourceKey);
                    output.Attributes[attributeToLocalizeIndex] = new TagHelperAttribute(attributeToLocalize.Name, value, attributeToLocalize.ValueStyle);
                }
                output.Attributes.Remove(attribute);
            }
        }

        private IHtmlLocalizer GetViewLocalizer()
        {
            var localizer = ViewContext.HttpContext.RequestServices.GetService<IViewLocalizer>();

            (localizer as IViewContextAware)?.Contextualize(ViewContext);

            return localizer;
        }
    }
}
