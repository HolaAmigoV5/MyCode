// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Templates.Test.Helpers
{
    public class Project
    {
        public const string DefaultFramework = "netcoreapp3.0";

        public SemaphoreSlim DotNetNewLock { get; set; }
        public SemaphoreSlim NodeLock { get; set; }
        public string ProjectName { get; set; }
        public string ProjectArguments { get; set; }
        public string ProjectGuid { get; set; }
        public string TemplateOutputDir { get; set; }
        public string TemplateBuildDir => Path.Combine(TemplateOutputDir, "bin", "Debug", DefaultFramework);
        public string TemplatePublishDir => Path.Combine(TemplateOutputDir, "bin", "Release", DefaultFramework, "publish");

        public ITestOutputHelper Output { get; set; }
        public IMessageSink DiagnosticsMessageSink { get; set; }

        internal async Task<ProcessEx> RunDotNetNewAsync(string templateName, string auth = null, string language = null, bool useLocalDB = false, bool noHttps = false)
        {
            var hiveArg = $"--debug:custom-hive \"{TemplatePackageInstaller.CustomHivePath}\"";
            var args = $"new {templateName} {hiveArg}";

            if (!string.IsNullOrEmpty(auth))
            {
                args += $" --auth {auth}";
            }

            if (!string.IsNullOrEmpty(language))
            {
                args += $" -lang {language}";
            }

            if (useLocalDB)
            {
                args += $" --use-local-db";
            }

            if (noHttps)
            {
                args += $" --no-https";
            }

            // Save a copy of the arguments used for better diagnostic error messages later.
            // We omit the hive argument and the template output dir as they are not relevant and add noise.
            ProjectArguments = args.Replace(hiveArg, "");

            args += $" -o {TemplateOutputDir}";

            // Only run one instance of 'dotnet new' at once, as a workaround for
            // https://github.com/aspnet/templating/issues/63

            await DotNetNewLock.WaitAsync();
            try
            {
                var execution = ProcessEx.Run(Output, AppContext.BaseDirectory, DotNetMuxer.MuxerPathOrDefault(), args);
                await execution.Exited;
                return execution;
            }
            finally
            {
                DotNetNewLock.Release();
            }
        }

        internal async Task<ProcessEx> RunDotNetPublishAsync(bool takeNodeLock = false)
        {
            Output.WriteLine("Publishing ASP.NET application...");

            // Workaround for issue with runtime store not yet being published
            // https://github.com/aspnet/Home/issues/2254#issuecomment-339709628
            var extraArgs = "-p:PublishWithAspNetCoreTargetManifest=false";


            // This is going to trigger a build, so we need to acquire the lock like in the other cases.
            // We want to take the node lock as some builds run NPM as part of the build and we want to make sure
            // it's run without interruptions.
            var effectiveLock = takeNodeLock ? new OrderedLock(NodeLock, DotNetNewLock) : new OrderedLock(nodeLock: null, DotNetNewLock);
            await effectiveLock.WaitAsync();
            try
            {
                var result = ProcessEx.Run(Output, TemplateOutputDir, DotNetMuxer.MuxerPathOrDefault(), $"publish -c Release {extraArgs}");
                await result.Exited;
                return result;
            }
            finally
            {
                effectiveLock.Release();
            }
        }

        internal async Task<ProcessEx> RunDotNetBuildAsync(bool takeNodeLock = false)
        {
            Output.WriteLine("Building ASP.NET application...");

            // This is going to trigger a build, so we need to acquire the lock like in the other cases.
            // We want to take the node lock as some builds run NPM as part of the build and we want to make sure
            // it's run without interruptions.
            var effectiveLock = takeNodeLock ? new OrderedLock(NodeLock, DotNetNewLock) : new OrderedLock(nodeLock: null, DotNetNewLock);
            await effectiveLock.WaitAsync();
            try
            {
                var result = ProcessEx.Run(Output, TemplateOutputDir, DotNetMuxer.MuxerPathOrDefault(), "build -c Debug");
                await result.Exited;
                return result;
            }
            finally
            {
                effectiveLock.Release();
            }
        }

        internal AspNetProcess StartBuiltProjectAsync()
        {
            var environment = new Dictionary<string, string>
            {
                ["ASPNETCORE_URLS"] = $"http://127.0.0.1:0;https://127.0.0.1:0",
                ["ASPNETCORE_ENVIRONMENT"] = "Development"
            };

            var projectDll = Path.Combine(TemplateBuildDir, $"{ProjectName}.dll");
            return new AspNetProcess(Output, TemplateOutputDir, projectDll, environment);
        }

        internal AspNetProcess StartPublishedProjectAsync()
        {
            var environment = new Dictionary<string, string>
            {
                ["ASPNETCORE_URLS"] = $"http://127.0.0.1:0;https://127.0.0.1:0",
            };

            var projectDll = $"{ProjectName}.dll";
            return new AspNetProcess(Output, TemplatePublishDir, projectDll, environment);
        }

        internal async Task<ProcessEx> RestoreWithRetryAsync(ITestOutputHelper output, string workingDirectory)
        {
            // "npm restore" sometimes fails randomly in AppVeyor with errors like:
            //    EPERM: operation not permitted, scandir <path>...
            // This appears to be a general NPM reliability issue on Windows which has
            // been reported many times (e.g., https://github.com/npm/npm/issues/18380)
            // So, allow multiple attempts at the restore.
            const int maxAttempts = 3;
            var attemptNumber = 0;
            ProcessEx restoreResult;
            do
            {
                restoreResult = await RestoreAsync(output, workingDirectory);
                if (restoreResult.ExitCode == 0)
                {
                    return restoreResult;
                }
                else
                {
                    // TODO: We should filter for EPEM here to avoid masking other errors silently.
                    output.WriteLine(
                        $"NPM restore in {workingDirectory} failed on attempt {attemptNumber} of {maxAttempts}. " +
                        $"Error was: {restoreResult.GetFormattedOutput()}");

                    // Clean up the possibly-incomplete node_modules dir before retrying
                    CleanNodeModulesFolder(workingDirectory, output);
                }
                attemptNumber++;
            } while (attemptNumber < maxAttempts);

            output.WriteLine($"Giving up attempting NPM restore in {workingDirectory} after {attemptNumber} attempts.");
            return restoreResult;

            void CleanNodeModulesFolder(string workingDirectory, ITestOutputHelper output)
            {
                var nodeModulesDir = Path.Combine(workingDirectory, "node_modules");
                try
                {
                    if (Directory.Exists(nodeModulesDir))
                    {
                        Directory.Delete(nodeModulesDir, recursive: true);
                    }
                }
                catch
                {
                    output.WriteLine($"Failed to clean up node_modules folder at {nodeModulesDir}.");
                }
            }
        }

        private async Task<ProcessEx> RestoreAsync(ITestOutputHelper output, string workingDirectory)
        {
            // It's not safe to run multiple NPM installs in parallel
            // https://github.com/npm/npm/issues/2500
            await NodeLock.WaitAsync();
            try
            {
                output.WriteLine($"Restoring NPM packages in '{workingDirectory}' using npm...");
                var result = await ProcessEx.RunViaShellAsync(output, workingDirectory, "npm install");
                return result;
            }
            finally
            {
                NodeLock.Release();
            }
        }

        internal async Task<ProcessEx> RunDotNetEfCreateMigrationAsync(string migrationName)
        {
            var assembly = typeof(ProjectFactoryFixture).Assembly;

            var dotNetEfFullPath = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .First(attribute => attribute.Key == "DotNetEfFullPath")
                .Value;

            var args = $"\"{dotNetEfFullPath}\" --verbose --no-build migrations add {migrationName}";

            // Only run one instance of 'dotnet new' at once, as a workaround for
            // https://github.com/aspnet/templating/issues/63
            await DotNetNewLock.WaitAsync();
            try
            {
                var result = ProcessEx.Run(Output, TemplateOutputDir, DotNetMuxer.MuxerPathOrDefault(), args);
                await result.Exited;
                return result;
            }
            finally
            {
                DotNetNewLock.Release();
            }
        }

        // If this fails, you should generate new migrations via migrations/updateMigrations.cmd
        public void AssertEmptyMigration(string migration)
        {
            var fullPath = Path.Combine(TemplateOutputDir, "Data/Migrations");
            var file = Directory.EnumerateFiles(fullPath).Where(f => f.EndsWith($"{migration}.cs")).FirstOrDefault();

            Assert.NotNull(file);
            var contents = File.ReadAllText(file);

            var emptyMigration = @"protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }";

            // This comparison can break depending on how GIT checked out newlines on different files.
            Assert.Contains(RemoveNewLines(emptyMigration), RemoveNewLines(contents));

            static string RemoveNewLines(string str)
            {
                return str.Replace("\n", string.Empty).Replace("\r", string.Empty);
            }
        }

        internal async Task<ProcessEx> RunDotNetNewRawAsync(string arguments)
        {
            await DotNetNewLock.WaitAsync();
            try
            {
                var result = ProcessEx.Run(
                    Output,
                    AppContext.BaseDirectory,
                    DotNetMuxer.MuxerPathOrDefault(),
                    arguments +
                        $" --debug:custom-hive \"{TemplatePackageInstaller.CustomHivePath}\"" +
                        $" -o {TemplateOutputDir}");
                await result.Exited;
                return result;
            }
            finally
            {
                DotNetNewLock.Release();
            }
        }

        public void Dispose()
        {
            DeleteOutputDirectory();
        }

        public void DeleteOutputDirectory()
        {
            const int NumAttempts = 10;

            for (var numAttemptsRemaining = NumAttempts; numAttemptsRemaining > 0; numAttemptsRemaining--)
            {
                try
                {
                    Directory.Delete(TemplateOutputDir, true);
                    return;
                }
                catch (Exception ex)
                {
                    if (numAttemptsRemaining > 1)
                    {
                        DiagnosticsMessageSink.OnMessage(new DiagnosticMessage($"Failed to delete directory {TemplateOutputDir} because of error {ex.Message}. Will try again {numAttemptsRemaining - 1} more time(s)."));
                        Thread.Sleep(3000);
                    }
                    else
                    {
                        DiagnosticsMessageSink.OnMessage(new DiagnosticMessage($"Giving up trying to delete directory {TemplateOutputDir} after {NumAttempts} attempts. Most recent error was: {ex.StackTrace}"));
                    }
                }
            }
        }

        private class OrderedLock
        {
            private bool _nodeLockTaken;
            private bool _dotNetLockTaken;

            public OrderedLock(SemaphoreSlim nodeLock, SemaphoreSlim dotnetLock)
            {
                NodeLock = nodeLock;
                DotnetLock = dotnetLock;
            }

            public SemaphoreSlim NodeLock { get; }
            public SemaphoreSlim DotnetLock { get; }

            public async Task WaitAsync()
            {
                if (NodeLock == null)
                {
                    await DotnetLock.WaitAsync();
                    _dotNetLockTaken = true;
                    return;
                }

                try
                {
                    // We want to take the NPM lock first as is going to be the busiest one, and we want other threads to be
                    // able to run dotnet new while we are waiting for another thread to finish running NPM.
                    await NodeLock.WaitAsync();
                    _nodeLockTaken = true;
                    await DotnetLock.WaitAsync();
                    _dotNetLockTaken = true;
                }
                catch
                {
                    if (_nodeLockTaken)
                    {
                        NodeLock.Release();
                        _nodeLockTaken = false;
                    }
                    throw;
                }
            }

            public void Release()
            {
                try
                {
                    if (_dotNetLockTaken)
                    {

                        DotnetLock.Release();
                        _dotNetLockTaken = false;
                    }
                }
                finally
                {
                    if (_nodeLockTaken)
                    {
                        NodeLock.Release();
                        _nodeLockTaken = false;
                    }
                }
            }
        }
    }
}
