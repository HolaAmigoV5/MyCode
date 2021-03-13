﻿using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Wby.Demo.PC.Common
{
    public class AnimationHelper
    {
        /// <summary>
        /// 创建宽度改变动画
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="Form">起始值</param>
        /// <param name="To">结束值</param>
        /// <param name="span">间隔</param>
        public static void CreateWidthChangedAnimation(UIElement element, double Form, double To, TimeSpan span)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation
            {
                From = Form,
                To = To,
                Duration = span
            };
            Storyboard.SetTarget(doubleAnimation, element);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Width"));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }
    }
}
