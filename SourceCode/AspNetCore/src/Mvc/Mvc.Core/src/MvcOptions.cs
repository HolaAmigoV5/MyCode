// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.WebUtilities;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Provides programmatic configuration for the MVC framework.
    /// </summary>
    public class MvcOptions : IEnumerable<ICompatibilitySwitch>
    {
        internal const int DefaultMaxModelBindingCollectionSize = FormReader.DefaultValueCountLimit;
        internal const int DefaultMaxModelBindingRecursionDepth = 32;

        private readonly IReadOnlyList<ICompatibilitySwitch> _switches = Array.Empty<ICompatibilitySwitch>();

        private int _maxModelStateErrors = ModelStateDictionary.DefaultMaxAllowedErrors;
        private int _maxModelBindingCollectionSize = DefaultMaxModelBindingCollectionSize;
        private int _maxModelBindingRecursionDepth = DefaultMaxModelBindingRecursionDepth;
        private int? _maxValidationDepth = 32;

        /// <summary>
        /// Creates a new instance of <see cref="MvcOptions"/>.
        /// </summary>
        public MvcOptions()
        {
            CacheProfiles = new Dictionary<string, CacheProfile>(StringComparer.OrdinalIgnoreCase);
            Conventions = new List<IApplicationModelConvention>();
            Filters = new FilterCollection();
            FormatterMappings = new FormatterMappings();
            InputFormatters = new FormatterCollection<IInputFormatter>();
            OutputFormatters = new FormatterCollection<IOutputFormatter>();
            ModelBinderProviders = new List<IModelBinderProvider>();
            ModelBindingMessageProvider = new DefaultModelBindingMessageProvider();
            ModelMetadataDetailsProviders = new List<IMetadataDetailsProvider>();
            ModelValidatorProviders = new List<IModelValidatorProvider>();
            ValueProviderFactories = new List<IValueProviderFactory>();
        }

        /// <summary>
        /// Gets or sets a value that determines if routing should use endpoints internally, or if legacy routing
        /// logic should be used. Endpoint routing is used to match HTTP requests to MVC actions, and to generate
        /// URLs with <see cref="IUrlHelper"/>.
        /// </summary>
        /// <value>
        /// The default value is <see langword="true"/>.
        /// </value>
        public bool EnableEndpointRouting { get; set; } = true;

        /// <summary>
        /// Gets or sets the flag which decides whether body model binding (for example, on an
        /// action method parameter with <see cref="FromBodyAttribute"/>) should treat empty
        /// input as valid. <see langword="false"/> by default.
        /// </summary>
        /// <example>
        /// When <see langword="false"/>, actions that model bind the request body (for example,
        /// using <see cref="FromBodyAttribute"/>) will register an error in the
        /// <see cref="ModelStateDictionary"/> if the incoming request body is empty.
        /// </example>
        public bool AllowEmptyInputInBodyModelBinding { get; set; }

        /// <summary>
        /// Gets a Dictionary of CacheProfile Names, <see cref="CacheProfile"/> which are pre-defined settings for
        /// response caching.
        /// </summary>
        public IDictionary<string, CacheProfile> CacheProfiles { get; }

        /// <summary>
        /// Gets a list of <see cref="IApplicationModelConvention"/> instances that will be applied to
        /// the <see cref="ApplicationModel"/> when discovering actions.
        /// </summary>
        public IList<IApplicationModelConvention> Conventions { get; }

        /// <summary>
        /// Gets a collection of <see cref="IFilterMetadata"/> which are used to construct filters that
        /// apply to all actions.
        /// </summary>
        public FilterCollection Filters { get; }

        /// <summary>
        /// Used to specify mapping between the URL Format and corresponding media type.
        /// </summary>
        public FormatterMappings FormatterMappings { get; }

        /// <summary>
        /// Gets a list of <see cref="IInputFormatter"/>s that are used by this application.
        /// </summary>
        public FormatterCollection<IInputFormatter> InputFormatters { get; }

        /// <summary>
        /// Gets or sets the flag to buffer the request body in input formatters. Default is <c>false</c>.
        /// </summary>
        public bool SuppressInputFormatterBuffering { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum number of validation errors that are allowed by this application before further
        /// errors are ignored.
        /// </summary>
        public int MaxModelValidationErrors
        {
            get => _maxModelStateErrors;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _maxModelStateErrors = value;
            }
        }

        /// <summary>
        /// Gets a list of <see cref="IModelBinderProvider"/>s used by this application.
        /// </summary>
        public IList<IModelBinderProvider> ModelBinderProviders { get; }

        /// <summary>
        /// Gets the default <see cref="ModelBinding.Metadata.ModelBindingMessageProvider"/>. Changes here are copied to the
        /// <see cref="ModelMetadata.ModelBindingMessageProvider"/> property of all <see cref="ModelMetadata"/>
        /// instances unless overridden in a custom <see cref="IBindingMetadataProvider"/>.
        /// </summary>
        public DefaultModelBindingMessageProvider ModelBindingMessageProvider { get; }

        /// <summary>
        /// Gets a list of <see cref="IMetadataDetailsProvider"/> instances that will be used to
        /// create <see cref="ModelMetadata"/> instances.
        /// </summary>
        /// <remarks>
        /// A provider should implement one or more of the following interfaces, depending on what
        /// kind of details are provided:
        /// <ul>
        /// <li><see cref="IBindingMetadataProvider"/></li>
        /// <li><see cref="IDisplayMetadataProvider"/></li>
        /// <li><see cref="IValidationMetadataProvider"/></li>
        /// </ul>
        /// </remarks>
        public IList<IMetadataDetailsProvider> ModelMetadataDetailsProviders { get; }

        /// <summary>
        /// Gets a list of <see cref="IModelValidatorProvider"/>s used by this application.
        /// </summary>
        public IList<IModelValidatorProvider> ModelValidatorProviders { get; }

        /// <summary>
        /// Gets a list of <see cref="IOutputFormatter"/>s that are used by this application.
        /// </summary>
        public FormatterCollection<IOutputFormatter> OutputFormatters { get; }

        /// <summary>
        /// Gets or sets the flag which causes content negotiation to ignore Accept header
        /// when it contains the media type */*. <see langword="false"/> by default.
        /// </summary>
        public bool RespectBrowserAcceptHeader { get; set; }

        /// <summary>
        /// Gets or sets the flag which decides whether an HTTP 406 Not Acceptable response
        /// will be returned if no formatter has been selected to format the response.
        /// <see langword="false"/> by default.
        /// </summary>
        public bool ReturnHttpNotAcceptable { get; set; }

        /// <summary>
        /// Gets a list of <see cref="IValueProviderFactory"/> used by this application.
        /// </summary>
        public IList<IValueProviderFactory> ValueProviderFactories { get; }

        /// <summary>
        /// Gets or sets the SSL port that is used by this application when <see cref="RequireHttpsAttribute"/>
        /// is used. If not set the port won't be specified in the secured URL e.g. https://localhost/path.
        /// </summary>
        public int? SslPort { get; set; }

        /// <summary>
        /// Gets or sets the default value for the Permanent property of <see cref="RequireHttpsAttribute"/>.
        /// </summary>
        public bool RequireHttpsPermanent { get; set; }

        /// <summary>
        /// Gets or sets the maximum depth to constrain the validation visitor when validating. Set to <see langword="null" />
        /// to disable this feature.
        /// <para>
        /// <see cref="ValidationVisitor"/> traverses the object graph of the model being validated. For models
        /// that are very deep or are infinitely recursive, validation may result in stack overflow.
        /// </para>
        /// <para>
        /// When not <see langword="null"/>, <see cref="ValidationVisitor"/> will throw if
        /// traversing an object exceeds the maximum allowed validation depth.
        /// </para>
        /// </summary>
        /// <value>
        /// The default value is <c>32</c>.
        /// </value>
        public int? MaxValidationDepth
        {
            get => _maxValidationDepth;
            set
            {
                if (value != null && value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _maxValidationDepth = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines if MVC will remove the suffix "Async" applied to
        /// controller action names.
        /// <para>
        /// <see cref="ControllerActionDescriptor.ActionName"/> is used to construct the route to the action as
        /// well as in view lookup. When <see langword="true"/>, MVC will trim the suffix "Async" applied
        /// to action method names.
        /// For example, the action name for <c>ProductsController.ListProductsAsync</c> will be
        /// canonicalized as <c>ListProducts.</c>. Consequently, it will be routeable at
        /// <c>/Products/ListProducts</c> with views looked up at <c>/Views/Products/ListProducts.cshtml</c>.
        /// </para>
        /// <para>
        /// This option does not affect values specified using using <see cref="ActionNameAttribute"/>.
        /// </para>
        /// </summary>
        /// <value>
        /// The default value is <see langword="true"/>.
        /// </value>
        public bool SuppressAsyncSuffixInActionNames { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum size of a complex collection to model bind. When this limit is reached, the model
        /// binding system will throw an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When binding a collection, some element binders may succeed unconditionally and model binding may run out
        /// of memory. This limit constrains such unbounded collection growth; it is a safeguard against incorrect
        /// model binders and models.
        /// </para>
        /// <para>
        /// This limit does not <em>correct</em> the bound model. The <see cref="InvalidOperationException"/> instead
        /// informs the developer of an issue in their model or model binder. The developer must correct that issue.
        /// </para>
        /// <para>
        /// This limit does not apply to collections of simple types. When
        /// <see cref="CollectionModelBinder{TElement}"/> relies entirely on <see cref="IValueProvider"/>s, it cannot
        /// create collections larger than the available data.
        /// </para>
        /// <para>
        /// A very high value for this option (<c>int.MaxValue</c> for example) effectively removes the limit and is
        /// not recommended.
        /// </para>
        /// </remarks>
        /// <value>The default value is <c>1024</c>, matching <see cref="FormReader.DefaultValueCountLimit"/>.</value>
        public int MaxModelBindingCollectionSize
        {
            get => _maxModelBindingCollectionSize;
            set
            {
                // Disallowing an empty collection would cause the CollectionModelBinder to throw unconditionally.
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _maxModelBindingCollectionSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum recursion depth of the model binding system. The
        /// <see cref="DefaultModelBindingContext"/> will throw an <see cref="InvalidOperationException"/> if more than
        /// this number of <see cref="IModelBinder"/>s are on the stack. That is, an attempt to recurse beyond this
        /// level will fail.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For some self-referential models, some binders may succeed unconditionally and model binding may result in
        /// stack overflow. This limit constrains such unbounded recursion; it is a safeguard against incorrect model
        /// binders and models. This limit also protects against very deep model type hierarchies lacking
        /// self-references.
        /// </para>
        /// <para>
        /// This limit does not <em>correct</em> the bound model. The <see cref="InvalidOperationException"/> instead
        /// informs the developer of an issue in their model. The developer must correct that issue.
        /// </para>
        /// <para>
        /// A very high value for this option (<c>int.MaxValue</c> for example) effectively removes the limit and is
        /// not recommended.
        /// </para>
        /// </remarks>
        /// <value>The default value is <c>32</c>, matching the default <see cref="MaxValidationDepth"/> value.</value>
        public int MaxModelBindingRecursionDepth
        {
            get => _maxModelBindingRecursionDepth;
            set
            {
                // Disallowing one model binder (if supported) would cause the model binding system to throw
                // unconditionally. DefaultModelBindingContext always allows a top-level binder i.e. its own creation.
                if (value <= 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _maxModelBindingRecursionDepth = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="JsonSerializerOptions"/> used by <see cref="SystemTextJsonInputFormatter"/> and
        /// <see cref="SystemTextJsonOutputFormatter"/>.
        /// </summary>
        public JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions
        {
            // Allow for the payload to have null values for some inputs (under-binding)
            IgnoreNullPropertyValueOnRead = true,

            ReaderOptions = new JsonReaderOptions
            {
                // Limit the object graph we'll consume to a fixed depth. This prevents stackoverflow exceptions
                // from deserialization errors that might occur from deeply nested objects.
                // This value is to be kept in sync with JsonSerializerSettingsProvider.DefaultMaxDepth
                MaxDepth = DefaultMaxModelBindingRecursionDepth,
            },
        };

        IEnumerator<ICompatibilitySwitch> IEnumerable<ICompatibilitySwitch>.GetEnumerator() => _switches.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _switches.GetEnumerator();
    }
}
