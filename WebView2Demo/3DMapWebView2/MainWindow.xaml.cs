using Microsoft.Web.WebView2.Core;
using Skyversation.UCAS.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _3DMapWebView2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitlizationMap();

            InitilationWebView();

            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        }

        private void InitlizationMap()
        {
            CommonClass.MapOperation.InitializationMapControl(mapHost, "UCAS.3DM");
        }

        private void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping("wbyHost", "assets",
                CoreWebView2HostResourceAccessKind.DenyCors);
                webView.Source = new Uri("https://wbyHost/MapPage.html");

                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

                InjectObjectToScript();

                // 禁用网页右键菜单
                //webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

                // 自定义右键菜单
                webView.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;

                // 基本身份验证
                webView.CoreWebView2.BasicAuthenticationRequested += CoreWebView2_BasicAuthenticationRequested;

                webView.Focus();
            }
        }

        private void CoreWebView2_BasicAuthenticationRequested(object? sender, CoreWebView2BasicAuthenticationRequestedEventArgs e)
        {
            CoreWebView2Deferral deferral = e.GetDeferral();
            System.Threading.SynchronizationContext.Current?.Post((_) => {
                using (deferral)
                {
                    bool userNameAndPasswordSet = false;
                    e.Response.UserName = "admin";
                    e.Response.Password = "xzwyadmin_0731";
                    userNameAndPasswordSet = true;

                    if(e.Response.UserName!="admin" && e.Response.Password != "xzwyadmin_0731")
                    {
                        System.Windows.Forms.MessageBox.Show("用户名或密码不对。", "提示", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                        userNameAndPasswordSet = false;
                    }
                    else
                    {
                        webView.CoreWebView2.Navigate("http://123.60.13.203:18080/#/login");
                    }

                    // 如果我们没有从最终用户那里获得用户名和密码。我们取消认证请求，不提供任何认证
                    if (!userNameAndPasswordSet)
                    {
                        e.Cancel = true;
                    }
                }
            }, null);
        }

        private void CoreWebView2_ContextMenuRequested(object? sender, CoreWebView2ContextMenuRequestedEventArgs e)
        {
            IList<CoreWebView2ContextMenuItem> allMenuList = e.MenuItems;
            //allMenuList.Clear();

            PopulateContextMenu(e, allMenuList);

            //var itemOfSaveAs = allMenuList.FirstOrDefault(x => x.Name == "saveAs");
            //if (itemOfSaveAs != null)
            //{
            //    allMenuList.Remove(itemOfSaveAs);
            //}

            //var itemOfPrint = allMenuList.FirstOrDefault(x => x.Name == "print");
            //if (itemOfPrint != null)
            //{
            //    allMenuList.Remove(itemOfPrint);
            //}

            //var itemOfCreateQRCode = allMenuList.FirstOrDefault(x => x.Label == "为此页面创建 QR 代码");
            //if (itemOfCreateQRCode != null)
            //{
            //    allMenuList.Remove(itemOfCreateQRCode);
            //}

            //var itemOfShare = allMenuList.FirstOrDefault(x => x.CommandId == 50460);
            //if (itemOfShare != null)
            //{
            //    allMenuList.Remove(itemOfShare);
            //}

            //var itemOfSaveInspectElement = allMenuList.FirstOrDefault(x => x.Name == "inspectElement");
            //if (itemOfSaveInspectElement != null)
            //{
            //    allMenuList.Remove(itemOfSaveInspectElement);
            //}

            //var itemOfSeparator = allMenuList.Where(x => x.Kind == CoreWebView2ContextMenuItemKind.Separator).ToList();
            //foreach (var item in itemOfSeparator)
            //{
            //    allMenuList.Remove(item);
            //}

        }

        // 添加自定义上下文菜单
        void PopulateContextMenu(CoreWebView2ContextMenuRequestedEventArgs args, IList<CoreWebView2ContextMenuItem> menuList)
        {
            // 添加一个分割线
            var newItem0 = webView.CoreWebView2.Environment.CreateContextMenuItem("", null, CoreWebView2ContextMenuItemKind.Separator);
            menuList.Insert(menuList.Count, newItem0);

            // 添加一个Command类型的菜单项
            var newItem1 = webView.CoreWebView2.Environment.CreateContextMenuItem("测试菜单1", null, CoreWebView2ContextMenuItemKind.Command);
            newItem1.CustomItemSelected += delegate {
                string pageUri = args.ContextMenuTarget.PageUri;
                System.Threading.SynchronizationContext.Current?.Post((_) =>
                {
                    System.Windows.MessageBox.Show(pageUri, "测试菜单1", MessageBoxButton.OK);
                }, null);
            };
            menuList.Insert(menuList.Count, newItem1);


            // 添加一个CheckBox类型的菜单项
            var newItem2 = webView.CoreWebView2.Environment.CreateContextMenuItem("测试菜单2", null, CoreWebView2ContextMenuItemKind.CheckBox);
            newItem2.CustomItemSelected += delegate
            {
                newItem2.IsChecked = true;
                System.Threading.SynchronizationContext.Current?.Post((_) =>
                {
                    webView.CoreWebView2.Navigate("https://www.baidu.com");
                }, null);
            };

            menuList.Insert(menuList.Count, newItem2);
        }


        private void InjectObjectToScript()
        {
            if (webView != null && webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.AddHostObjectToScript("customWebView2HostObject", new CustomWebView2HostObject());
            }
        }

        private async void InitilationWebView()
        {
            await webView.EnsureCoreWebView2Async();
        }

        private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var msg = e.TryGetWebMessageAsString();
            webView.CoreWebView2.PostWebMessageAsJson($"发送消息：{msg}");
        }

        bool flag;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommonClass.MapOperation.BuildingVisibility(flag);
            flag = !flag;
        }
    }
}
