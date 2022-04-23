using System;
using System.Collections.Generic;
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
    /// CustomizeBehavior.xaml 的交互逻辑
    /// </summary>
    public partial class CustomizeBehavior : UserControl
    {
        public CustomizeBehavior()
        {
            InitializeComponent();

            DataContext = this;
            NumList = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
        }

        public List<string> NumList { get; set; }
    }
}
