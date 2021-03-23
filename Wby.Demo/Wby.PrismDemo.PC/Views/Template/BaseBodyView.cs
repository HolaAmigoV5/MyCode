using System.Windows;
using System.Windows.Controls;

namespace Wby.PrismDemo.PC.Views.Template
{
    public class BaseBodyView : ContentControl
    {
        //这里直接继承自ContentControl空间，已经定义好了ContentTemplate属性。因此以下代码注释，如果继承自Control空间，以下代码保留
        //public static readonly DependencyProperty ContentTemplateProperty =
        //       DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(BaseBodyView),
        //           new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentTemplateChanged)));


        ///// <summary>
        ///// ContentTemplate is the template used to display the content of the control.
        ///// </summary>
        //public DataTemplate ContentTemplate
        //{
        //    get { return (DataTemplate)GetValue(ContentTemplateProperty); }
        //    set { SetValue(ContentTemplateProperty, value); }
        //}

        ///// <summary>
        ///// Called when ContentTemplateProperty is invalidated on "d."
        ///// </summary>
        //private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        //{
        //    var oldValue = args.OldValue;
        //    var newValue = args.NewValue;
        //    if (oldValue == newValue)
        //        return;

        //    var target = d as BaseBodyView;
        //    target?.OnContentTemplateChanged(oldValue, newValue);
        //}

        //protected virtual void OnContentTemplateChanged(object oldValue, object newValue)
        //{

        //}
    }
}
