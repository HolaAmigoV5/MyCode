// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Reflection;

namespace Microsoft.AspNetCore
{
    public class TestData
    {
        public static string GetSharedFxVersion() => GetTestDataValue("SharedFxVersion");

        public static string GetMicrosoftNETCoreAppPackageVersion() => GetTestDataValue("MicrosoftNETCoreAppPackageVersion");

        public static string GetRepositoryCommit() => GetTestDataValue("RepositoryCommit");

        public static string GetSharedFxRuntimeIdentifier() => GetTestDataValue("TargetRuntimeIdentifier");

        public static string GetSharedFxDependencies() => GetTestDataValue("SharedFxDependencies");

        public static string GetTestDataValue(string key)
             => typeof(TestData).Assembly.GetCustomAttributes<TestDataAttribute>().Single(d => d.Key == key).Value;
    }
}
