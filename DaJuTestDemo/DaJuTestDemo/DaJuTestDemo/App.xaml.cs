using DaJuTestDemo.Modules.ModuleName;
using DaJuTestDemo.Services;
using DaJuTestDemo.Services.Interfaces;
using DaJuTestDemo.Views;
using I3DMapOperation;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace DaJuTestDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMapOperation, MapOperation>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //moduleCatalog.AddModule<ModuleNameModule>();
        }
    }
}
