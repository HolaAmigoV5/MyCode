using System.Windows;
using Wby.Demo.PC.Common;

namespace Wby.Demo.PC
{
    /// <summary>
    /// MaterialDesignMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MaterialDesignMainWindow : Window
    {
        private const string Url = " https://www.baidu.com/";

        public MaterialDesignMainWindow()
        {
            InitializeComponent();
        }

        private void btnGithub(object sender, RoutedEventArgs e)
        {
            Link.OpenInBrowser(Url);
        }

        private void btnBilibili(object sender, RoutedEventArgs e)
        {
            Link.OpenInBrowser(Url);
        }

        private void btnQQ(object sender, RoutedEventArgs e)
        {
            Link.OpenInBrowser(Url);
        }
    }
}
