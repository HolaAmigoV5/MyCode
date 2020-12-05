// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.E2ETesting;
using Templates.Test.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Templates.Test.SpaTemplateTest
{
    public class AngularTemplateTest : SpaTemplateTestBase
    {
        public AngularTemplateTest(ProjectFactoryFixture projectFactory, BrowserFixture browserFixture, ITestOutputHelper output)
            : base(projectFactory, browserFixture, output) { }

        [Fact]
        public Task AngularTemplate_Works()
            => SpaTemplateImplAsync("angularnoauth", "angular", useLocalDb: false, usesAuth: false);

        [Fact]
        public Task AngularTemplate_IndividualAuth_Works()
            => SpaTemplateImplAsync("angularindividual", "angular", useLocalDb: false, usesAuth: true);

        [Fact]
        public Task AngularTemplate_IndividualAuth_Works_LocalDb()
            => SpaTemplateImplAsync("angularindividualuld", "angular", useLocalDb: true, usesAuth: true);
    }
}
