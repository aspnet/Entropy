// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace Entropy.FunctionalTests
{
    public class CustomRazorPageHandlerTest : IClassFixture<SampleTestFixture<Mvc.CustomRazorPageHandlers.Startup>>
    {
        public CustomRazorPageHandlerTest(SampleTestFixture<Mvc.CustomRazorPageHandlers.Startup> fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task SimpleGet_Works()
        {
            // Arrange
            var expected = "Hello Hello World!";

            // Act
            var response = await Client.GetStringAsync(string.Empty);

            // Assert
            Assert.Contains(expected, response);
        }

        [Fact]
        public async Task CustomizedHandler_Works()
        {
            // Arrange
            var expected = "Delete handler invoked";
            var request = new HttpRequestMessage(HttpMethod.Post, "/Delete");
            await AddAntiforgeryHeadersAsync(request);

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains(expected, responseContent);
        }

        private async Task AddAntiforgeryHeadersAsync(HttpRequestMessage request)
        {
            var getResponse = await Client.GetAsync(request.RequestUri);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var getResponseBody = await getResponse.Content.ReadAsStringAsync();
            var formToken = RetrieveAntiforgeryToken(getResponseBody);
            var cookie = RetrieveAntiforgeryCookie(getResponse);
 
            request.Headers.Add("Cookie", cookie.key + "=" + cookie.value);
            request.Headers.Add("RequestVerificationToken", formToken);
         }

        public static string RetrieveAntiforgeryToken(string htmlContent)
        {
            htmlContent = "<Root>" + htmlContent + "</Root>";
            var reader = new StringReader(htmlContent);
            var htmlDocument = XDocument.Load(reader);
            var antiforgeryField = htmlDocument
                .Descendants("form")
                .Descendants("input")
                .FirstOrDefault(input => input.Attribute("name").Value == "__RequestVerificationToken");

            if (antiforgeryField == null)
            {
                throw new Exception($"Antiforgery token could not be located in {htmlContent}.");
            }

            return antiforgeryField.Attribute("value").Value;
        }

        public static (string key, string value) RetrieveAntiforgeryCookie(HttpResponseMessage response)
        {
            var setCookieArray = response.Headers.GetValues("Set-Cookie").ToArray();
            var cookie = setCookieArray[0].Split(';').First().Split('=');
            return (cookie[0], cookie[1]);
        }
    }
}
