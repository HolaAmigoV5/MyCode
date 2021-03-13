using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wby.Demo.PC.Common
{
    /// <summary>
    /// UI可视化帮助类
    /// </summary>
    public static class VisualHelper
    {
        /// <summary>
        /// 给DataGrid设置列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="frameworkElement"></param>
        /// <param name="name">表格名称</param>
        /// <param name="type">表格绑定的类型</param>
        public static void SetDataGridColumns<T>(T frameworkElement, string name, Type type) where T : FrameworkElement
        {
            if (frameworkElement == null)
                throw new ArgumentNullException(nameof(frameworkElement));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (frameworkElement.FindName(name) is DataGrid dataGrid)
            {
                var properties = type.GetProperties();
                foreach (PropertyInfo item in properties)
                {
                    var attr = item.GetCustomAttribute<DescriptionAttribute>();
                    if (attr != null)
                    {
                        dataGrid.Columns.Add(new DataGridTextColumn()
                        {
                            Header = attr.Description,
                            Binding = new Binding(item.Name)
                        });
                    }
                }
            }
        }
    }
}
