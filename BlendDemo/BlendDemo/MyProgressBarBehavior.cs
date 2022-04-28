using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using System.Windows.Media.Animation;
using System.Windows;

namespace BlendDemo
{
    public class MyProgressBarBehavior:Behavior<ProgressBar>
    {
        bool isRun = false;
        protected override void OnAttached()
        {
            base.OnAttached();

            var pb = AssociatedObject;
            pb.ValueChanged += Pb_ValueChanged;
        }

        private void Pb_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (isRun) return;
            isRun = true;
            ProgressBar progress = sender as ProgressBar;
            DoubleAnimation doubleAnimation = new DoubleAnimation(e.OldValue, e.NewValue, new Duration(TimeSpan.FromMilliseconds(300)), FillBehavior.Stop);
            doubleAnimation.Completed += DoubleAnimation_Completed;
            progress.BeginAnimation(ProgressBar.ValueProperty,doubleAnimation);
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            isRun = false;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            var pb = AssociatedObject;
            pb.ValueChanged -= Pb_ValueChanged;
        }
    }
}
