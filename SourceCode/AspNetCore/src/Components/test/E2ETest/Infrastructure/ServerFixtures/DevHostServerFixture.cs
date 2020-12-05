// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using DevHostServerProgram = Microsoft.AspNetCore.Blazor.Cli.Server.Program;

namespace Microsoft.AspNetCore.Components.E2ETest.Infrastructure.ServerFixtures
{
    public class DevHostServerFixture<TProgram> : WebHostServerFixture
    {
        public string Environment { get; set; }
        public string PathBase { get; set; }
        public string ContentRoot { get; private set; }

        protected override IWebHost CreateWebHost()
        {
            ContentRoot = FindSampleOrTestSitePath(
                typeof(TProgram).Assembly.FullName);

            var args = new List<string>
            {
                "--urls", "http://127.0.0.1:0",
                "--contentroot", ContentRoot,
                "--pathbase", PathBase
            };

            if (!string.IsNullOrEmpty(Environment))
            {
                args.Add("--environment");
                args.Add(Environment);
            }

            return DevHostServerProgram.BuildWebHost(args.ToArray());
        }
    }
}
