// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET461
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Testing;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Entropy.FunctionalTests
{
    public class LocalizationSampleTest : IClassFixture<SampleTestFixture<global::Mvc.LocalizationSample.Web.Startup>>
    {
        public LocalizationSampleTest(SampleTestFixture<global::Mvc.LocalizationSample.Web.Startup> fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }

        public static IEnumerable<object[]> LocalizationData
        {
            get
            {
                var expected1 =
@"<language-layout>en-gb-index
partial
mypartial
</language-layout>";

                yield return new[] { "en-GB", expected1 };

                var expected2 =
@"<fr-language-layout>fr-index
fr-partial
mypartial
</fr-language-layout>";
                yield return new[] { "fr", expected2 };

                if (!TestPlatformHelper.IsMono)
                {
                    // https://github.com/aspnet/Mvc/issues/2759
                    var expected3 =
 @"<language-layout>index
partial
mypartial
</language-layout>";
                    yield return new[] { "!-invalid-!", expected3 };
                }
            }
        }

        [Theory]
        [MemberData(nameof(LocalizationData))]
        public async Task Localization_SuffixViewName(string value, string expected)
        {
            // Arrange
            var cultureCookie = "c=" + value + "|uic=" + value;
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/");
            request.Headers.Add(
                "Cookie",
                new CookieHeaderValue(CookieRequestCultureProvider.DefaultCookieName, cultureCookie).ToString());

            // Act
            var response = await Client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(expected, body.Trim(), ignoreLineEndingDifferences: true);
        }

        public static IEnumerable<object[]> LocalizationResourceData
        {
            get
            {
                var expected1 =
                    "Hello there!!" + Environment.NewLine +
                    "Learn More" + Environment.NewLine +
                    "Hi John      ! You are in 2015 year and today is Thursday";

                yield return new[] { "en-GB", expected1 };

                var expected2 =
                    "Bonjour!" + Environment.NewLine +
                    "apprendre Encore Plus" + Environment.NewLine +
                    "Salut John      ! Vous êtes en 2015 an aujourd'hui est Thursday";
                yield return new[] { "fr", expected2 };
            }
        }

        [Theory]
        [MemberData(nameof(LocalizationResourceData))]
        public async Task Localization_Resources_ReturnExpectedValues(string value, string expected)
        {
            // Arrange
            var cultureCookie = "c=" + value + "|uic=" + value;
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/Home/Locpage");
            request.Headers.Add(
                "Cookie",
                new CookieHeaderValue(CookieRequestCultureProvider.DefaultCookieName, cultureCookie).ToString());

            // Act
            var response = await Client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(expected, body.Trim());
        }

        [Fact]
        public async Task Localization_InvalidModel_ValidationAttributes_ReturnsLocalizedErrorMessage()
        {
            // Arrange
            var expected =
@"<span class=""field-validation-error"" data-valmsg-for=""Name"" data-valmsg-replace=""true"">Nom d&#x27;utilisateur non valide. Longueur minimale de Nom d&#x27;utilisateur est 6</span>
<span class=""field-validation-error"" data-valmsg-for=""Product.ProductName"" data-valmsg-replace=""true"">Nom du produit est invalide</span>
<div class=""editor-label""><label for=""Name"">Nom d&#x27;utilisateur</label></div>
<div class=""editor-field""><input class=""input-validation-error text-box single-line"" data-val=""true"" data-val-minlength=""Nom d&#x27;utilisateur non valide. Longueur minimale de Nom d&#x27;utilisateur est 6"" data-val-minlength-min=""6"" id=""Name"" name=""Name"" type=""text"" value=""A"" /> <span class=""field-validation-error"" data-valmsg-for=""Name"" data-valmsg-replace=""true"">Nom d&#x27;utilisateur non valide. Longueur minimale de Nom d&#x27;utilisateur est 6</span></div>

<div class=""editor-label""><label for=""Product_ProductName"">Nom du produit</label></div>
<div class=""editor-field""><input class=""input-validation-error text-box single-line"" data-val=""true"" data-val-required=""Nom du produit est invalide"" id=""Product_ProductName"" name=""Product.ProductName"" type=""text"" value="""" /> <span class=""field-validation-error"" data-valmsg-for=""Product.ProductName"" data-valmsg-replace=""true"">Nom du produit est invalide</span></div>
<div class=""editor-label""><label for=""Product_ProductDescription"">Description</label></div>
<div class=""editor-field""><input class=""input-validation-error text-box single-line"" data-val=""true"" data-val-minlength=""Description doit &#xEA;tre d&#x27;au moins 6 caract&#xE8;res"" data-val-minlength-min=""6"" id=""Product_ProductDescription"" name=""Product.ProductDescription"" type=""text"" value="""" /> <span class=""field-validation-error"" data-valmsg-for=""Product.ProductDescription"" data-valmsg-replace=""true"">Description doit &#xEA;tre d&#x27;au moins 6 caract&#xE8;res</span></div>
<div class=""editor-label""><label for=""Product_ProductComment"">Commentaire</label></div>
<div class=""editor-field""><input class=""input-validation-error text-box single-line"" data-val=""true"" data-val-maxlength=""Commentaire doivent avoir une longueur d&#x27;au plus deux."" data-val-maxlength-max=""2"" id=""Product_ProductComment"" name=""Product.ProductComment"" type=""text"" value="""" /> <span class=""field-validation-error"" data-valmsg-for=""Product.ProductComment"" data-valmsg-replace=""true"">Commentaire doivent avoir une longueur d&#x27;au plus deux.</span></div>";

            var cultureCookie = "c=fr|uic=fr";
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/Home/GetInvalidUser");
            request.Headers.Add(
                "Cookie",
                new CookieHeaderValue(CookieRequestCultureProvider.DefaultCookieName, cultureCookie).ToString());

            // Act
            var response = await Client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(expected, body.Trim(), ignoreLineEndingDifferences: true);
        }
    }
}
#elif NETCOREAPP2_0
#else
#error Target framework needs to be updated
#endif