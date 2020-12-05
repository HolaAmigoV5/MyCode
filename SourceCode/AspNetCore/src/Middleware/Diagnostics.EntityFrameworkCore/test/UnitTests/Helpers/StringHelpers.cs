// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore.Tests.Helpers
{
    public class StringsHelpers
    {
        public static string GetResourceString(string stringName, params object[] parameters)
        {
            var strings = typeof(DatabaseErrorPageMiddleware).GetTypeInfo().Assembly.GetType("Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore.Strings").GetTypeInfo();
            var method = strings.GetDeclaredMethods(stringName).Single();
            return (string)method.Invoke(null, parameters);
        }
    }
}