using System.Windows;
using System.Windows.Controls;

namespace BlendDemo
{
    public class MenuItemDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MenuItemModel mi)
            {
                FrameworkElement fe = container as FrameworkElement;
                return mi.Data != null && mi.Data.Count > 0 ? fe.FindResource("MenuItems") as DataTemplate : fe.FindResource("MenuButtons") as DataTemplate;
            }
            return null;
        }
    }
}
