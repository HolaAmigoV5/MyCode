﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.AspNetCore.Razor.Language;
using Xunit;
using static Microsoft.AspNetCore.Razor.Hosting.TestRazorCompiledItem;

namespace Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
{
    public class ChecksumValidatorTest
    {
        private VirtualRazorProjectFileSystem ProjectFileSystem { get; } = new VirtualRazorProjectFileSystem();

        [Fact]
        public void IsRecompilationSupported_NoChecksumAttributes_ReturnsFalse()
        {
            // Arrange
            var item = new TestRazorCompiledItem(typeof(string), "mvc.1.0.view", "/Views/Home/Index.cstml", new object[] { });

            // Act
            var result = ChecksumValidator.IsRecompilationSupported(item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsRecompilationSupported_NoPrimaryChecksumAttribute_ReturnsFalse()
        {
            // Arrange
            var item = new TestRazorCompiledItem(typeof(string), "mvc.1.0.view", "/Views/Home/Index.cstml", new object[]
            {
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some import"), "/Views/Home/_ViewImports.cstml"),
            });

            // Act
            var result = ChecksumValidator.IsRecompilationSupported(item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsRecompilationSupported_HasPrimaryChecksumAttribute_ReturnsTrue()
        {
            // Arrange
            var item = new TestRazorCompiledItem(typeof(string), "mvc.1.0.view", "/Views/Home/Index.cstml", new object[]
            {
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some import"), "/Views/Home/_ViewImports.cstml"),
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some content"), "/Views/Home/Index.cstml"),
            });

            // Act
            var result = ChecksumValidator.IsRecompilationSupported(item);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsItemValid_NoChecksumAttributes_ReturnsTrue()
        {
            // Arrange
            var item = new TestRazorCompiledItem(typeof(string), "mvc.1.0.view", "/Views/Home/Index.cstml", new object[] { });

            // Act
            var result = ChecksumValidator.IsItemValid(ProjectFileSystem, item);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsItemValid_NoPrimaryChecksumAttribute_ReturnsTrue()
        {
            // Arrange
            var item = new TestRazorCompiledItem(typeof(string), "mvc.1.0.view", "/Views/Home/Index.cstml", new object[]
            {
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some import"), "/Views/Home/_ViewImports.cstml"),
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some content"), "/Views/Home/About.cstml"),
            });

            // Act
            var result = ChecksumValidator.IsItemValid(ProjectFileSystem, item);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsItemValid_PrimaryFileDoesNotExist_ReturnsTrue()
        {
            // Arrange
            var item = new TestRazorCompiledItem(typeof(string), "mvc.1.0.view", "/Views/Home/Index.cstml", new object[]
            {
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some import"), "/Views/Home/_ViewImports.cstml"),
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some content"), "/Views/Home/Index.cstml"),
            });

            ProjectFileSystem.Add(new TestRazorProjectItem("/Views/Home/_ViewImports.cstml", "dkdkfkdf")); // This will be ignored

            // Act
            var result = ChecksumValidator.IsItemValid(ProjectFileSystem, item);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsItemValid_PrimaryFileExistsButDoesNotMatch_ReturnsFalse()
        {
            // Arrange
            var item = new TestRazorCompiledItem(typeof(string), "mvc.1.0.view", "/Views/Home/Index.cstml", new object[]
            {
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some import"), "/Views/Home/_ViewImports.cstml"),
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some content"), "/Views/Home/Index.cstml"),
            });

            ProjectFileSystem.Add(new TestRazorProjectItem("/Views/Home/Index.cstml", "other content"));

            // Act
            var result = ChecksumValidator.IsItemValid(ProjectFileSystem, item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsItemValid_ImportFileDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var item = new TestRazorCompiledItem(typeof(string), "mvc.1.0.view", "/Views/Home/Index.cstml", new object[]
            {
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some import"), "/Views/Home/_ViewImports.cstml"),
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some content"), "/Views/Home/Index.cstml"),
            });

            ProjectFileSystem.Add(new TestRazorProjectItem("/Views/Home/Index.cstml", "some content"));

            // Act
            var result = ChecksumValidator.IsItemValid(ProjectFileSystem, item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsItemValid_ImportFileExistsButDoesNotMatch_ReturnsFalse()
        {
            // Arrange
            var item = new TestRazorCompiledItem(typeof(string), "mvc.1.0.view", "/Views/Home/Index.cstml", new object[]
            {
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some import"), "/Views/Home/_ViewImports.cstml"),
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some content"), "/Views/Home/Index.cstml"),
            });

            ProjectFileSystem.Add(new TestRazorProjectItem("/Views/Home/Index.cstml", "some content"));
            ProjectFileSystem.Add(new TestRazorProjectItem("/Views/Home/_ViewImports.cstml", "some other import"));

            // Act
            var result = ChecksumValidator.IsItemValid(ProjectFileSystem, item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsItemValid_AllFilesMatch_ReturnsTrue()
        {
            // Arrange
            var item = new TestRazorCompiledItem(typeof(string), "mvc.1.0.view", "/Views/Home/Index.cstml", new object[]
            {
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some other import"), "/Views/_ViewImports.cstml"),
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some import"), "/Views/Home/_ViewImports.cstml"),
                new RazorSourceChecksumAttribute("SHA1", GetChecksum("some content"), "/Views/Home/Index.cstml"),
            });

            ProjectFileSystem.Add(new TestRazorProjectItem("/Views/Home/Index.cstml", "some content"));
            ProjectFileSystem.Add(new TestRazorProjectItem("/Views/Home/_ViewImports.cstml", "some import"));
            ProjectFileSystem.Add(new TestRazorProjectItem("/Views/_ViewImports.cstml", "some other import"));

            // Act
            var result = ChecksumValidator.IsItemValid(ProjectFileSystem, item);

            // Assert
            Assert.True(result);
        }
    }
}
