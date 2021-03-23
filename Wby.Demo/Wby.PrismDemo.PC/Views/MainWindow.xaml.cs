using MaterialDesignThemes.Wpf;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using Wby.PrismDemo.PC.Infrastructure.Common;
using Wby.PrismDemo.PC.Infrastructure.Constants;

namespace Wby.PrismDemo.PC.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string Url = " https://www.baidu.com/";


        public MainWindow()
        {
            InitializeComponent();
            
            var ea = ContainerLocator.Current.Resolve<IEventAggregator>();
            var regionManager = ContainerLocator.Current.Resolve<IRegionManager>();

            //这里特别注意：需要注册ContentRegion
            if (regionManager != null)
            {
                SetRegionManager(regionManager, this.body, RegionNames.ContentRegion);
            }

            regionManager.RequestNavigate(RegionNames.ContentRegion, "HomeView");

            //控件Snackbar显示提示信息
            ea?.GetEvent<MessageSentEvent>().Subscribe(MainViewMessageReceived);

            //支持拖拽
            this.MouseDown += (sender,e)=> {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                    this.DragMove();
            };
        }

        private void SetRegionManager(IRegionManager regionManager, DependencyObject regionTarget, string regionName)
        {
            RegionManager.SetRegionName(regionTarget, regionName);
            RegionManager.SetRegionManager(regionTarget, regionManager);
        }

        private void MainViewMessageReceived(string msg)
        {
            var messageQueue = SnackbarThree.MessageQueue;
            messageQueue.Enqueue(msg);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenInBrowser(Url);
        }

        private static void OpenInBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
        }


        private void Btn_Min(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Btn_Max(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Btn_ExpandMenu(object sender, RoutedEventArgs e)
        {
            if (this.MENU.Width < 200)
            {
                this.IC.ItemTemplate = this.Resources["expanderTemplate"] as DataTemplate;
                AnimationHelper.CreateWidthChangedAnimation(this.MENU, 100, 200, new TimeSpan(0, 0, 0, 0, 300));
            }
            else
            {
                this.IC.ItemTemplate = this.Resources["groupTemplate"] as DataTemplate;
                AnimationHelper.CreateWidthChangedAnimation(this.MENU, 200, 100, new TimeSpan(0, 0, 0, 0, 300));
            }
                
        }
        
        //当提示信息关闭，且点击的是True时，退出系统
        private void View_Closing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return;
            Environment.Exit(0);
        }
    }
}
