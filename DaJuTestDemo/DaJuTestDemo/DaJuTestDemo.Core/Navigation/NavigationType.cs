﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DaJuTestDemo.Core
{
    /// <summary>
    /// Identifies the types of navigation that are supported.
    /// </summary>
    public enum NavigationType
    {
        /// <summary>
        /// Navigating to new content.
        /// </summary>
        New,
        /// <summary>
        /// Navigating back in the back navigation history.
        /// </summary>
        Back,
        /// <summary>
        /// Reloading the current content.
        /// </summary>
        Refresh
    }
}
