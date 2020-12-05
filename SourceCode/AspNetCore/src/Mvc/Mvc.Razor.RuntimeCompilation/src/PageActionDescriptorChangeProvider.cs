﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
{
    internal class PageActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        private readonly RuntimeCompilationFileProvider _fileProvider;
        private readonly string[] _searchPatterns;
        private readonly string[] _additionalFilesToTrack;

        public PageActionDescriptorChangeProvider(
            RazorProjectEngine projectEngine,
            RuntimeCompilationFileProvider fileProvider,
            IOptions<RazorPagesOptions> razorPagesOptions)
        {
            if (projectEngine == null)
            {
                throw new ArgumentNullException(nameof(projectEngine));
            }

            if (fileProvider == null)
            {
                throw new ArgumentNullException(nameof(fileProvider));
            }

            if (razorPagesOptions == null)
            {
                throw new ArgumentNullException(nameof(razorPagesOptions));
            }

            _fileProvider = fileProvider;

            var rootDirectory = razorPagesOptions.Value.RootDirectory;
            Debug.Assert(!string.IsNullOrEmpty(rootDirectory));
            rootDirectory = rootDirectory.TrimEnd('/');

            // Search pattern that matches all cshtml files under the Pages RootDirectory
            var pagesRootSearchPattern = rootDirectory + "/**/*.cshtml";

            // Search pattern that matches all cshtml files under the Pages AreaRootDirectory
            var areaRootSearchPattern = "/Areas/**/*.cshtml";

            _searchPatterns = new[]
            {
                pagesRootSearchPattern,
                areaRootSearchPattern
            };

            // pagesRootSearchPattern will miss _ViewImports outside the RootDirectory despite these influencing
            // compilation. e.g. when RootDirectory = /Dir1/Dir2, the search pattern will ignore changes to 
            // [/_ViewImports.cshtml, /Dir1/_ViewImports.cshtml]. We need to additionally account for these.
            var importFeatures = projectEngine.ProjectFeatures.OfType<IImportProjectFeature>().ToArray();
            var fileAtPagesRoot = projectEngine.FileSystem.GetItem(rootDirectory + "/Index.cshtml");

            _additionalFilesToTrack = GetImports(importFeatures, fileAtPagesRoot);
        }

        public IChangeToken GetChangeToken()
        {
            var fileProvider = _fileProvider.FileProvider;

            var changeTokens = new IChangeToken[_additionalFilesToTrack.Length + _searchPatterns.Length];
            for (var i = 0; i < _additionalFilesToTrack.Length; i++)
            {
                changeTokens[i] = fileProvider.Watch(_additionalFilesToTrack[i]);
            }

            for (var i = 0; i < _searchPatterns.Length; i++)
            {
                var wildcardChangeToken = fileProvider.Watch(_searchPatterns[i]);
                changeTokens[_additionalFilesToTrack.Length + i] = wildcardChangeToken;
            }

            return new CompositeChangeToken(changeTokens);
        }

        private static string[] GetImports(
            IImportProjectFeature[] importFeatures,
            RazorProjectItem file)
        {
            return importFeatures
                .SelectMany(f => f.GetImports(file))
                .Where(f => f.FilePath != null)
                .Select(f => f.FilePath)
                .ToArray();
        }
    }
}