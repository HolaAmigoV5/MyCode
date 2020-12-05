﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.StackTrace.Sources
{
    /// <summary>
    /// Contains the source code where the exception occurred.
    /// </summary>
    internal class StackFrameSourceCodeInfo
    {
        /// <summary>
        /// Function containing instruction
        /// </summary>
        public string Function { get; set; }

        /// <summary>
        /// File containing the instruction
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// The line number of the instruction
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// The line preceding the frame line
        /// </summary>
        public int PreContextLine { get; set; }

        /// <summary>
        /// Lines of code before the actual error line(s).
        /// </summary>
        public IEnumerable<string> PreContextCode { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Line(s) of code responsible for the error.
        /// </summary>
        public IEnumerable<string> ContextCode { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Lines of code after the actual error line(s).
        /// </summary>
        public IEnumerable<string> PostContextCode { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Specific error details for this stack frame.
        /// </summary>
        public string ErrorDetails { get; set; }
    }
}
