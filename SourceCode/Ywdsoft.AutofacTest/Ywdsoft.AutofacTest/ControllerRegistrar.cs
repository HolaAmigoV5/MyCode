using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Ywdsoft.Core;

namespace Ywdsoft.AutofacTest
{
    /// <summary>
    /// 
    /// </summary>
    public class ControllerRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// 
        /// </summary>
        public int Order
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="listType"></param>
        public void Register(ContainerBuilder builder, List<Type> listType)
        {
            builder.RegisterType(typeof(LogInterceptor));
            //注册Controller,实现属性注入
            var IControllerType = typeof(ControllerBase);
            var arrControllerType = listType.Where(t => IControllerType.IsAssignableFrom(t) && t != IControllerType).ToArray();
            builder.RegisterTypes(arrControllerType).PropertiesAutowired().EnableClassInterceptors();
        }
    }
}
