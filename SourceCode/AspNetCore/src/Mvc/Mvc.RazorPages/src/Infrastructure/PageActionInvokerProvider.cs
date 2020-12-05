﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure
{
    internal class PageActionInvokerProvider : IActionInvokerProvider
    {
        private readonly PageLoader _loader;
        private readonly IPageFactoryProvider _pageFactoryProvider;
        private readonly IPageModelFactoryProvider _modelFactoryProvider;
        private readonly IModelBinderFactory _modelBinderFactory;
        private readonly IRazorPageFactoryProvider _razorPageFactoryProvider;
        private readonly IActionDescriptorCollectionProvider _collectionProvider;
        private readonly IFilterProvider[] _filterProviders;
        private readonly IReadOnlyList<IValueProviderFactory> _valueProviderFactories;
        private readonly ParameterBinder _parameterBinder;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly ITempDataDictionaryFactory _tempDataFactory;
        private readonly MvcOptions _mvcOptions;
        private readonly HtmlHelperOptions _htmlHelperOptions;
        private readonly IPageHandlerMethodSelector _selector;
        private readonly DiagnosticListener _diagnosticListener;
        private readonly ILogger<PageActionInvoker> _logger;
        private readonly IActionResultTypeMapper _mapper;
        private readonly IActionContextAccessor _actionContextAccessor;

        private volatile InnerCache _currentCache;

        public PageActionInvokerProvider(
            PageLoader loader,
            IPageFactoryProvider pageFactoryProvider,
            IPageModelFactoryProvider modelFactoryProvider,
            IRazorPageFactoryProvider razorPageFactoryProvider,
            IActionDescriptorCollectionProvider collectionProvider,
            IEnumerable<IFilterProvider> filterProviders,
            ParameterBinder parameterBinder,
            IModelMetadataProvider modelMetadataProvider,
            IModelBinderFactory modelBinderFactory,
            ITempDataDictionaryFactory tempDataFactory,
            IOptions<MvcOptions> mvcOptions,
            IOptions<HtmlHelperOptions> htmlHelperOptions,
            IPageHandlerMethodSelector selector,
            DiagnosticListener diagnosticListener,
            ILoggerFactory loggerFactory,
            IActionResultTypeMapper mapper)
            : this(
                  loader,
                  pageFactoryProvider,
                  modelFactoryProvider,
                  razorPageFactoryProvider,
                  collectionProvider,
                  filterProviders,
                  parameterBinder,
                  modelMetadataProvider,
                  modelBinderFactory,
                  tempDataFactory,
                  mvcOptions,
                  htmlHelperOptions,
                  selector,
                  diagnosticListener,
                  loggerFactory,
                  mapper,
                  actionContextAccessor: null)
        {
        }

        public PageActionInvokerProvider(
            PageLoader loader,
            IPageFactoryProvider pageFactoryProvider,
            IPageModelFactoryProvider modelFactoryProvider,
            IRazorPageFactoryProvider razorPageFactoryProvider,
            IActionDescriptorCollectionProvider collectionProvider,
            IEnumerable<IFilterProvider> filterProviders,
            ParameterBinder parameterBinder,
            IModelMetadataProvider modelMetadataProvider,
            IModelBinderFactory modelBinderFactory,
            ITempDataDictionaryFactory tempDataFactory,
            IOptions<MvcOptions> mvcOptions,
            IOptions<HtmlHelperOptions> htmlHelperOptions,
            IPageHandlerMethodSelector selector,
            DiagnosticListener diagnosticListener,
            ILoggerFactory loggerFactory,
            IActionResultTypeMapper mapper,
            IActionContextAccessor actionContextAccessor)
        {
            _loader = loader;
            _pageFactoryProvider = pageFactoryProvider;
            _modelFactoryProvider = modelFactoryProvider;
            _modelBinderFactory = modelBinderFactory;
            _razorPageFactoryProvider = razorPageFactoryProvider;
            _collectionProvider = collectionProvider;
            _filterProviders = filterProviders.ToArray();
            _valueProviderFactories = mvcOptions.Value.ValueProviderFactories.ToArray();
            _parameterBinder = parameterBinder;
            _modelMetadataProvider = modelMetadataProvider;
            _tempDataFactory = tempDataFactory;
            _mvcOptions = mvcOptions.Value;
            _htmlHelperOptions = htmlHelperOptions.Value;
            _selector = selector;
            _diagnosticListener = diagnosticListener;
            _logger = loggerFactory.CreateLogger<PageActionInvoker>();
            _mapper = mapper;
            _actionContextAccessor = actionContextAccessor ?? ActionContextAccessor.Null;
        }

        public int Order { get; } = -1000;

        public void OnProvidersExecuting(ActionInvokerProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var actionContext = context.ActionContext;
            var actionDescriptor = actionContext.ActionDescriptor as PageActionDescriptor;
            if (actionDescriptor == null)
            {
                return;
            }

            var cache = CurrentCache;
            IFilterMetadata[] filters;
            if (!cache.Entries.TryGetValue(actionDescriptor, out var cacheEntry))
            {
                CompiledPageActionDescriptor compiledPageActionDescriptor;
                if (_mvcOptions.EnableEndpointRouting)
                {
                    // With endpoint routing, PageLoaderMatcherPolicy should have already produced a CompiledPageActionDescriptor.
                    compiledPageActionDescriptor = (CompiledPageActionDescriptor)actionDescriptor;
                }
                else
                {
                    // With legacy routing, we're forced to perform a blocking call. The exceptation is that
                    // in the most common case - build time views or successsively cached runtime views - this should finish synchronously.
                    compiledPageActionDescriptor = _loader.LoadAsync(actionDescriptor).GetAwaiter().GetResult();
                }

                actionContext.ActionDescriptor = compiledPageActionDescriptor;

                var filterFactoryResult = FilterFactory.GetAllFilters(_filterProviders, actionContext);
                filters = filterFactoryResult.Filters;
                cacheEntry = CreateCacheEntry(context, filterFactoryResult.CacheableFilters);
                cacheEntry = cache.Entries.GetOrAdd(actionDescriptor, cacheEntry);
            }
            else
            {
                filters = FilterFactory.CreateUncachedFilters(
                    _filterProviders,
                    actionContext,
                    cacheEntry.CacheableFilters);
            }

            context.Result = CreateActionInvoker(actionContext, cacheEntry, filters);
        }

        public void OnProvidersExecuted(ActionInvokerProviderContext context)
        {

        }

        private InnerCache CurrentCache
        {
            get
            {
                var current = _currentCache;
                var actionDescriptors = _collectionProvider.ActionDescriptors;

                if (current == null || current.Version != actionDescriptors.Version)
                {
                    current = new InnerCache(actionDescriptors.Version);
                    _currentCache = current;
                }

                return current;
            }
        }

        private PageActionInvoker CreateActionInvoker(
            ActionContext actionContext,
            PageActionInvokerCacheEntry cacheEntry,
            IFilterMetadata[] filters)
        {
            var pageContext = new PageContext(actionContext)
            {
                ActionDescriptor = cacheEntry.ActionDescriptor,
                ValueProviderFactories = new CopyOnWriteList<IValueProviderFactory>(_valueProviderFactories),
                ViewData = cacheEntry.ViewDataFactory(_modelMetadataProvider, actionContext.ModelState),
                ViewStartFactories = cacheEntry.ViewStartFactories.ToList(),
            };

            return new PageActionInvoker(
                _selector,
                _diagnosticListener,
                _logger,
                _actionContextAccessor,
                _mapper,
                pageContext,
                filters,
                cacheEntry,
                _parameterBinder,
                _tempDataFactory,
                _htmlHelperOptions);
        }

        private PageActionInvokerCacheEntry CreateCacheEntry(
            ActionInvokerProviderContext context,
            FilterItem[] cachedFilters)
        {
            var compiledActionDescriptor = (CompiledPageActionDescriptor)context.ActionContext.ActionDescriptor;

            var viewDataFactory = ViewDataDictionaryFactory.CreateFactory(compiledActionDescriptor.DeclaredModelTypeInfo);

            var pageFactory = _pageFactoryProvider.CreatePageFactory(compiledActionDescriptor);
            var pageDisposer = _pageFactoryProvider.CreatePageDisposer(compiledActionDescriptor);
            var propertyBinder = PageBinderFactory.CreatePropertyBinder(
                _parameterBinder,
                _modelMetadataProvider,
                _modelBinderFactory,
                compiledActionDescriptor);

            Func<PageContext, object> modelFactory = null;
            Action<PageContext, object> modelReleaser = null;
            if (compiledActionDescriptor.ModelTypeInfo != compiledActionDescriptor.PageTypeInfo)
            {
                modelFactory = _modelFactoryProvider.CreateModelFactory(compiledActionDescriptor);
                modelReleaser = _modelFactoryProvider.CreateModelDisposer(compiledActionDescriptor);
            }

            var viewStartFactories = GetViewStartFactories(compiledActionDescriptor);

            var handlerExecutors = GetHandlerExecutors(compiledActionDescriptor);
            var handlerBinders = GetHandlerBinders(compiledActionDescriptor);

            return new PageActionInvokerCacheEntry(
                compiledActionDescriptor,
                viewDataFactory,
                pageFactory,
                pageDisposer,
                modelFactory,
                modelReleaser,
                propertyBinder,
                handlerExecutors,
                handlerBinders,
                viewStartFactories,
                cachedFilters);
        }

        // Internal for testing.
        internal List<Func<IRazorPage>> GetViewStartFactories(CompiledPageActionDescriptor descriptor)
        {
            var viewStartFactories = new List<Func<IRazorPage>>();
            // Always pick up all _ViewStarts, including the ones outside the Pages root.
            foreach (var filePath in RazorFileHierarchy.GetViewStartPaths(descriptor.RelativePath))
            {
                var factoryResult = _razorPageFactoryProvider.CreateFactory(filePath);
                if (factoryResult.Success)
                {
                    viewStartFactories.Insert(0, factoryResult.RazorPageFactory);
                }
            }

            return viewStartFactories;
        }

        private static PageHandlerExecutorDelegate[] GetHandlerExecutors(CompiledPageActionDescriptor actionDescriptor)
        {
            if (actionDescriptor.HandlerMethods == null || actionDescriptor.HandlerMethods.Count == 0)
            {
                return Array.Empty<PageHandlerExecutorDelegate>();
            }

            var results = new PageHandlerExecutorDelegate[actionDescriptor.HandlerMethods.Count];

            for (var i = 0; i < actionDescriptor.HandlerMethods.Count; i++)
            {
                results[i] = ExecutorFactory.CreateExecutor(actionDescriptor.HandlerMethods[i]);
            }

            return results;
        }

        private PageHandlerBinderDelegate[] GetHandlerBinders(CompiledPageActionDescriptor actionDescriptor)
        {
            if (actionDescriptor.HandlerMethods == null || actionDescriptor.HandlerMethods.Count == 0)
            {
                return Array.Empty<PageHandlerBinderDelegate>();
            }

            var results = new PageHandlerBinderDelegate[actionDescriptor.HandlerMethods.Count];

            for (var i = 0; i < actionDescriptor.HandlerMethods.Count; i++)
            {
                results[i] = PageBinderFactory.CreateHandlerBinder(
                    _parameterBinder,
                    _modelMetadataProvider,
                    _modelBinderFactory,
                    actionDescriptor,
                    actionDescriptor.HandlerMethods[i],
                    _mvcOptions);
            }

            return results;
        }

        internal class InnerCache
        {
            public InnerCache(int version)
            {
                Version = version;
            }

            public ConcurrentDictionary<ActionDescriptor, PageActionInvokerCacheEntry> Entries { get; } =
                new ConcurrentDictionary<ActionDescriptor, PageActionInvokerCacheEntry>();

            public int Version { get; }
        }
    }
}
