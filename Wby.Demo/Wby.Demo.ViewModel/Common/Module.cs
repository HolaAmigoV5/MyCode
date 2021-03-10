using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Wby.Demo.ViewModel.Common
{
    /// <summary>
    /// 模块
    /// </summary>
    public class Module : ObservableObject
    {
        /// <summary>
        /// 模块图标代码
        /// </summary>
        private string code;
        public string Code
        {
            get { return code; }
            set { SetProperty(ref code, value); }
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        /// <summary>
        /// 权限值
        /// </summary>
        private int auth;
        public int Auth
        {
            get { return auth; }
            set { SetProperty(ref auth, value); }
        }

        /// <summary>
        /// 模块命名空间
        /// </summary>
        private string typeName;
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
