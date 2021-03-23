using Prism.Ioc;
using Prism.Unity;
using System.Configuration;
using System.Windows;
using Wby.Demo.Shared.Common;
using Wby.PrismDemo.PC.Infrastructure.Extensions;
using Wby.PrismDemo.PC.ViewModels;
using Wby.PrismDemo.PC.Views;
using Wby.PrismDemo.PC.Views.Dialogs;

namespace Wby.PrismDemo.PC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Contract.ServerUrl = ConfigurationManager.AppSettings["serverAddress"];
            base.OnStartup(e);
        }
        protected override Window CreateShell()
        {
            return Container.Resolve<LoginView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var container = PrismIocExtensions.GetContainer(containerRegistry);

            //注册所有服务
            container.RegisterServers();

            //注册导航
            containerRegistry.RegisterForNavigation<HomeView>();
            containerRegistry.RegisterForNavigation<SkinView>();
            containerRegistry.RegisterForNavigation<UserView>();
            containerRegistry.RegisterForNavigation<MenuView>();
            containerRegistry.RegisterForNavigation<GroupView>();

            //注册对话
            //这里依赖ViewModelLocator找到对应的ViewModel。
            //MsgView是UserControl，它不能直接控制拥有它的Window，只能通过在MsgView中添加附加属性定义Window的样式，
            //DialogService自己创建一个Window将View放进去。
            containerRegistry.RegisterDialog<MsgView, MsgViewModel>(); //可以指定ViewModel注册
            //containerRegistry.RegisterDialogWindow<DialogWindow>();  //这里注册自定义Window来承载MsgView。也可以不用


            NetCoreProvider.RegisterUnityContainer(container);
        }
    }
}
