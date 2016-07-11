// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using Mvc.RenderViewToString;
using Xunit;

namespace EntropyTests
{
    public class RenderViewToStringTest
    {
        [Fact]
        public void RenderView_ReturnsContents()
        {
            // Arrange
            var resourceFile = "RenderViewToString.html";
            var applicationBasePath = TestServices.GetApplicationDirectory(
                typeof(Program).GetTypeInfo().Assembly.GetName().Name);

            // Act
            var actual = Program.RenderView(applicationBasePath);

            // Assert
            RazorEmbeddedResource.AssertContent(resourceFile, actual);
        }
    }
}
