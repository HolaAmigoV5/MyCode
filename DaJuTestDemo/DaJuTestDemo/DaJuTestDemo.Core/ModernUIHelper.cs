﻿using System.ComponentModel;
using System.Windows;

namespace DaJuTestDemo.Core
{
    public static class ModernUIHelper
    {
        private static bool? isInDesignMode;
        /// <summary>
        /// Determines whether the current code is executed in a design time environment such as Visual Studio or Blend.
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                if (!isInDesignMode.HasValue)
                {
                    isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
                }

                return isInDesignMode.Value;
            }
        }

    }
}
