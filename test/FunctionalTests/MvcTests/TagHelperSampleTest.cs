// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace EntropyTests
{
    public class TagHelperSampleTest : IClassFixture<SampleTestFixture<TagHelperSample.Web.Startup>>
    {
        private static readonly object _writeLock = new object();

        public TagHelperSampleTest(SampleTestFixture<TagHelperSample.Web.Startup> fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task HomeController_Index_ReturnsExpectedContent()
        {
            // Arrange
            var resetResponse = await Client.PostAsync("http://localhost/Home/Reset", content: null);

            // Guard 1 (start from scratch)
            AssertRedirectsToHome(resetResponse);

            var createBillyContent = CreateUserFormContent("Billy", "2000-11-28", 0, "hello");
            var createBillyResponse = await Client.PostAsync("http://localhost/Home/Create", createBillyContent);

            // Guard 2 (ensure user 0 exists)
            AssertRedirectsToHome(createBillyResponse);

            var createBobbyContent = CreateUserFormContent("Bobby", "1999-10-27", 1, "howdy");
            var createBobbyResponse = await Client.PostAsync("http://localhost/Home/Create", createBobbyContent);

            // Guard 3 (ensure user 1 exists)
            AssertRedirectsToHome(createBobbyResponse);

            var expectedMediaType = MediaTypeHeaderValue.Parse("text/html; charset=utf-8");
            var resourceFile = "TagHelperSample.Web.Home.Index.html";

            // Act
            var response = await Client.GetAsync("http://localhost/");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMediaType, response.Content.Headers.ContentType);
            AssertContent(resourceFile, responseContent);
        }

        [Fact]
        public async Task HomeController_Index_ReturnsExpectedContent_AfterReset()
        {
            // Arrange
            var resetResponse = await Client.PostAsync("http://localhost/Home/Reset", content: null);

            // Guard (start from scratch)
            AssertRedirectsToHome(resetResponse);

            var expectedMediaType = MediaTypeHeaderValue.Parse("text/html; charset=utf-8");
            var resourceFile = "TagHelperSample.Web.Home.Index-Reset.html";

            // Act
            var response = await Client.GetAsync("http://localhost/");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMediaType, response.Content.Headers.ContentType);
            AssertContent(resourceFile, responseContent);
        }

        [Fact]
        public async Task HomeController_Create_Get_ReturnsSuccess()
        {
            // Act
            var response = await Client.GetAsync("http://localhost/Home/Create");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task HomeController_Create_Post_ReturnsSuccess()
        {
            // Arrange
            var createBillyContent = CreateUserFormContent("Billy", "2000-11-30", 0, "hello");

            // Act
            var response = await Client.PostAsync("http://localhost/Home/Create", createBillyContent);

            // Assert
            AssertRedirectsToHome(response);
        }

        [Fact]
        public async Task HomeController_Edit_Get_ReturnsSuccess()
        {
            // Arrange
            var createBillyContent = CreateUserFormContent("Billy", "2000-11-30", 0, "hello");
            var createBilly = await Client.PostAsync("http://localhost/Home/Create", createBillyContent);

            // Guard (ensure user 0 exists)
            AssertRedirectsToHome(createBilly);

            // Act
            var response = await Client.GetAsync("http://localhost/Home/Edit/0");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task HomeController_Edit_Post_ReturnsSuccess()
        {
            // Arrange
            var createBillyContent = CreateUserFormContent("Billy", "2000-11-30", 0, "hello");
            var createBilly = await Client.PostAsync("http://localhost/Home/Create", createBillyContent);

            // Guard (ensure user 0 exists)
            AssertRedirectsToHome(createBilly);

            var changeBillyContent = CreateUserFormContent("Bobby", "1999-11-30", 1, "howdy");

            // Act
            var changeBilly = await Client.PostAsync("http://localhost/Home/Edit/0", changeBillyContent);

            // Assert
            AssertRedirectsToHome(changeBilly);
        }

        [Fact]
        public async Task HomeController_Reset_ReturnsSuccess()
        {
            // Arrange and Act
            var response = await Client.PostAsync("http://localhost/Home/Reset", content: null);

            // Assert
            AssertRedirectsToHome(response);
        }

        public static TheoryData MoviesControllerPageData
        {
            get
            {
                return new TheoryData<Func<HttpClient, Task<HttpResponseMessage>>>
                {
                    async (client) =>  await client.GetAsync("http://localhost/Movies"),
                    async (client) =>  await client.PostAsync(
                        "http://localhost/Movies/UpdateMovieRatings",
                        new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>())),
                    async (client) =>  await client.PostAsync(
                        "http://localhost/Movies/UpdateCriticsQuotes",
                        new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>())),
                    async (client) =>
                    {
                        await client.PostAsync(
                            "http://localhost/Movies/UpdateCriticsQuotes",
                            new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>()));
                        await client.PostAsync(
                            "http://localhost/Movies/UpdateCriticsQuotes",
                            new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>()));
                        return await client.GetAsync("http://localhost/Movies/Index");
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(MoviesControllerPageData))]
        public async Task MoviesController_Pages_ReturnSuccess(Func<HttpClient, Task<HttpResponseMessage>> requestPage)
        {
            // Act
            var response = await requestPage(Client);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task TagHelperController_ConditionalComment_ReturnsExpectedContent()
        {
            // Arrange
            var expectedMediaType = MediaTypeHeaderValue.Parse("text/html; charset=utf-8");
            var resourceFile = "TagHelperSample.Web.TagHelper.ConditionalComment.html";

            // Act
            var response = await Client.GetAsync("http://localhost/TagHelper/ConditionalComment");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMediaType, response.Content.Headers.ContentType);
            AssertContent(resourceFile, responseContent);
        }

        private static void AssertRedirectsToHome(HttpResponseMessage response)
        {
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            var redirectLocations = response.Headers.GetValues("Location");
            var redirectsTo = Assert.Single(redirectLocations);
            Assert.Equal("/", redirectsTo, StringComparer.Ordinal);
        }

        private static HttpContent CreateUserFormContent(
            string name,
            string dateOfBirth,
            int yearsEmployeed,
            string blurb)
        {
            var form = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Name", name),
                new KeyValuePair<string, string>("DateOfBirth", dateOfBirth),
                new KeyValuePair<string, string>("YearsEmployeed", yearsEmployeed.ToString()),
                new KeyValuePair<string, string>("Blurb", blurb),
            };
            var content = new FormUrlEncodedContent(form);

            return content;
        }

        private static void AssertContent(string resourceFile, string actual)
        {
            var expected = GetResourceContent(resourceFile);
#if GENERATE_BASELINES
            // Normalize line endings to '\r\n' for comparison. This removes Environment.NewLine from the equation. Not
            // worth updating files just because we generate baselines on a different system.
            var normalizedPreviousContent = expected?.Replace("\r", "").Replace("\n", "\r\n");
            var normalizedContent = actual.Replace("\r", "").Replace("\n", "\r\n");

            if (!string.Equals(normalizedPreviousContent, normalizedContent, StringComparison.Ordinal))
            {
                var solutionRoot = TestServices.GetSolutionDirectory();
                var projectName = GetType().GetTypeInfo().Assembly.GetName().Name;
                var fullPath = Path.Combine(solutionRoot, "test", GetProjectName(), "resources", resourceFile);
                lock (_writeLock)
                {
                    // Write content to the file, creating it if necessary.
                    File.WriteAllText(fullPath, actual);
                }
            }
#else
            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
#endif
        }

        private static string GetResourceContent(string resourceFile)
        {
            resourceFile = $"{GetProjectName()}.resources.{resourceFile}";
            var assembly = typeof(TagHelperSampleTest).GetTypeInfo().Assembly;
            using (var streamReader = new StreamReader(assembly.GetManifestResourceStream(resourceFile)))
            {
                // Normalize line endings to '\r\n' (CRLF). This removes core.autocrlf, core.eol, core.safecrlf, and
                // .gitattributes from the equation and treats "\r\n" and "\n" as equivalent. Does not handle
                // some line endings like "\r" but otherwise ensures checksums and line mappings are consistent.
                return streamReader.ReadToEnd().Replace("\r", "").Replace("\n", "\r\n");
            }
        }

        private static string GetProjectName() => typeof(TagHelperSampleTest).GetTypeInfo().Assembly.GetName().Name;
    }
}