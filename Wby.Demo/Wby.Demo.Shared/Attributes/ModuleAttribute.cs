using System;
using Wby.Demo.Shared.Common.Enums;

namespace Wby.Demo.Shared.Attributes
{
    /// <summary>
    /// 模块特性, 标记该特性表示属于应用模块的部分
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// 模块组名称
        /// </summary>
        public ModuleType ModuleType { get; }

        public ModuleAttribute(string name, ModuleType moduleType)
        {
            Name = name;
            ModuleType = moduleType;
        }
    }
}
