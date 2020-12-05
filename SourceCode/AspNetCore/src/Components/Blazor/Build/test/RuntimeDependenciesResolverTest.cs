// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNetCore.Blazor.Build.Test
{
    public class RuntimeDependenciesResolverTest
    {
        private readonly ITestOutputHelper _output;

        public RuntimeDependenciesResolverTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [ConditionalFact]
        [SkipOnHelix] // https://github.com/aspnet/AspNetCore/issues/6549
        public void FindsReferenceAssemblyGraph_ForStandaloneApp()
        {
            // Arrange
            var standaloneAppAssembly = typeof(StandaloneApp.Program).Assembly;
            var mainAssemblyLocation = standaloneAppAssembly.Location;
            var mainAssemblyDirectory = Path.GetDirectoryName(mainAssemblyLocation);
            // This list of hints is populated by MSBuild so it will be on the output
            // folder.
            var hintPaths = File.ReadAllLines(Path.Combine(
                mainAssemblyDirectory, "referenceHints.txt"));
            var bclLocations = File.ReadAllLines(Path.Combine(
                mainAssemblyDirectory, "bclLocations.txt"));
            var references = new[]
            {
                "Microsoft.AspNetCore.Blazor.dll",
                "Microsoft.AspNetCore.Components.Browser.dll",
                "Microsoft.AspNetCore.Components.dll",
                "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
                "Microsoft.Extensions.DependencyInjection.dll",
                "Microsoft.JSInterop.dll",
                "Mono.WebAssembly.Interop.dll",
            }.Select(a => hintPaths.Single(p => Path.GetFileName(p) == a))
            .ToArray();

            var expectedContents = new[]
            {
                /*
                 The current Mono WASM BCL forwards from netstandard.dll to various facade assemblies
                 in which small bits of implementation live, such as System.Xml.XPath.XDocument. So
                 if you reference netstandard, then you also reference System.Xml.XPath.XDocument.dll,
                 even though you're very unlikely to be calling it at runtime. That's why the following
                 list (for a very basic Blazor app) is longer than you'd expect.

                 These redundant references could be stripped out during publishing, but it's still
                 unfortunate that in development mode you'd see all these unexpected assemblies get
                 fetched from the server. We should try to get the Mono WASM BCL reorganized so that
                 all the implementation goes into mscorlib.dll, with the facade assemblies existing only
                 in case someone (or some 3rd party assembly) references them directly, but with their
                 implementations 100% forwarding to mscorlib.dll. Then in development you'd fetch far
                 fewer assemblies from the server, and during publishing, illink would remove all the
                 uncalled implementation code from mscorlib.dll anyway.
                 */
                "Microsoft.AspNetCore.Blazor.dll",
                "Microsoft.AspNetCore.Blazor.pdb",
                "Microsoft.AspNetCore.Components.Browser.dll",
                "Microsoft.AspNetCore.Components.Browser.pdb",
                "Microsoft.AspNetCore.Components.dll",
                "Microsoft.AspNetCore.Components.pdb",
                "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
                "Microsoft.Extensions.DependencyInjection.dll",
                "Microsoft.JSInterop.dll",
                "Mono.Security.dll",
                "Mono.WebAssembly.Interop.dll",
                "mscorlib.dll",
                "netstandard.dll",
                "StandaloneApp.dll",
                "StandaloneApp.pdb",
                "System.dll",
                "System.Collections.Concurrent.dll",
                "System.Collections.dll",
                "System.ComponentModel.Composition.dll",
                "System.ComponentModel.dll",
                "System.ComponentModel.Annotations.dll",
                "System.ComponentModel.DataAnnotations.dll",
                "System.Core.dll",
                "System.Data.dll",
                "System.Diagnostics.Debug.dll",
                "System.Diagnostics.Tracing.dll",
                "System.Drawing.Common.dll",
                "System.IO.Compression.dll",
                "System.IO.Compression.FileSystem.dll",
                "System.Linq.dll",
                "System.Linq.Expressions.dll",
                "System.Net.Http.dll",
                "System.Numerics.dll",
                "System.Reflection.Emit.ILGeneration.dll",
                "System.Reflection.Emit.Lightweight.dll",
                "System.Reflection.Primitives.dll",
                "System.Resources.ResourceManager.dll",
                "System.Runtime.dll",
                "System.Runtime.Extensions.dll",
                "System.Runtime.Serialization.dll",
                "System.ServiceModel.Internals.dll",
                "System.Threading.dll",
                "System.Transactions.dll",
                "System.Web.Services.dll",
                "System.Xml.dll",
                "System.Xml.Linq.dll",
            }.OrderBy(i => i, StringComparer.Ordinal)
            .ToArray();

            // Act

            var paths = RuntimeDependenciesResolver
                .ResolveRuntimeDependenciesCore(
                    mainAssemblyLocation,
                    references,
                    bclLocations);

            var contents = paths
                .Select(p => Path.GetFileName(p))
                .OrderBy(i => i, StringComparer.Ordinal)
                .ToArray();

            var expected = new HashSet<string>(expectedContents);
            var actual = new HashSet<string>(contents);
            _output.WriteLine("Expected contents to have:");
            _output.WriteLine(string.Join(",", expected.Except(actual)));

            _output.WriteLine("Unexpected contents:");
            _output.WriteLine(string.Join(",", actual.Except(expected)));

            // Assert
            Assert.Equal(expectedContents, contents);
        }
    }
}
