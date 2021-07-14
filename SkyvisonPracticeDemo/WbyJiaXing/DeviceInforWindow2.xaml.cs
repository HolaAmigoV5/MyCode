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
using System.Windows.Shapes;

namespace WbyJiaXing
{
    /// <summary>
    /// DeviceInforWindow2.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceInforWindow2 : Window
    {
        public DeviceInforWindow2()
        {
            InitializeComponent();

            this.MouseDown += (sender, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };
        }
    }
}
