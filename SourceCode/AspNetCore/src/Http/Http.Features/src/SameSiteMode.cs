// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// Used to set the SameSite field on response cookies to indicate if those cookies should be included by the client on future "same-site" or "cross-site" requests.
    /// RFC Draft: https://tools.ietf.org/html/draft-ietf-httpbis-cookie-same-site-00
    /// </summary>
    // This mirrors Microsoft.Net.Http.Headers.SameSiteMode
    public enum SameSiteMode
    {
        /// <summary>No SameSite field will be set, the client should follow its default cookie policy.</summary>
        None = 0,
        /// <summary>Indicates the client should send the cookie with "same-site" requests, and with "cross-site" top-level navigations.</summary>
        Lax,
        /// <summary>Indicates the client should only send the cookie with "same-site" requests.</summary>
        Strict
    }
}
