using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Wby.Demo.Shared.Attributes;

namespace Wby.PrismDemo.PC.Infrastructure.Common
{
    public class ModuleComponent
    {
        /// <summary>
        /// 获取程序集下的所有具备模块特性的集合
        /// </summary>
        /// <returns>模块特性集合</returns>
        public async Task<List<ModuleAttribute>> GetAssemblyModules()
        {
            try
            {
                List<ModuleAttribute> list = new List<ModuleAttribute>();
                await Task.Run(() =>
                {
                    Assembly asm = Assembly.GetEntryAssembly();
                    var types = asm.GetTypes();
                    foreach (var t in types)
                    {
                        var attr = (ModuleAttribute)t.GetCustomAttribute(typeof(ModuleAttribute), false);
                        if (attr != null)
                            list.Add(attr);
                    }
                });
                return list;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
