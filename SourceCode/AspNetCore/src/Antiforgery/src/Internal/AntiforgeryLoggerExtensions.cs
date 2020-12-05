// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Antiforgery
{
    internal static class AntiforgeryLoggerExtensions
    {
        private static readonly Action<ILogger, Exception> _failedToDeserialzeTokens;
        private static readonly Action<ILogger, string, Exception> _validationFailed;
        private static readonly Action<ILogger, Exception> _validated;
        private static readonly Action<ILogger, string, Exception> _missingCookieToken;
        private static readonly Action<ILogger, string, string, Exception> _missingRequestToken;
        private static readonly Action<ILogger, Exception> _newCookieToken;
        private static readonly Action<ILogger, Exception> _reusedCookieToken;
        private static readonly Action<ILogger, Exception> _tokenDeserializeException;
        private static readonly Action<ILogger, Exception> _responseCacheHeadersOverridenToNoCache;

        static AntiforgeryLoggerExtensions()
        {
            _validationFailed = LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(1, "ValidationFailed"),
                "Antiforgery validation failed with message '{Message}'.");
            _validated = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(2, "Validated"),
                "Antiforgery successfully validated a request.");
            _missingCookieToken = LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(3, "MissingCookieToken"),
                "The required antiforgery cookie '{CookieName}' is not present.");
            _missingRequestToken = LoggerMessage.Define<string, string>(
                LogLevel.Warning,
                new EventId(4, "MissingRequestToken"),
                "The required antiforgery request token was not provided in either form field '{FormFieldName}' "
                    + "or header '{HeaderName}'.");
            _newCookieToken = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(5, "NewCookieToken"),
                "A new antiforgery cookie token was created.");
            _reusedCookieToken = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(6, "ReusedCookieToken"),
                "An antiforgery cookie token was reused.");
            _tokenDeserializeException = LoggerMessage.Define(
                LogLevel.Error,
                new EventId(7, "TokenDeserializeException"),
                "An exception was thrown while deserializing the token.");
            _responseCacheHeadersOverridenToNoCache = LoggerMessage.Define(
                LogLevel.Warning,
                new EventId(8, "ResponseCacheHeadersOverridenToNoCache"),
                "The 'Cache-Control' and 'Pragma' headers have been overridden and set to 'no-cache, no-store' and " +
                "'no-cache' respectively to prevent caching of this response. Any response that uses antiforgery " +
                "should not be cached.");
            _failedToDeserialzeTokens = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(9, "FailedToDeserialzeTokens"),
                "Failed to deserialize antiforgery tokens.");
        }

        public static void ValidationFailed(this ILogger logger, string message)
        {
            _validationFailed(logger, message, null);
        }

        public static void ValidatedAntiforgeryToken(this ILogger logger)
        {
            _validated(logger, null);
        }

        public static void MissingCookieToken(this ILogger logger, string cookieName)
        {
            _missingCookieToken(logger, cookieName, null);
        }

        public static void MissingRequestToken(this ILogger logger, string formFieldName, string headerName)
        {
            _missingRequestToken(logger, formFieldName, headerName, null);
        }

        public static void NewCookieToken(this ILogger logger)
        {
            _newCookieToken(logger, null);
        }

        public static void ReusedCookieToken(this ILogger logger)
        {
            _reusedCookieToken(logger, null);
        }

        public static void TokenDeserializeException(this ILogger logger, Exception exception)
        {
            _tokenDeserializeException(logger, exception);
        }

        public static void ResponseCacheHeadersOverridenToNoCache(this ILogger logger)
        {
            _responseCacheHeadersOverridenToNoCache(logger, null);
        }

        public static void FailedToDeserialzeTokens(this ILogger logger, Exception exception)
        {
            _failedToDeserialzeTokens(logger, exception);
        }
    }
}
