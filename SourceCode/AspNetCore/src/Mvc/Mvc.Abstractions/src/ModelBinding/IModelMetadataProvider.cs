// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    /// <summary>
    /// A provider that can supply instances of <see cref="ModelMetadata"/>.
    /// </summary>
    /// <remarks>
    /// While not obsolete, implementing or using <see cref="ModelMetadataProvider" /> is preferred over <see cref="IModelMetadataProvider"/>.
    /// </remarks>
    public interface IModelMetadataProvider
    {
        /// <summary>
        /// Supplies metadata describing a <see cref="Type"/>.
        /// </summary>
        /// <param name="modelType">The <see cref="Type"/>.</param>
        /// <returns>A <see cref="ModelMetadata"/> instance describing the <see cref="Type"/>.</returns>
        ModelMetadata GetMetadataForType(Type modelType);

        /// <summary>
        /// Supplies metadata describing the properties of a <see cref="Type"/>.
        /// </summary>
        /// <param name="modelType">The <see cref="Type"/>.</param>
        /// <returns>A set of <see cref="ModelMetadata"/> instances describing properties of the <see cref="Type"/>.</returns>
        IEnumerable<ModelMetadata> GetMetadataForProperties(Type modelType);
    }
}