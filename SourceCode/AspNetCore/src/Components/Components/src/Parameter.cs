// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Components
{
    /// <summary>
    /// Represents a single parameter supplied to an <see cref="IComponent"/>
    /// by its parent in the render tree.
    /// </summary>
    public readonly struct Parameter
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value being supplied for the parameter.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets a value to indicate whether the parameter is cascading, meaning that it
        /// was supplied by a <see cref="CascadingValue{T}"/>.
        /// </summary>
        public bool Cascading { get; }

        internal Parameter(string name, object value, bool cascading)
        {
            Name = name;
            Value = value;
            Cascading = cascading;
        }
    }
}
