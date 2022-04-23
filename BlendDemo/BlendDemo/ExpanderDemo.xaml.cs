using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlendDemo
{
    /// <summary>
    /// ExpanderDemo.xaml 的交互逻辑
    /// </summary>
    public partial class ExpanderDemo : UserControl
    {
        public ExpanderDemo()
        {
            InitializeComponent();
            DataContext = this;

            MenuItems = new ObservableCollection<MenuItemModel>() {
                new MenuItemModel()
                {
                    ItemTitle="一级菜单栏1" ,
                    StringIcon="&#xf13d;",
                    Data=new ObservableCollection<MenuItemModel>()
                    {
                        new MenuItemModel()
                        {
                            ItemTitle="二级菜单栏",
                            StringIcon="&#xf206;",
                            Data=new ObservableCollection<MenuItemModel>(){new MenuItemModel() { ItemTitle="三级菜单栏", StringIcon= "&#xf1b9;" } }
                        }
                    }
                },
                new MenuItemModel(){ItemTitle="一级菜单栏2",StringIcon="&#xf1ae;" },
                new MenuItemModel(){ItemTitle="一级菜单栏3", StringIcon="&#xf1cb;"}
            };
        }

        public ObservableCollection<MenuItemModel> MenuItems { get; set; }
    }
}
