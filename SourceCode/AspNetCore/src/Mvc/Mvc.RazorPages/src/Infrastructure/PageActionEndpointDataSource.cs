﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure
{
    internal class PageActionEndpointDataSource : ActionEndpointDataSourceBase
    {
        private readonly ActionEndpointFactory _endpointFactory;

        public PageActionEndpointDataSource(IActionDescriptorCollectionProvider actions, ActionEndpointFactory endpointFactory)
            : base(actions)
        {
            _endpointFactory = endpointFactory;

            // IMPORTANT: this needs to be the last thing we do in the constructor. 
            // Change notifications can happen immediately!
            Subscribe();
        }

        protected override List<Endpoint> CreateEndpoints(IReadOnlyList<ActionDescriptor> actions, IReadOnlyList<Action<EndpointBuilder>> conventions)
        {
            var endpoints = new List<Endpoint>();
            for (var i = 0; i < actions.Count; i++)
            {
                if (actions[i] is PageActionDescriptor action)
                {
                    _endpointFactory.AddEndpoints(endpoints, action, Array.Empty<ConventionalRouteEntry>(), conventions);
                }
            }

            return endpoints;
        }
    }
}

