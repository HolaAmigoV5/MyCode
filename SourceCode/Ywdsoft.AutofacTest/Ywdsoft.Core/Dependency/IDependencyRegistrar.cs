using Autofac;
using System;
using System.Collections.Generic;

namespace Ywdsoft.Core
{
    /// <summary>
    /// 依赖注册接口
    /// </summary>
    public interface IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="listType">all type</param>
        void Register(ContainerBuilder builder,List<Type> listType);

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        int Order { get; }
    }
}
