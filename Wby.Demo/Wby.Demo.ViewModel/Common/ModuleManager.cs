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
        private ObservableCollection<Module> modules;

        /// <summary>
        /// 已加载模块
        /// </summary>
        public ObservableCollection<Module> Modules
        {
            get { return modules; }
            set { SetProperty(ref modules, value); }
        }

        private ObservableCollection<ModuleGroup> moduleGroups;

        /// <summary>
        /// 已加载模块-分组
        /// </summary>
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
                                Modules = new ObservableCollection<Module>()
                            };
                            newgroup.Modules.Add(MapMenuToModule(i.Name, m.MenuCaption, m.MenuNameSpace, m.MenuAuth));
                            ModuleGroups.Add(newgroup);
                        }
                        else
                        {
                            group.Modules.Add(MapMenuToModule(i.Name, m.MenuCaption, m.MenuNameSpace, m.MenuAuth));
                        }
                        Modules.Add(MapMenuToModule(i.Name, m.MenuCaption, m.MenuNameSpace, m.MenuAuth));
                    }
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Module MapMenuToModule(string moduleName, string code, string typeName, int auth)
        {
            return new Module()
            {
                Name = moduleName,
                Code = code,
                TypeName = typeName,
                Auth = auth
            };
        }
    }
}
