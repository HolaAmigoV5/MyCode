// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Microsoft.AspNetCore.Mvc.Abstractions
{
    /// <summary>
    /// Describes an MVC action.
    /// </summary>
    public class ActionDescriptor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ActionDescriptor"/>.
        /// </summary>
        public ActionDescriptor()
        {
            Id = Guid.NewGuid().ToString();
            Properties = new Dictionary<object, object>();
            RouteValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets an id which uniquely identifies the action.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets or sets the collection of route values that must be provided by routing
        /// for the action to be selected.
        /// </summary>
        public IDictionary<string, string> RouteValues { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Routing.AttributeRouteInfo"/>.
        /// </summary>
        public AttributeRouteInfo AttributeRouteInfo { get; set; }

        /// <summary>
        /// The set of constraints for this action. Must all be satisfied for the action to be selected.
        /// </summary>
        public IList<IActionConstraintMetadata> ActionConstraints { get; set; }

        /// <summary>
        /// Gets or sets the endpoint metadata for this action.
        /// </summary>
        public IList<object> EndpointMetadata { get; set; }

        /// <summary>
        /// The set of parameters associated with this action.
        /// </summary>
        public IList<ParameterDescriptor> Parameters { get; set; }

        /// <summary>
        /// The set of properties which are model bound.
        /// </summary>
        public IList<ParameterDescriptor> BoundProperties { get; set; }

        /// <summary>
        /// The set of filters associated with this action.
        /// </summary>
        public IList<FilterDescriptor> FilterDescriptors { get; set; }

        /// <summary>
        /// A friendly name for this action.
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Stores arbitrary metadata properties associated with the <see cref="ActionDescriptor"/>.
        /// </summary>
        public IDictionary<object, object> Properties { get; set; }
    }
}
