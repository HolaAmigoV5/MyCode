using PrismDemo.Services;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace BootstrapperShell.Views
{
    /// <summary>
    /// Header.xaml 的交互逻辑
    /// </summary>
    public partial class Header : UserControl
    {
        IHomePageService _service;
        public Header(IHomePageService service)
        {
            InitializeComponent();

            _service = service;

            // 读取配置文件信息
            txt.Text = $"我是Header: {ConfigurationManager.AppSettings["BusinessApiServer"]}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 调用配置文件中注入的服务（带参数）。
            var res = _service.SayHi();
            MessageBox.Show(res);
            e.Handled = true;
        }
    }
}
