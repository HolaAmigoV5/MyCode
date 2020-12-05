// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    /// <summary>
    /// An error that occured during model binding and validation.
    /// </summary>
    public class ModelError
    {
        /// <summary>
        /// Intiializes a new instance of <see cref="ModelError"/> with the specified <paramref name="exception"/>.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception"/>.</param>
        public ModelError(Exception exception)
            : this(exception, errorMessage: null)
        {
        }

        /// <summary>
        /// Intiializes a new instance of <see cref="ModelError"/> with the specified <paramref name="exception"/>
        /// and specified <paramref name="errorMessage"/>.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception"/>.</param>
        /// <param name="errorMessage">The error message.</param>
        public ModelError(Exception exception, string errorMessage)
            : this(errorMessage)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ModelError"/> with the specified <paramref name="errorMessage"/>.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public ModelError(string errorMessage)
        {
            ErrorMessage = errorMessage ?? string.Empty;
        }

        /// <summary>
        /// Gets the <see cref="System.Exception"/> associated with this <see cref="ModelError"/> instance.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the error message associated with this <see cref="ModelError"/> instance.
        /// </summary>
        public string ErrorMessage { get; }
    }
}
