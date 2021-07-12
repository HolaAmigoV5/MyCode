using BasicNavigation;
using BootstrapperShell.Views;
using ModuleA;
using ModuleC;
using ModuleD;
using ModulePerson;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using PrismAndWPF.Core;
using System.Windows;
using System.Windows.Controls;

namespace BootstrapperShell
{
    public class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            regionAdapterMappings.RegisterMapping(typeof(StackPanel), Container.Resolve<StackPanelRegionAdapter>());
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            //配置文件形式加载ModuleA
            //return new ConfigurationModuleCatalog();

            //加载文件夹Modules中的ViewB
            return new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //代码方式加载ModuleA
            moduleCatalog.AddModule<ModuleAModule>();
            //moduleCatalog.AddModule<ModuleCModule>();

            //添加ModuleCModule，手动显示ViewC
            var moduleCType = typeof(ModuleCModule);
            moduleCatalog.AddModule(new ModuleInfo()
            {
                ModuleName = moduleCType.Name,
                ModuleType = moduleCType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.OnDemand
            });

            //添加ModuleDModule
            moduleCatalog.AddModule<ModuleDModule>();

            //添加ModulePersonModule
            moduleCatalog.AddModule<ModulePersonModule>();

            //添加ModuleBasicRegionNavigationModule
            moduleCatalog.AddModule<BasicNavigationModule>();
        }


        //protected override void ConfigureViewModelLocator()
        //{
        //    base.ConfigureViewModelLocator();


        //    //不使用Prism自动View和ViewModel绑定的方式关联，使用当前方式关联
        //    //ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
        //    //{
        //    //    var viewName = viewType.FullName;
        //    //    var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
        //    //    var viewModelName = $"{viewName}ViewModel, {viewAssemblyName}";
        //    //    return Type.GetType(viewModelName);
        //    //});


        //    //4种方式，使用自己自定义的CustomViewModel与MainWindow关联
        //    //type /type
        //    //ViewModelLocationProvider.Register(typeof(MainWindow).ToString(), typeof(CustomViewModel));

        //    //type /factory
        //    //ViewModelLocationProvider.Register(typeof(MainWindow).ToString(), () => Container.Resolve<CustomViewModel>());

        //    //generic factory
        //    //ViewModelLocationProvider.Register<MainWindow>(() => Container.Resolve<CustomViewModel>());

        //    //generic type
        //    ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();

        //}
    }
}
