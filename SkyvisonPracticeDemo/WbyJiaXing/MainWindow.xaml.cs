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

namespace WbyJiaXing
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DeviceInforWindow DiWin1 = null;
        private DeviceInforWindow2 DiWin2 = null;
        private RenderControlService rService;

        public MainWindow()
        {
            InitializeComponent();

            leftRegion.Content = new MainControl();

            //加载3D
            rService = new RenderControlService();
            rService.RenderControl.BeginInit();
            MapControlHost.Child = rService.RenderControl;
            rService.RenderControl.EndInit();

            //支持拖动
            this.MouseDown += (sender, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };
        }

        private void RadioBtn(object sender, RoutedEventArgs e)
        {
            var name = (sender as RadioButton).Name;
            if (name != null)
                leftRegion.Content = Activator.CreateInstance(null, $"WbyJiaXing.{name}").Unwrap();
            e.Handled = true;
        }

        private void GsBtn(object sender, RoutedEventArgs e)
        {
            var name = (sender as RadioButton).Name;
            if (name == "DeviceInforWindow")
            {
                if (DiWin1 == null)
                    DiWin1 = new DeviceInforWindow();
                DiWin1.Show();
            }
            else
            {
                if (DiWin2 == null)
                    DiWin2 = new DeviceInforWindow2();
                DiWin2.Show();
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rService.InitlizeRenderControl();
        }
    }
}
