﻿using ModuleC.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModuleC
{
    public class ModuleCModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("CRegion", typeof(ViewC));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}