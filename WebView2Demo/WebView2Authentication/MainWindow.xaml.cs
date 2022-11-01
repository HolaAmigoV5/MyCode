using Microsoft.Web.WebView2.Core;
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

namespace WebView2Authentication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool _isNavigating = false;
        public MainWindow()
        {
            InitializeComponent();

            //Authentication();

            InitializeAsync();

            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            webView.NavigationStarting += WebView_NavigationStarting;
            webView.NavigationCompleted += WebView_NavigationCompleted;
        }

        // webView2 完成新页面上的内容加载
        private void WebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            _isNavigating = false;
            MessageBox.Show("WebView_NavigationCompleted 事件。" + Environment.NewLine +
                "e.NavigationId = " + e.NavigationId, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void WebView_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            _isNavigating = true;
            string uri = e.Uri;
            if (!uri.StartsWith("https://"))
            {
                MessageBoxResult dr = MessageBox.Show($"{uri} 不安全，请尝试 https 链接。\r\n\r\n确定要访问吗？",
                    "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (dr == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess == false)
            {
                MessageBox.Show($"WebView_CoreWebView2InitializationCompleted 事件，发生异常。" + Environment.NewLine +
                    e.InitializationException.Message, "提示", MessageBoxButton.OK);
            }

            webView.SourceChanged += WebView_SourceChanged;
            webView.ContentLoading += WebView_ContentLoading;
            webView.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
            webView.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;

            webView.CoreWebView2.BasicAuthenticationRequested += CoreWebView2_BasicAuthenticationRequested;
            webView.CoreWebView2.ProcessFailed += CoreWebView2_ProcessFailed;
        }

        private void CoreWebView2_ProcessFailed(object? sender, CoreWebView2ProcessFailedEventArgs e)
        {
            MessageBox.Show("CoreWebView2_ProcessFailed 事件。"
                + Environment.NewLine + "e.ExitCode = " + e.ExitCode
                + Environment.NewLine + "e.FrameInfosForFaileProcess = " + e.FrameInfosForFailedProcess
                + Environment.NewLine + "e.ProcessDescription = " + e.ProcessDescription
                + Environment.NewLine + "e.ProcessFailedKind = " + e.ProcessFailedKind
                + Environment.NewLine + "e.Reason = " + e.Reason,
                "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CoreWebView2_BasicAuthenticationRequested(object? sender, CoreWebView2BasicAuthenticationRequestedEventArgs e)
        {
            MessageBox.Show("CoreWebView2_BasicAuthenticationRequested 事件。"
                + Environment.NewLine + "e.Uri = " + e.Uri
                + Environment.NewLine + "e.Cancel = " + e.Cancel
                + Environment.NewLine + "e.Challenge = " + e.Challenge
                + Environment.NewLine + "e.Response = " + e.Response,
                "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // webView2 完成对DOM内容分析，但未在页面上加载所有图像、脚本和其他内容
        private void CoreWebView2_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            MessageBox.Show("CoreWebView2_DOMContentLoaded 事件。" + Environment.NewLine +
                "e.NavigationId = " + e.NavigationId, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 更新历史记录
        private void CoreWebView2_HistoryChanged(object? sender, object e)
        {
            MessageBox.Show("CoreWebView2_HistoryChanged 事件。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 加载新页面内容
        private void WebView_ContentLoading(object? sender, CoreWebView2ContentLoadingEventArgs e)
        {
            MessageBox.Show("WebView_ContentLoading 事件。" + Environment.NewLine +
                "e.IsErrorPage = " + e.IsErrorPage, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 导航到新的URL
        private void WebView_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
        {
            MessageBox.Show("WebView_SourceChanged 事件。" + Environment.NewLine +
                "e.IsNewDocument = " + e.IsNewDocument, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Authentication()
        {
            if (webView != null && webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.BasicAuthenticationRequested += (sender, e) =>
                {
                    e.Response.UserName = "admin";
                    e.Response.Password = "559a512cf41031934d6383feee161a0f";
                };

                webView.CoreWebView2.Navigate("http://123.60.13.203:18080/#/login");
            }
        }

        async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
        }

        private async void GoToPageCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await webView.EnsureCoreWebView2Async(null);

            var rawUrl = (string)e.Parameter;
            Uri uri;
            if (Uri.IsWellFormedUriString(rawUrl, UriKind.Absolute))
            {
                uri = new Uri(rawUrl);
            }
            else if (!rawUrl.Contains(' ') && rawUrl.Contains('.'))
            {
                uri = new Uri("http://" + rawUrl);
            }
            else
            {
                // Otherwise treat it as a web search.
                uri = new Uri("https://bing.com/search?q=" +
                    string.Join("+", Uri.EscapeDataString(rawUrl).Split(new string[] { "%20" }, StringSplitOptions.RemoveEmptyEntries)));
            }

            webView.CoreWebView2.Navigate(uri.ToString());
        }

        private void GoToPageCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = webView != null && !_isNavigating;
        }
    }
}
