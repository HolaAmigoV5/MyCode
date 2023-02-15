using GeneralUpdate.Core;
using GeneralUpdate.Core.Bootstrap;
using GeneralUpdate.Core.Domain.Entity;
using GeneralUpdate.Core.Domain.Enum;
using GeneralUpdate.Core.Strategys.PlatformWindows;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GeneralUpdateDemo.Upgrade
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewData viewData;

        public MainWindow()
        {
            InitializeComponent();

            viewData = new ViewData();
            DataContext = viewData;
        }

        public MainWindow(string args)
        {
            InitializeComponent();

            viewData = new ViewData();
            DataContext = viewData;
            Upgrade(args);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Upgrade(string args)
        {
            Task.Run(async () =>
            {
                var bootstrap = new GeneralUpdateBootstrap();

                //单个或多个更新包下载通知事件
                bootstrap.MutiDownloadProgressChanged += OnMutiDownloadProgressChanged;
                //单个或多个更新包下载速度、剩余下载事件、当前下载版本信息通知事件
                bootstrap.MutiDownloadStatistics += OnMutiDownloadStatistics;
                //单个或多个更新包下载完成
                bootstrap.MutiDownloadCompleted += OnMutiDownloadCompleted;
                //下载过程出现的异常通知
                bootstrap.MutiDownloadError += OnMutiDownloadError;
                //整个更新过程出现的任何问题都会通过这个事件通知
                bootstrap.Exception += OnException;

                bootstrap.Strategy<WindowsStrategy>().
                    Option(UpdateOption.Encoding, Encoding.Default).
                    Option(UpdateOption.DownloadTimeOut, 60).
                    Option(UpdateOption.Format, Format.ZIP).Remote(args);

                await bootstrap.LaunchTaskAsync();
            });
        }

        #region GeneralClientBootstrap_Event
        private void OnMutiDownloadProgressChanged(object sender, MutiDownloadProgressChangedEventArgs e)
        {
            //e.TotalBytesToReceive 当前更新包需要下载的总大小
            //e.ProgressValue 当前进度值
            //e.ProgressPercentage 当前进度的百分比
            //e.Version 当前下载的版本信息
            //e.Type 当前正在执行的操作  1.ProgressType.Check 检查版本信息中 2.ProgressType.Donwload 正在下载当前版本 3. ProgressType.Updatefile 更新当前版本 4. ProgressType.Done更新完成 5.ProgressType.Fail 更新失败
            //e.BytesReceived 已下载大小

            viewData.HandledCount = (int)e.BytesReceived;
            viewData.TotalCount = (int)e.TotalBytesToReceive;
            Debug.WriteLine($"Total:{e.TotalBytesToReceive}, Received:{e.BytesReceived}, " +
                $"Value:{e.ProgressValue}, Percentage:{e.ProgressPercentage}, Version:{e.Version}, Type:{e.Type}");
        }

        private void OnMutiDownloadStatistics(object sender, MutiDownloadStatisticsEventArgs e)
        {
            //e.Remaining 剩余下载时间
            //e.Speed 下载速度
            //e.Version 当前下载的版本信息
            viewData.HandleText = $"下载速度：{e.Speed}, 剩余时间：{e.Remaining.Minute}:{e.Remaining.Second}";
        }

        private void OnMutiDownloadCompleted(object sender, MutiDownloadCompletedEventArgs e)
        {
            viewData.MainText = "更新完成，即将启动主程序";
            viewData.MainTextColor = new SolidColorBrush(Colors.ForestGreen);
        }

        private void OnMutiDownloadError(object sender, MutiDownloadErrorEventArgs e)
        {
            var info = e.Version as VersionInfo;
            viewData.HandleText = $"文件{info?.Name}下载错误！";
        }

        private void OnException(object sender, GeneralUpdate.Core.Bootstrap.ExceptionEventArgs e)
        {
            // 出现异常
            Debug.WriteLine(e.Exception.InnerException);
        }
        #endregion
    }
}
