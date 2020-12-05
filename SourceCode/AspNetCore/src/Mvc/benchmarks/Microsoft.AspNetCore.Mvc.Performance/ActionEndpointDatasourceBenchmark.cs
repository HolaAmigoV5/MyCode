﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace Microsoft.AspNetCore.Mvc.Performance
{
    public class ActionEndpointDataSourceBenchmark
    {
        private const string DefaultRoute = "{Controller=Home}/{Action=Index}/{id?}";

        // Attribute routes can't have controller and action as parameters, so we edit the
        // route template in the test to make it more realistic.
        private const string ControllerReplacementToken = "{Controller=Home}";
        private const string ActionReplacementToken = "{Action=Index}";

        private MockActionDescriptorCollectionProvider _conventionalActionProvider;
        private MockActionDescriptorCollectionProvider _attributeActionProvider;
        private List<ConventionalRouteEntry> _routes;

        [Params(1, 100, 1000)]
        public int ActionCount;

        [GlobalSetup]
        public void Setup()
        {
            _conventionalActionProvider = new MockActionDescriptorCollectionProvider(
                Enumerable.Range(0, ActionCount).Select(i => CreateConventionalRoutedAction(i)).ToList()
                );

            _attributeActionProvider = new MockActionDescriptorCollectionProvider(
                Enumerable.Range(0, ActionCount).Select(i => CreateAttributeRoutedAction(i)).ToList()
                );

            _routes = new List<ConventionalRouteEntry>
            {
                new ConventionalRouteEntry(
                    "Default",
                    DefaultRoute,
                    new RouteValueDictionary(),
                    new Dictionary<string, object>(),
                    new RouteValueDictionary())
            };
        }

        [Benchmark]
        public void AttributeRouteEndpoints()
        {
            var endpointDataSource = CreateDataSource(_attributeActionProvider);
            var endpoints = endpointDataSource.Endpoints;

            AssertHasEndpoints(endpoints);
        }

        [Benchmark]
        public void ConventionalEndpoints()
        {
            var dataSource = CreateDataSource(_conventionalActionProvider);
            for (var i = 0; i < _routes.Count; i++)
            {
                dataSource.AddRoute(_routes[i]);
            }

            var endpoints = dataSource.Endpoints;
            AssertHasEndpoints(endpoints);
        }

        private ActionDescriptor CreateAttributeRoutedAction(int id)
        {
            var routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Controller"] = "Controller" + id,
                ["Action"] = "Index"
            };

            var template = DefaultRoute
                .Replace(ControllerReplacementToken, routeValues["Controller"])
                .Replace(ActionReplacementToken, routeValues["Action"]);

            return new ActionDescriptor
            {
                RouteValues = routeValues,
                DisplayName = "Action " + id,
                AttributeRouteInfo = new AttributeRouteInfo()
                {
                    Template = template,
                }
            };
        }

        private ActionDescriptor CreateConventionalRoutedAction(int id)
        {
            return new ActionDescriptor
            {
                RouteValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Controller"] = "Controller" + id,
                    ["Action"] = "Index"
                },
                DisplayName = "Action " + id
            };
        }

        private ActionEndpointDataSource CreateDataSource(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            var dataSource = new ActionEndpointDataSource(
                actionDescriptorCollectionProvider,
                new ActionEndpointFactory(new MockRoutePatternTransformer()));

            return dataSource;
        }

        private class MockRoutePatternTransformer : RoutePatternTransformer
        {
            public override RoutePattern SubstituteRequiredValues(RoutePattern original, object requiredValues)
            {
                return original;
            }
        }

        private class MockActionDescriptorCollectionProvider : IActionDescriptorCollectionProvider
        {
            public MockActionDescriptorCollectionProvider(List<ActionDescriptor> actionDescriptors)
            {
                ActionDescriptors = new ActionDescriptorCollection(actionDescriptors, 0);
            }

            public ActionDescriptorCollection ActionDescriptors { get; }
        }

        private class MockParameterPolicyFactory : ParameterPolicyFactory
        {
            public override IParameterPolicy Create(RoutePatternParameterPart parameter, string inlineText)
            {
                throw new NotImplementedException();
            }

            public override IParameterPolicy Create(RoutePatternParameterPart parameter, IParameterPolicy parameterPolicy)
            {
                throw new NotImplementedException();
            }
        }

        private static void AssertHasEndpoints(IReadOnlyList<Http.Endpoint> endpoints)
        {
            if (endpoints.Count == 0)
            {
                throw new InvalidOperationException("Expected endpoints from data source.");
            }
        }
    }
}
