using System.Collections.Generic;

namespace Wby.PrismDemo.PC.Infrastructure.Common
{
    /// <summary>
    /// 模块分组
    /// </summary>
    public class ModuleGroup
    {
        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 子模块
        /// </summary>
        public List<Module> Modules { get; set; }

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpended { get; set; } = false;
    }
}
