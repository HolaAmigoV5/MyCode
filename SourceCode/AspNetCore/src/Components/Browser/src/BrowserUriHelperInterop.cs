// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Components.Browser
{
    // Shared interop constants
    internal static class BrowserUriHelperInterop
    {
        private static readonly string Prefix = "Blazor._internal.uriHelper.";

        public static readonly string EnableNavigationInterception = Prefix + "enableNavigationInterception";

        public static readonly string GetLocationHref = Prefix + "getLocationHref";

        public static readonly string GetBaseUri = Prefix + "getBaseURI";

        public static readonly string NavigateTo = Prefix + "navigateTo";
    }
}
