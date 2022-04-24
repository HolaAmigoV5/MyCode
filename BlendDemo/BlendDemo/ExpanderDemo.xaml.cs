using System.Collections.ObjectModel;
using System.Windows.Controls;

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
                    StringIcon="\f13d",
                    Data=new ObservableCollection<MenuItemModel>()
                    {
                        new MenuItemModel()
                        {
                            ItemTitle="二级菜单栏",
                            StringIcon="\f206",
                            Data=new ObservableCollection<MenuItemModel>(){new MenuItemModel() { ItemTitle="三级菜单栏", StringIcon= "\f1b9" } }
                        }
                    }
                },
                new MenuItemModel(){ItemTitle="一级菜单栏2",StringIcon="\f1ae" },
                new MenuItemModel(){ItemTitle="一级菜单栏3", StringIcon="\f1cb"}
            };
        }

        public ObservableCollection<MenuItemModel> MenuItems { get; set; }
    }
}
