﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure
{
    internal class PageLoaderMatcherPolicy : MatcherPolicy, IEndpointSelectorPolicy
    {
        private readonly PageLoader _loader;

        public PageLoaderMatcherPolicy(PageLoader loader)
        {
            if (loader == null)
            {
                throw new ArgumentNullException(nameof(loader));
            }

            _loader = loader;
        }

        public override int Order => int.MinValue + 100;

        public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            if (!ContainsDynamicEndpoints(endpoints))
            {
                // Pages are always dynamic endpoints.
                return false;
            }
            
            for (var i = 0; i < endpoints.Count; i++)
            {
                var page = endpoints[i].Metadata.GetMetadata<PageActionDescriptor>();
                if (page != null)
                {
                    // Found a page
                    return true;
                }
            }

            return false;
        }

        public Task ApplyAsync(HttpContext httpContext, EndpointSelectorContext context, CandidateSet candidates)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (candidates == null)
            {
                throw new ArgumentNullException(nameof(candidates));
            }

            for (var i = 0; i < candidates.Count; i++)
            {
                ref var candidate = ref candidates[i];
                var endpoint = candidate.Endpoint;

                var page = endpoint.Metadata.GetMetadata<PageActionDescriptor>();
                if (page != null)
                {
                    // We found an endpoint instance that has a PageActionDescriptor, but not a
                    // CompiledPageActionDescriptor. Update the CandidateSet.
                    var compiled = _loader.LoadAsync(page);
                    if (compiled.IsCompletedSuccessfully)
                    {
                        candidates.ReplaceEndpoint(i, compiled.Result.Endpoint, candidate.Values);
                    }
                    else
                    {
                        // In the most common case, GetOrAddAsync will return a synchronous result.
                        // Avoid going async since this is a fairly hot path.
                        return ApplyAsyncAwaited(candidates, compiled, i);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private async Task ApplyAsyncAwaited(CandidateSet candidates, Task<CompiledPageActionDescriptor> actionDescriptorTask, int index)
        {
            var compiled = await actionDescriptorTask;
            candidates.ReplaceEndpoint(index, compiled.Endpoint, candidates[index].Values);

            for (var i = index + 1; i < candidates.Count; i++)
            {
                var candidate = candidates[i];
                var endpoint = candidate.Endpoint;

                var page = endpoint.Metadata.GetMetadata<PageActionDescriptor>();
                if (page != null)
                {
                    compiled = await _loader.LoadAsync(page);
                    candidates.ReplaceEndpoint(i, compiled.Endpoint, candidates[i].Values);
                }
            }
        }
    }
}
