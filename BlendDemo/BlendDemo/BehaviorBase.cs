using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace BlendDemo
{
    public class BehaviorBase : Behavior<FrameworkElement>
    {
        /// <summary>
        /// 当前元素即将要加载的效果
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is StackPanel sp)
            {
                for (int i = 0; i < sp.Children.Count; i++)
                {
                    if (sp.Children[i] is FrameworkElement item)
                    {
                        item.Margin = new Thickness(sp.ActualWidth, 0, 0, 0);
                        ThicknessAnimation animation = new ThicknessAnimation(new Thickness(sp.ActualHeight, 0, 0, 0), new Thickness(20, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.3)))
                        {
                            BeginTime = TimeSpan.FromSeconds(i * 0.2)
                        };
                        item.BeginAnimation(FrameworkElement.MarginProperty, animation);
                    }
                }
            }
        }

        /// <summary>
        /// 当前元素即将要卸载
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }
    }
}
