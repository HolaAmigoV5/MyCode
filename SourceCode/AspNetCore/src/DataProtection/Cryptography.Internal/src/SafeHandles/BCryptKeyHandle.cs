// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Cryptography.SafeHandles
{
    internal sealed class BCryptKeyHandle : BCryptHandle
    {
        private BCryptAlgorithmHandle _algProviderHandle;

        // Called by P/Invoke when returning SafeHandles
        private BCryptKeyHandle() { }

        // Do not provide a finalizer - SafeHandle's critical finalizer will call ReleaseHandle for you.
        protected override bool ReleaseHandle()
        {
            _algProviderHandle = null;
            return (UnsafeNativeMethods.BCryptDestroyKey(handle) == 0);
        }

        // We don't actually need to hold a reference to the algorithm handle, as the native CNG library
        // already holds the reference for us. But once we create a key from an algorithm provider, odds
        // are good that we'll create another key from the same algorithm provider at some point in the
        // future. And since algorithm providers are expensive to create, we'll hold a strong reference
        // to all known in-use providers. This way the cached algorithm provider handles utility class
        // doesn't keep creating providers over and over.
        internal void SetAlgorithmProviderHandle(BCryptAlgorithmHandle algProviderHandle)
        {
            _algProviderHandle = algProviderHandle;
        }
    }
}
