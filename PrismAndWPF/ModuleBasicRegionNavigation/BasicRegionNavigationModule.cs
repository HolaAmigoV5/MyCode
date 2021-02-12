using BasicRegionNavigation.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace BasicRegionNavigation
{
    public class BasicRegionNavigationModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA>();
            containerRegistry.RegisterForNavigation<ViewB>();
        }
    }
}