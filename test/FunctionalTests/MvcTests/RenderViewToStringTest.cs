// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using System.Threading.Tasks;
using Mvc.RenderViewToString;
using Xunit;

namespace EntropyTests
{
    public class RenderViewToStringTest
    {
        [Fact]
        public async Task RenderView_ReturnsContents()
        {
            // Arrange
            var resourceFile = "RenderViewToString.html";
            var applicationBasePath = TestServices.GetApplicationDirectory(
                typeof(Program).GetTypeInfo().Assembly.GetName().Name);
            var serviceScopeFactory = Program.InitializeServices(applicationBasePath);

            // Act
            var actual = await Program.RenderViewAsync(serviceScopeFactory);

            // Assert
            RazorEmbeddedResource.AssertContent(resourceFile, actual);
        }

        [Fact]
        public async Task RenderView_WorksForMultipleViewsBeingRenderedConcurrently()
        {
            // Arrange
            var resourceFile = "RenderViewToString.html";
            var applicationBasePath = TestServices.GetApplicationDirectory(
                typeof(Program).GetTypeInfo().Assembly.GetName().Name);
            var serviceScopeFactory = Program.InitializeServices(applicationBasePath);

            // Act
            var task1 = Program.RenderViewAsync(serviceScopeFactory);
            var task2 = Program.RenderViewAsync(serviceScopeFactory);
            await Task.WhenAll(task1, task2);

            // Assert
            RazorEmbeddedResource.AssertContent(resourceFile, task1.Result);
            RazorEmbeddedResource.AssertContent(resourceFile, task2.Result);
        }
    }
}
