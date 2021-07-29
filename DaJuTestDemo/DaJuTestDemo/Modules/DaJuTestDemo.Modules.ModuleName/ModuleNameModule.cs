using DaJuTestDemo.Core;
using DaJuTestDemo.Modules.ModuleName.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DaJuTestDemo.Modules.ModuleName
{
    public class ModuleNameModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ModuleNameModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.MainPageContentRegion, "ViewA");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA>();
        }
    }
}