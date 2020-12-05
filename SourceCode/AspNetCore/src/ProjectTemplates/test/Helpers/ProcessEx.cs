// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;
using Xunit.Abstractions;

namespace Templates.Test.Helpers
{
    internal class ProcessEx : IDisposable
    {
        private static readonly string NUGET_PACKAGES = GetNugetPackagesRestorePath();

        private readonly ITestOutputHelper _output;
        private readonly Process _process;
        private readonly StringBuilder _stderrCapture;
        private readonly StringBuilder _stdoutCapture;
        private readonly object _pipeCaptureLock = new object();
        private BlockingCollection<string> _stdoutLines;
        private TaskCompletionSource<int> _exited;

        public ProcessEx(ITestOutputHelper output, Process proc)
        {
            _output = output;
            _stdoutCapture = new StringBuilder();
            _stderrCapture = new StringBuilder();
            _stdoutLines = new BlockingCollection<string>();

            _process = proc;
            proc.EnableRaisingEvents = true;
            proc.OutputDataReceived += OnOutputData;
            proc.ErrorDataReceived += OnErrorData;
            proc.Exited += OnProcessExited;
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            _exited = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public Task Exited => _exited.Task;

        public bool HasExited => _process.HasExited;

        public string Error
        {
            get
            {
                lock (_pipeCaptureLock)
                {
                    return _stderrCapture.ToString();
                }
            }
        }

        public string Output
        {
            get
            {
                lock (_pipeCaptureLock)
                {
                    return _stdoutCapture.ToString();
                }
            }
        }

        public IEnumerable<string> OutputLinesAsEnumerable => _stdoutLines.GetConsumingEnumerable();

        public int ExitCode => _process.ExitCode;

        public static ProcessEx Run(ITestOutputHelper output, string workingDirectory, string command, string args = null, IDictionary<string, string> envVars = null)
        {
            var startInfo = new ProcessStartInfo(command, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };

            if (envVars != null)
            {
                foreach (var envVar in envVars)
                {
                    startInfo.EnvironmentVariables[envVar.Key] = envVar.Value;
                }
            }

            startInfo.EnvironmentVariables["NUGET_PACKAGES"] = NUGET_PACKAGES;

            output.WriteLine($"==> {startInfo.FileName} {startInfo.Arguments} [{startInfo.WorkingDirectory}]");
            var proc = Process.Start(startInfo);

            return new ProcessEx(output, proc);
        }

        public static async Task<ProcessEx> RunViaShellAsync(ITestOutputHelper output, string workingDirectory, string commandAndArgs)
        {
            var (shellExe, argsPrefix) = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? ("cmd", "/c")
                : ("bash", "-c");

            var result = Run(output, workingDirectory, shellExe, $"{argsPrefix} \"{commandAndArgs}\"");
            await result.Exited;
            return result;
        }

        private void OnErrorData(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }

            lock (_pipeCaptureLock)
            {
                _stderrCapture.AppendLine(e.Data);
            }

            _output.WriteLine("[ERROR] " + e.Data);
        }

        private void OnOutputData(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }

            lock (_pipeCaptureLock)
            {
                _stdoutCapture.AppendLine(e.Data);
            }

            _output.WriteLine(e.Data);

            if (_stdoutLines != null)
            {
                _stdoutLines.Add(e.Data);
            }
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            _process.WaitForExit();
            _stdoutLines.CompleteAdding();
            _stdoutLines = null;
            _exited.TrySetResult(_process.ExitCode);
        }

        internal string GetFormattedOutput()
        {
            if (!_process.HasExited)
            {
                throw new InvalidOperationException("Process has not finished running.");
            }

            return $"Process exited with code {_process.ExitCode}\nStdErr: {Error}\nStdOut: {Output}";
        }

        public void WaitForExit(bool assertSuccess)
        {
            Exited.Wait();

            if (assertSuccess && _process.ExitCode != 0)
            {
                throw new Exception($"Process exited with code {_process.ExitCode}\nStdErr: {Error}\nStdOut: {Output}");
            }
        }

        private static string GetNugetPackagesRestorePath() =>
            typeof(ProcessEx).Assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .First(attribute => attribute.Key == "TestPackageRestorePath")
                .Value;

        public void Dispose()
        {
            if (_process != null && !_process.HasExited)
            {
                _process.KillTree();
            }

            _process.CancelOutputRead();
            _process.CancelErrorRead();

            _process.ErrorDataReceived -= OnErrorData;
            _process.OutputDataReceived -= OnOutputData;
            _process.Exited -= OnProcessExited;
            _process.Dispose();
        }
    }
}
