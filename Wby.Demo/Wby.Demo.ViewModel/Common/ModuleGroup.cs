using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Wby.Demo.ViewModel.Common
{
    /// <summary>
    /// 模块分组
    /// </summary>
    public class ModuleGroup : ObservableObject
    {
        /// <summary>
        /// 组名称
        /// </summary>
        private string groupName;
        public string GroupName
        {
            get { return groupName; }
            set { SetProperty(ref groupName, value); }
        }

        /// <summary>
        /// 收缩面板-模板
        /// </summary>
        private bool contractionTemplate;
        public bool ContractionTemplate
        {
            get { return contractionTemplate; }
            set { SetProperty(ref contractionTemplate, value); }
        }

        /// <summary>
        /// 包含子模块
        /// </summary>
        private ObservableCollection<Module> modules;
        public ObservableCollection<Module> Modules
        {
            get { return modules; }
            set { SetProperty(ref modules, value); }
        }
    }
}
