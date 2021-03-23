using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Wby.Demo.ViewModel.Common
{
    /// <summary>
    /// 模块
    /// </summary>
    public class Module : ObservableObject
    {
        
        private string code;

        /// <summary>
        /// 模块图标代码
        /// </summary>
        public string Code
        {
            get { return code; }
            set { SetProperty(ref code, value); }
        }

        
        private string name;

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        
        private int auth;

        /// <summary>
        /// 权限值
        /// </summary>
        public int Auth
        {
            get { return auth; }
            set { SetProperty(ref auth, value); }
        }

       
        private string typeName;

        /// <summary>
        /// 模块命名空间
        /// </summary>
        public string TypeName
        {
            get { return typeName; }
            set { SetProperty(ref typeName, value); }
        }
    }

    /// <summary>
    /// 模块UI组件
    /// </summary>
    public class ModuleUIComponent : Module
    {
        /// <summary>
        /// 页面内容
        /// </summary>
        private object body;
        public object Body
        {
            get { return body; }
            set { SetProperty(ref body, value); }
        }
    }
}
