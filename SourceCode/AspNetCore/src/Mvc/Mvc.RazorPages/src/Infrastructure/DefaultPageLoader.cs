﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure
{
    internal class DefaultPageLoader : PageLoader
    {
        private readonly IActionDescriptorCollectionProvider _collectionProvider;
        private readonly IPageApplicationModelProvider[] _applicationModelProviders;
        private readonly IViewCompilerProvider _viewCompilerProvider;
        private readonly ActionEndpointFactory _endpointFactory;
        private readonly PageConventionCollection _conventions;
        private readonly FilterCollection _globalFilters;
        private volatile InnerCache _currentCache;

        public DefaultPageLoader(
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            IEnumerable<IPageApplicationModelProvider> applicationModelProviders,
            IViewCompilerProvider viewCompilerProvider,
            ActionEndpointFactory endpointFactory,
            IOptions<RazorPagesOptions> pageOptions,
            IOptions<MvcOptions> mvcOptions)
        {
            _collectionProvider = actionDescriptorCollectionProvider;
            _applicationModelProviders = applicationModelProviders
                .OrderBy(p => p.Order)
                .ToArray();

            _viewCompilerProvider = viewCompilerProvider;
            _endpointFactory = endpointFactory;
            _conventions = pageOptions.Value.Conventions;
            _globalFilters = mvcOptions.Value.Filters;
        }

        private IViewCompiler Compiler => _viewCompilerProvider.GetCompiler();

        private ConcurrentDictionary<PageActionDescriptor, Task<CompiledPageActionDescriptor>> CurrentCache
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

                return current.Entries;
            }
        }

        public override Task<CompiledPageActionDescriptor> LoadAsync(PageActionDescriptor actionDescriptor)
        {
            if (actionDescriptor == null)
            {
                throw new ArgumentNullException(nameof(actionDescriptor));
            }

            var cache = CurrentCache;
            if (cache.TryGetValue(actionDescriptor, out var compiledDescriptorTask))
            {
                return compiledDescriptorTask;
            }

            return cache.GetOrAdd(actionDescriptor, LoadAsyncCore(actionDescriptor));
        }

        private async Task<CompiledPageActionDescriptor> LoadAsyncCore(PageActionDescriptor actionDescriptor)
        {
            var viewDescriptor = await Compiler.CompileAsync(actionDescriptor.RelativePath);
            var context = new PageApplicationModelProviderContext(actionDescriptor, viewDescriptor.Type.GetTypeInfo());
            for (var i = 0; i < _applicationModelProviders.Length; i++)
            {
                _applicationModelProviders[i].OnProvidersExecuting(context);
            }

            for (var i = _applicationModelProviders.Length - 1; i >= 0; i--)
            {
                _applicationModelProviders[i].OnProvidersExecuted(context);
            }

            ApplyConventions(_conventions, context.PageApplicationModel);

            var compiled = CompiledPageActionDescriptorBuilder.Build(context.PageApplicationModel, _globalFilters);

            // We need to create an endpoint for routing to use and attach it to the CompiledPageActionDescriptor...
            // routing for pages is two-phase. First we perform routing using the route info - we can do this without
            // compiling/loading the page. Then once we have a match we load the page and we can create an endpoint
            // with all of the information we get from the compiled action descriptor.
            var endpoints = new List<Endpoint>();
            _endpointFactory.AddEndpoints(endpoints, compiled, Array.Empty<ConventionalRouteEntry>(), Array.Empty<Action<EndpointBuilder>>());

            // In some test scenarios there's no route so the endpoint isn't created. This is fine because
            // it won't happen for real.
            compiled.Endpoint = endpoints.SingleOrDefault();

            return compiled;
        }

        internal static void ApplyConventions(
            PageConventionCollection conventions,
            PageApplicationModel pageApplicationModel)
        {
            var applicationModelConventions = GetConventions<IPageApplicationModelConvention>(pageApplicationModel.HandlerTypeAttributes);
            foreach (var convention in applicationModelConventions)
            {
                convention.Apply(pageApplicationModel);
            }

            var handlers = pageApplicationModel.HandlerMethods.ToArray();
            foreach (var handlerModel in handlers)
            {
                var handlerModelConventions = GetConventions<IPageHandlerModelConvention>(handlerModel.Attributes);
                foreach (var convention in handlerModelConventions)
                {
                    convention.Apply(handlerModel);
                }

                var parameterModels = handlerModel.Parameters.ToArray();
                foreach (var parameterModel in parameterModels)
                {
                    var parameterModelConventions = GetConventions<IParameterModelBaseConvention>(parameterModel.Attributes);
                    foreach (var convention in parameterModelConventions)
                    {
                        convention.Apply(parameterModel);
                    }
                }
            }

            var properties = pageApplicationModel.HandlerProperties.ToArray();
            foreach (var propertyModel in properties)
            {
                var propertyModelConventions = GetConventions<IParameterModelBaseConvention>(propertyModel.Attributes);
                foreach (var convention in propertyModelConventions)
                {
                    convention.Apply(propertyModel);
                }
            }

            IEnumerable<TConvention> GetConventions<TConvention>(
                IReadOnlyList<object> attributes)
            {
                return Enumerable.Concat(
                    conventions.OfType<TConvention>(),
                    attributes.OfType<TConvention>());
            }
        }

        private sealed class InnerCache
        {
            public InnerCache(int version)
            {
                Version = version;
                Entries = new ConcurrentDictionary<PageActionDescriptor, Task<CompiledPageActionDescriptor>>();
            }

            public ConcurrentDictionary<PageActionDescriptor, Task<CompiledPageActionDescriptor>> Entries { get; }

            public int Version { get; }
        }
    }
}