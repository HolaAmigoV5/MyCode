﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures
{
    internal class UnsupportedJavaScriptRuntime : IJSRuntime
    {
        public Task<T> InvokeAsync<T>(string identifier, params object[] args)
        {
            throw new InvalidOperationException("JavaScript interop calls cannot be issued during server-side prerendering, because the page has not yet loaded in the browser. Prerendered components must wrap any JavaScript interop calls in conditional logic to ensure those interop calls are not attempted during prerendering.");
        }

        public void UntrackObjectRef(DotNetObjectRef dotNetObjectRef)
        {
            throw new InvalidOperationException("JavaScript interop calls cannot be issued during server-side prerendering, because the page has not yet loaded in the browser. Prerendered components must wrap any JavaScript interop calls in conditional logic to ensure those interop calls are not attempted during prerendering.");
        }
    }
}
