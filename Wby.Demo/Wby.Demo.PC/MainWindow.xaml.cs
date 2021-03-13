using System.Windows;

namespace Wby.Demo.PC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnMax(object sender, RoutedEventArgs e)
        {
            UnregisterName("");
            SetWindowSize();
        }

        private void btnClose(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void btnMin(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void SetWindowSize()
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }
    }
}
