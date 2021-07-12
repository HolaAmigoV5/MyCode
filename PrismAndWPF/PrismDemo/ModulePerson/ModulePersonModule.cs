using ModulePerson.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModulePerson
{
    public class ModulePersonModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("PersonListRegion", "PersonList");

            //regionManager.RegisterViewWithRegion("PersonListRegion", typeof(PersonList));
            //regionManager.RegisterViewWithRegion("PersonDetailsRegion", typeof(PersonDetail));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<PersonList>();
            containerRegistry.RegisterForNavigation<PersonDetail>();
        }
    }
}