using DaJuTestDemo.Core;
using DaJuTestDemo.Core.Layout;
using I3DMapOperation;
using Prism.Ioc;
using Prism.Regions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace DaJuTestDemo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IMapOperation mapOperation;
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            mapOperation = ContainerLocator.Current.Resolve<IMapOperation>();
            regionManager.RegisterViewWithRegion(RegionNames.MainPageContentRegion, typeof(HomePageLayout));
            Loaded += MainWindow_Loaded;
            KeyUp += MainWindow_KeyUp;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Screen[] _screens = Screen.AllScreens;
            if (_screens.Length > 1)
            {
                Left = Screen.PrimaryScreen.WorkingArea.Width;
                Top = 0;
                WindowState = WindowState.Maximized;
            }
            else
                WindowState = WindowState.Maximized;
        }

        private void MainWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (System.Windows.MessageBox.Show("确定退出系统吗？", "提示", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Close();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }
            else if (e.Key == Key.F2)
            {
                // 重置摄像机位置
                mapOperation.InitlizedCameraPosition();
            }
        }

        private void btn_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
            if (btn.Name == "btnTime")
            {
                timePopup.IsOpen = true;
            }
            else
            {
                speedPopup.IsOpen = true;
            }
            e.Handled = true;
        }
    }
}
