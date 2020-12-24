using System;
using System.Linq.Expressions;

namespace My.Util
{
    /// <summary>
    /// 描述：Linq操作帮助类
    /// 作者：wby 2019/9/26 16:27:36
    /// </summary>
    public static class LinqHelper
    {
        /// <summary>
        /// 创建初始条件为True的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>()
        {
            return x => true;
        }

        /// <summary>
        /// 创建初始条件为False的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>()
        {
            return x => false;
        }
    }
}
