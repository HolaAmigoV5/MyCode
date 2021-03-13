using Autofac;
using System.Configuration;
using System.Windows;
using Wby.Demo.PC.Common;
using Wby.Demo.PC.Extensions;
using Wby.Demo.PC.ViewCenter;
using Wby.Demo.Service;
using Wby.Demo.Shared.Common;
using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.ViewModel;
using Wby.Demo.ViewModel.Core;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
            NetCoreProvider.Resolve<ILog>()?.Warn(e.Exception, e.Exception.Message);
            e.Handled = true;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            Contract.ServerUrl = ConfigurationManager.AppSettings["serverAddress"];
            ConfigureServices();
            var login = NetCoreProvider.ResolveNamed<ILoginCenter>("LoginCenter");
            await login.ShowDialog();
        }

        private void ConfigureServices()
        {
            var service = new ContainerBuilder();
            service.AddRepository<UserService, IUserRepository>()
                .AddRepository<GroupService, IGroupRepository>()
                .AddRepository<MenuService, IMenuRepository>()
                .AddRepository<BasicService, IBasicRepository>()
                .AddRepository<WbyNLog, ILog>();


            service.AddViewModel<UserViewModel, IUserViewModel>()
                .AddViewModel<LoginViewModel, ILoginViewModel>()
                .AddViewModel<MainViewModel, IMainViewModel>()
                .AddViewModel<GroupViewModel, IGroupViewModel>()
                .AddViewModel<MenuViewModel, IMenuViewModel>()
                .AddViewModel<BasicViewModel, IBasicViewModel>()
                .AddViewModel<SkinViewModel, ISkinViewModel>()
                .AddViewModel<HomeViewModel, IHomeViewModel>()
                .AddViewModel<DashboardViewModel, IDashboardViewModel>();


            service.AddViewCenter<LoginCenter, ILoginCenter>()
                .AddViewCenter<MainCenter, IMainCenter>()
                .AddViewCenter<MsgCenter, IMsgCenter>()
                .AddViewCenter<HomeCenter, IHomeCenter>()

                .AddViewCenter<UserCenter, IBaseCenter>()
                .AddViewCenter<MenuCenter, IBaseCenter>()
                .AddViewCenter<SkinCenter, IBaseCenter>()
                .AddViewCenter<GroupCenter, IBaseCenter>()
                .AddViewCenter<BasicCenter, IBaseCenter>()
                .AddViewCenter<DashboardCenter, IBaseCenter>();

            NetCoreProvider.RegisterServiceLocator(service.Build());
        }
    }
}
