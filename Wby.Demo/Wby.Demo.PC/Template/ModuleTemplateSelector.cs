using System.Windows;
using System.Windows.Controls;
using Wby.Demo.ViewModel.Common;

namespace Wby.Demo.PC.Template
{
    public class ModuleTemplateSelector: DataTemplateSelector
    {
        public DataTemplate GroupTemplate { get; set; }
        public DataTemplate ExpanderTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ModuleGroup group = (ModuleGroup)item;
            //if (group != null)
            //{
            //    if (!group.ContractionTemplate)
            //        return ExpanderTemplate;
            //    else
            //        return GroupTemplate;
            //}
            if (group != null)
            {
                return !group.ContractionTemplate ? ExpanderTemplate : GroupTemplate;
            }
            return ExpanderTemplate;
        }
    }
}
