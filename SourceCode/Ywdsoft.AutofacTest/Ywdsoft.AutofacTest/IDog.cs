using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using Ywdsoft.Core;

namespace Ywdsoft.AutofacTest
{
    public interface IDog
    {
        /// <summary>
        /// 品种
        /// </summary>
        string Breed { get; }

        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// 萨摩耶
    /// </summary>
    [Intercept(typeof(LogInterceptor))]
    public class Samoyed : IDog, ITransientDependency
    {
        /// <summary>
        /// 品种
        /// </summary>
        public string Breed
        {
            get
            {
                return "Samoyed（萨摩耶）";
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return "小黄";
            }
        }
    }

    /// <summary>
    /// 藏獒
    /// </summary>
    public class TibetanMastiff : IDog, ITransientDependency
    {
        /// <summary>
        /// 品种
        /// </summary>
        public string Breed
        {
            get
            {
                return "Mastiff Class（獒犬类）";
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return "小黑";
            }
        }
    }
}
