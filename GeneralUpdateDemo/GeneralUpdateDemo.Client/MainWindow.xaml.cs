using GeneralUpdate.ClientCore;
using GeneralUpdate.ClientCore.Hubs;
using GeneralUpdate.Core.Bootstrap;
using GeneralUpdate.Core.Domain.Entity;
using GeneralUpdate.Core.Domain.Enum;
using GeneralUpdate.Core.Utils;
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

namespace GeneralUpdateDemo.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string baseUrl = $"http://localhost:5057";
        private const string hubName = "versionhub";

        public MainWindow()
        {
            InitializeComponent();

            MyButton.Click += MyButton_Click;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var md5 = FileUtil.GetFileMD5(@"C:\Users\Wby\Desktop\123.zip");
            VersionHub<string>.Instance.Subscribe($"{baseUrl}/{hubName}", "TESTNAME", new Action<string>(GetMessage));
        }

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GetMessage(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
                Upgrade();
        }

        private void Upgrade()
        {
            Task.Run(async () =>
            {
                Configinfo configinfo = GetWindowsConfiginfo();
                var generalClientBootstrap = new GeneralClientBootstrap();
                //单个或多个更新包下载通知事件
                generalClientBootstrap.MutiDownloadProgressChanged += OnMutiDownloadProgressChanged;
                //单个或多个更新包下载速度、剩余下载事件、当前下载版本信息通知事件
                generalClientBootstrap.MutiDownloadStatistics += OnMutiDownloadStatistics;
                //单个或多个更新包下载完成
                generalClientBootstrap.MutiDownloadCompleted += OnMutiDownloadCompleted;
                //完成所有的下载任务通知
                generalClientBootstrap.MutiAllDownloadCompleted += OnMutiAllDownloadCompleted;
                //下载过程出现的异常通知
                generalClientBootstrap.MutiDownloadError += OnMutiDownloadError;

            });
        }

        #region GeneralClientBootstrap_Event
        private void OnMutiDownloadProgressChanged(object sender, MutiDownloadProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        } 
        #endregion

        /// <summary>
        /// 获取Windows平台所需的配置参数
        /// </summary>
        /// <returns></returns>
        private Configinfo GetWindowsConfiginfo()
        {
            //该对象用于主程序客户端与更新组件进程之间交互用的对象
            var config = new Configinfo
            {
                //本机的客户端程序应用地址
                InstallPath = @"D:\Updatetest_hub\Run_app",
                UpdateLogUrl = "https://www.baidu.com/",
                //客户端当前版本号
                ClientVersion = "1.1.1.1",
                //客户端类型：1.主程序客户端 2.更新组件
                AppType = AppType.ClientApp,
                //指定应用密钥，用于区分客户端应用
                AppSecretKey = "41A54379-C7D6-4920-8768-21A3468572E5",
                //更新程序exe名称
                AppName = "AutoUpdate.Core",
                //主程序客户端exe名称
                MainAppName = "AutoUpdate.ClientCore"
            };
            //更新组件更新包下载地址
            config.UpdateUrl = $"{baseUrl}/versions/{config.AppType}/{config.ClientVersion}/{config.AppSecretKey}";
            //主程序信息
            var mainVersion = "1.1.1.1";
            //主程序客户端更新包下载地址
            config.MainUpdateUrl = $"{baseUrl}/versions/{AppType.ClientApp}/{mainVersion}/{config.AppSecretKey}";
            return config;
        }
    }
}
