using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace LocalizedRazorPages
{
    public class PageCultureConvention : IPageRouteModelConvention
    {
        public void Apply(PageRouteModel model)
        {
            var path = model.ViewEnginePath;
            var lastSeparator = path.LastIndexOf('/');
            Debug.Assert(lastSeparator != -1);
            var lastDot = path.LastIndexOf('.', path.Length - 1, path.Length - lastSeparator);

            // /SomeFolder/MyPage.fr-FR -> fr-FR
            if (lastDot != -1)
            {
                var cultureName = path.Substring(lastDot + 1);
                var constraint = new CultureConstraint(new CultureInfo(cultureName));
                for (var i = model.Selectors.Count - 1; i >= 0; i--)
                {
                    var selector = model.Selectors[i];

                    selector.ActionConstraints.Add(constraint);
                    var template = selector.AttributeRouteModel.Template;
                    template = template.Substring(0, lastDot - 1);

                    selector.AttributeRouteModel.Template = template;
                    var fileName = template.Substring(lastSeparator);
                    
                    if (fileName == "Index")
                    {
                        selector.AttributeRouteModel.SuppressLinkGeneration = true;

                        var indexSelector = new SelectorModel(selector);

                        template = selector.AttributeRouteModel.Template.Substring(0, lastSeparator);
                        indexSelector.AttributeRouteModel.Template = template;
                        model.Selectors.Add(indexSelector);
                    }
                }
            }
        }

        private class CultureConstraint : IActionConstraint
        {
            private readonly CultureInfo _culture;

            public CultureConstraint(CultureInfo culture)
            {
                _culture = culture;
            }

            public int Order => 0;

            public bool Accept(ActionConstraintContext context)
            {
                var feature = context.RouteContext.HttpContext.Features.Get<IRequestCultureFeature>();
                if (feature == null)
                {
                    // The page is associated with a culture but the request is not.
                    return false;
                }

                return string.Equals(feature.RequestCulture.Culture.Name, _culture.Name, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
