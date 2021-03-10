using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Wby.Demo.Shared.Common;
using Wby.Demo.Shared.Common.Enums;

namespace Wby.Demo.ViewModel.Common
{
    public class ModuleManager : ObservableObject
    {
        /// <summary>
        /// 已加载模块
        /// </summary>
        private ObservableCollection<Module> modules;
        public ObservableCollection<Module> Modules
        {
            get { return modules; }
            set { SetProperty(ref modules, value); }
        }

        /// <summary>
        /// 已加载模块-分组
        /// </summary>
        private ObservableCollection<ModuleGroup> moduleGroups;
        public ObservableCollection<ModuleGroup> ModuleGroups
        {
            get { return moduleGroups; }
            set { SetProperty(ref moduleGroups, value); }
        }


        /// <summary>
        /// 加载程序集模块
        /// </summary>
        /// <returns></returns>
        public async Task LoadAssemblyModule()
        {
            try
            {
                Modules = new ObservableCollection<Module>();
                ModuleGroups = new ObservableCollection<ModuleGroup>();
                var emMt = Enum.GetValues(typeof(ModuleType));
                for (int i = 0; i < emMt.Length; i++)
                    ModuleGroups.Add(new ModuleGroup() { GroupName = emMt.GetValue(i).ToString(), Modules = new ObservableCollection<Module>() });
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
                                Modules = new ObservableCollection<Module>()
                            };
                            newgroup.Modules.Add(new Module()
                            {
                                Name = i.Name,
                                Code = m.MenuCaption,
                                TypeName = m.MenuNameSpace,
                                Auth = m.MenuAuth
                            });
                            ModuleGroups.Add(newgroup);
                        }
                        else
                        {
                            group.Modules.Add(new Module()
                            {
                                Name = i.Name,
                                Code = m.MenuCaption,
                                TypeName = m.MenuNameSpace,
                                Auth = m.MenuAuth
                            });
                        }
                        Modules.Add(new Module()
                        {
                            Name = i.Name,
                            Code = m.MenuCaption,
                            TypeName = m.MenuNameSpace,
                            Auth = m.MenuAuth
                        });
                    }
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                throw ex;
            }
        }
    }
}
