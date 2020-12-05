// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Specifies options for the <see cref="RequestLocalizationMiddleware"/>.
    /// </summary>
    public class RequestLocalizationOptions
    {
        private RequestCulture _defaultRequestCulture =
            new RequestCulture(CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);

        /// <summary>
        /// Creates a new <see cref="RequestLocalizationOptions"/> with default values.
        /// </summary>
        public RequestLocalizationOptions()
        {
            RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider { Options = this },
                new CookieRequestCultureProvider { Options = this },
                new AcceptLanguageHeaderRequestCultureProvider { Options = this }
            };
        }

        /// <summary>
        /// Gets or sets the default culture to use for requests when a supported culture could not be determined by
        /// one of the configured <see cref="IRequestCultureProvider"/>s.
        /// Defaults to <see cref="CultureInfo.CurrentCulture"/> and <see cref="CultureInfo.CurrentUICulture"/>.
        /// </summary>
        public RequestCulture DefaultRequestCulture
        {
            get
            {
                return _defaultRequestCulture;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _defaultRequestCulture = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to set a request culture to an parent culture in the case the
        /// culture determined by the configured <see cref="IRequestCultureProvider"/>s is not in the
        /// <see cref="SupportedCultures"/> list but a parent culture is.
        /// Defaults to <c>true</c>;
        /// </summary>
        /// <remarks>
        /// Note that the parent culture check is done using only the culture name.
        /// </remarks>
        /// <example>
        /// If this property is <c>true</c> and the application is configured to support the culture "fr", but not the
        /// culture "fr-FR", and a configured <see cref="IRequestCultureProvider"/> determines a request's culture is
        /// "fr-FR", then the request's culture will be set to the culture "fr", as it is a parent of "fr-FR".
        /// </example>
        public bool FallBackToParentCultures { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to set a request UI culture to a parent culture in the case the
        /// UI culture determined by the configured <see cref="IRequestCultureProvider"/>s is not in the
        /// <see cref="SupportedUICultures"/> list but a parent culture is.
        /// Defaults to <c>true</c>;
        /// </summary>
        /// <remarks>
        /// Note that the parent culture check is done using ony the culture name.
        /// </remarks>
        /// <example>
        /// If this property is <c>true</c> and the application is configured to support the UI culture "fr", but not
        /// the UI culture "fr-FR", and a configured <see cref="IRequestCultureProvider"/> determines a request's UI
        /// culture is "fr-FR", then the request's UI culture will be set to the culture "fr", as it is a parent of
        /// "fr-FR".
        /// </example>
        public bool FallBackToParentUICultures { get; set; } = true;

        /// <summary>
        /// The cultures supported by the application. The <see cref="RequestLocalizationMiddleware"/> will only set
        /// the current request culture to an entry in this list.
        /// Defaults to <see cref="CultureInfo.CurrentCulture"/>.
        /// </summary>
        public IList<CultureInfo> SupportedCultures { get; set; } = new List<CultureInfo> { CultureInfo.CurrentCulture };

        /// <summary>
        /// The UI cultures supported by the application. The <see cref="RequestLocalizationMiddleware"/> will only set
        /// the current request culture to an entry in this list.
        /// Defaults to <see cref="CultureInfo.CurrentUICulture"/>.
        /// </summary>
        public IList<CultureInfo> SupportedUICultures { get; set; } = new List<CultureInfo> { CultureInfo.CurrentUICulture };

        /// <summary>
        /// An ordered list of providers used to determine a request's culture information. The first provider that
        /// returns a non-<c>null</c> result for a given request will be used.
        /// Defaults to the following:
        /// <list type="number">
        ///     <item><description><see cref="QueryStringRequestCultureProvider"/></description></item>
        ///     <item><description><see cref="CookieRequestCultureProvider"/></description></item>
        ///     <item><description><see cref="AcceptLanguageHeaderRequestCultureProvider"/></description></item>
        /// </list>
        /// </summary>
        public IList<IRequestCultureProvider> RequestCultureProviders { get; set; }

        /// <summary>
        /// Adds the set of the supported cultures by the application.
        /// </summary>
        /// <param name="cultures">The cultures to be added.</param>
        /// <returns>The <see cref="RequestLocalizationOptions"/>.</returns>
        public RequestLocalizationOptions AddSupportedCultures(params string[] cultures)
        {
            var supportedCultures = new List<CultureInfo>();

            foreach (var culture in cultures)
            {
                supportedCultures.Add(new CultureInfo(culture));
            }

            SupportedCultures = supportedCultures;
            return this;
        }

        /// <summary>
        /// Adds the set of the supported UI cultures by the application.
        /// </summary>
        /// <param name="uiCultures">The UI cultures to be added.</param>
        /// <returns>The <see cref="RequestLocalizationOptions"/>.</returns>
        public RequestLocalizationOptions AddSupportedUICultures(params string[] uiCultures)
        {
            var supportedUICultures = new List<CultureInfo>();
            foreach (var culture in uiCultures)
            {
                supportedUICultures.Add(new CultureInfo(culture));
            }

            SupportedUICultures = supportedUICultures;
            return this;
        }

        /// <summary>
        /// Set the default culture which is used by the application when a supported culture could not be determined by
        /// one of the configured <see cref="IRequestCultureProvider"/>s.
        /// </summary>
        /// <param name="defaultCulture">The default culture to be set.</param>
        /// <returns>The <see cref="RequestLocalizationOptions"/>.</returns>
        public RequestLocalizationOptions SetDefaultCulture(string defaultCulture)
        {
            DefaultRequestCulture = new RequestCulture(defaultCulture);
            return this;
        }
    }
}
