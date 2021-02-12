using BasicNavigation.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace BasicNavigation
{
    public class BasicNavigationModule : IModule
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