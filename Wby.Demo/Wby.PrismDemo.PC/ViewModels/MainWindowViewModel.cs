using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Wby.Demo.Shared.Common;
using Wby.Demo.Shared.Common.Enums;
using Wby.PrismDemo.PC.Infrastructure.Common;
using Wby.PrismDemo.PC.Infrastructure.Constants;
using Wby.PrismDemo.PC.Views;

namespace Wby.PrismDemo.PC.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(IRegionManager regionManager)
        {
            RegionManager = regionManager;
            Initialize();
        }

        #region Properties
        public IRegionManager RegionManager { get; }

        private ObservableCollection<ModuleGroup> moduleGroups;
        
        /// <summary>
        /// 菜单对象
        /// </summary>
        public ObservableCollection<ModuleGroup> ModuleGroups
        {
            get { return moduleGroups; }
            set { SetProperty(ref moduleGroups, value); }
        }


        private ObservableCollection<Module> moduleList;

        /// <summary>
        /// 所有展开的模块
        /// </summary>
        public ObservableCollection<Module> ModuleList
        {
            get { return moduleList; }
            set { SetProperty(ref moduleList, value); }
        }

        #endregion

        #region Commands

        private DelegateCommand<object> _openPageCommand;
        public DelegateCommand<object> OpenPageCommand => _openPageCommand ??= new DelegateCommand<object>(OpenPage);

        private DelegateCommand<object> _closePageCommand;
        public DelegateCommand<object> ClosePageCommand => _closePageCommand ??= new DelegateCommand<object>(ClosePage);

        #endregion

        #region Excutes
        private void OpenPage(object obj)
        {
            try
            {
                if(obj is Module module)
                {
                    RegionManager?.RequestNavigate(RegionNames.ContentRegion, module.ViewName);
                    SendMsgInfo.SendCurrentModule(module);
                    if (!ModuleList.Contains(module))
                    {
                        ModuleList.Add(module);
                    }
                       
                }
            }
            catch (Exception ex)
            {
                SendMsgInfo.SendMsgToSnackBar(ex.Message);
            }
        }

        private void ClosePage(object obj)
        {
            if(obj is Module module)
            {
                if (ModuleList.Contains(module))
                    ModuleList.Remove(module);


                var nModule = ModuleList.LastOrDefault();
                if (nModule == null)
                {
                    var homeModule = GetHomeModule();
                    ModuleList.Add(homeModule);
                    SendMsgInfo.SendCurrentModule(homeModule);
                    RegionManager?.RequestNavigate(RegionNames.ContentRegion, homeModule.ViewName);
                }
                else
                {
                    RegionManager?.RequestNavigate(RegionNames.ContentRegion, nModule.ViewName);
                    SendMsgInfo.SendCurrentModule(nModule);
                }
                   
            }
        }

        private Module GetHomeModule()
        {
            return new Module() { Code = "Home", Name = "首页", ViewName = "HomeView", Auth = 5 };
        }

        private async void Initialize()
        {
            //初始化ModuleList对象
            moduleList = new ObservableCollection<Module>
            {
                GetHomeModule()
            };

            //初始化ModuleGroups对象
            await LoadAssemblyModule();
        }

        /// <summary>
        /// 加载程序集模块
        /// </summary>
        /// <returns></returns>
        private async Task LoadAssemblyModule()
        {
            try
            {
                moduleGroups = new ObservableCollection<ModuleGroup>();

                var emMt = Enum.GetValues(typeof(ModuleType));
                for (int i = 0; i < emMt.Length; i++)
                    ModuleGroups.Add(new ModuleGroup() { GroupName = emMt.GetValue(i).ToString(), Modules = new List<Module>() });

                //查找程序集下所有带有ModuleAttribute的对象
                var ms = await new ModuleComponent().GetAssemblyModules();
                foreach (var i in ms)
                {
                    //如果当前程序集的模快在服务器上可以匹配到就添加模块列表
                    var m = Contract.Menus.FirstOrDefault(t => t.MenuName.Equals(i.Name));
                    if (m != null)
                    {
                        var group = ModuleGroups.FirstOrDefault(t => t.GroupName == i.ModuleType.ToString());
                        if (group == null)
                        {
                            ModuleGroup newgroup = new ModuleGroup
                            {
                                GroupName = i.ModuleType.ToString(),
                                Modules = new List<Module>()
                            };
                            newgroup.Modules.Add(MapMenuToModule(i.Name, m.MenuCaption, m.MenuNameSpace, m.MenuAuth));
                            ModuleGroups.Add(newgroup);
                        }
                        else
                        {
                            group.Modules.Add(MapMenuToModule(i.Name, m.MenuCaption, m.MenuNameSpace, m.MenuAuth));
                        }
                    }
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                SendMsgInfo.SendMsgToSnackBar(ex.Message);
            }
        }

        private Module MapMenuToModule(string moduleName, string code, string typeName, int auth)
        {
            return new Module()
            {
                Name = moduleName,
                Code = code,
                ViewName = typeName,
                Auth = auth
            };
        }
        #endregion
    }
}
