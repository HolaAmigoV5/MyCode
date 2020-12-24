using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace My.Util
{
    /// <summary>
    /// 描述：DbContext扩展
    /// 作者：wby 2019/10/11 15:01:26
    /// </summary>
    public static partial class Extension
    {
        /// <summary>
        /// 获取IQueryable
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="entityType">实体类型</param>
        /// <returns></returns>
        public static IQueryable GetQueryable(this DbContext context,Type entityType)
        {
            var dbSet = context.GetType().GetMethod("Set").MakeGenericMethod(entityType).Invoke(context, null);
            var resQ = typeof(EntityFrameworkQueryableExtensions).GetMethod("AsNoTracking").MakeGenericMethod(entityType).Invoke(null, new object[] { dbSet });

            return resQ as IQueryable;
        }
    }
}
