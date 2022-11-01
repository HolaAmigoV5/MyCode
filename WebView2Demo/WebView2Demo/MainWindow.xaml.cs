using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
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

namespace WebView2Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string browserExecutableFolder = "D:\\GitHub\\Microsoft.WebView2";
        public MainWindow()
        {
            InitializeComponent();

            // 方法一：使用指定WebView2 Runtime运行
            //webView.CreationProperties = new CoreWebView2CreationProperties()
            //{
            //    BrowserExecutableFolder = browserExecutableFolder
            //};

            InitializeAsync();
            webView.NavigationStarting += EnsureHttps;
        }

        private void EnsureHttps(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            string uri = e.Uri;
            if (!uri.StartsWith("https://"))
            {
                // 执行注入Js脚本
                webView.CoreWebView2.ExecuteScriptAsync($"alert('{uri} is not safe, try an https link')");
                e.Cancel = true;
            }
        }

        async void InitializeAsync()
        {
            //方法二：使用WebView2控件的固定版本运行时环境
            //var env = await CoreWebView2Environment.CreateAsync(browserExecutableFolder);
            //await webView.EnsureCoreWebView2Async(env);

            await webView.EnsureCoreWebView2Async(null);

            // 注册要响应WebMessageReceived的事件处理程序
            webView.CoreWebView2.WebMessageReceived += UpdateAddressBar;

            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.addEventListener(\'message\',event=>alert(event.data));");
        }

        private void UpdateAddressBar(object? sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            string uri = args.TryGetWebMessageAsString();
            addressBar.Text = uri;
            webView.CoreWebView2.PostWebMessageAsString($"发送消息：{uri}");
        }

        private async Task<ValueTuple<bool, string>> CheckExistWebView2Runtime()
        {
            string msg = "检测到系统中未安装WebView2运行时。";
            bool isExist = false;

            try
            {
                await webView.EnsureCoreWebView2Async(null);
                string browserVersion = webView.CoreWebView2.Environment.BrowserVersionString;  // 获取浏览器版本
                if (!string.IsNullOrWhiteSpace(browserVersion))
                {
                    msg = "已安装，版本号：" + browserVersion;
                    isExist = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CheckExistWebView2Runtime()方法，逻辑发生异常{ex.Message}");
            }
            return ValueTuple.Create(isExist, msg);
        }

        /// <summary>
        /// 导航到新的URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGo_Click(object sender, RoutedEventArgs e)
        {
            if (webView != null && webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.Navigate("https://cn.bing.com/images/trending?form=Z9LH");
            }

            e.Handled = true;
        }

        /// <summary>
        /// 检测运行时环境
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            var checkResult = CheckExistWebView2Runtime().Result;
            if (checkResult.Item1 == false)
            {
                MessageBox.Show(checkResult.Item2);
            }
            else
            {
                var browserVersion = webView.CoreWebView2.Environment.BrowserVersionString;
                MessageBox.Show($"当前安装的BrowserVersion版本是：{browserVersion}");
                //webView.CoreWebView2.Navigate(addressBar.Text.Trim());
            }
            e.Handled = true;
        }

        private void ButtonGet_Click(object sender, RoutedEventArgs e)
        {
            var str= CoreWebView2Environment.GetAvailableBrowserVersionString();    //检测浏览器版本
            if (str != null)
            {
                MessageBox.Show(str);
            }
        }
    }
}
