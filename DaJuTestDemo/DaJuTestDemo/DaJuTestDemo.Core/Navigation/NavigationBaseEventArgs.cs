﻿using DaJuTestDemo.Core.Controls;
using System;

namespace DaJuTestDemo.Core
{
    /// <summary>
    /// Defines the base navigation event arguments.
    /// </summary>
    public abstract class NavigationBaseEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the frame that raised this event.
        /// </summary>
        public ModernFrame Frame { get; internal set; }
        /// <summary>
        /// Gets the source uri for the target being navigated to.
        /// </summary>
        public Uri Source { get; internal set; }
    }
}
