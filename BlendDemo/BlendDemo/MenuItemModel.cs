using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlendDemo
{
    public class MenuItemModel
    {
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// 导航地址
        /// </summary>
        public string PageKey { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string ItemTitle { get; set; }

        /// <summary>
        /// 字体图标
        /// </summary>
        public string StringIcon { get; set; }

        /// <summary>
        /// 多级菜单集合
        /// </summary>
        public ObservableCollection<MenuItemModel> Data { get; set; }
    }
}
