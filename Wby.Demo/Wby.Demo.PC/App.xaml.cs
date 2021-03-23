using Autofac;
using System.Configuration;
using System.Windows;
using Wby.Demo.PC.Extensions;
using Wby.Demo.Shared.Common;
using Wby.Demo.Shared.DataInterfaces;
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
            service.AddRepository();
            service.AddViewModel();
            service.AddViewCenter();

            NetCoreProvider.RegisterServiceLocator(service.Build());
        }
    }
}
