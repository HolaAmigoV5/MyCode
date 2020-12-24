﻿using System.Linq;

namespace My.Repository
{
    /// <summary>
    /// 描述：扩展
    /// 作者：wby 2019/11/15 13:44:09
    /// </summary>
    public static partial class Extention
    {
        /// <summary>
        /// 转为Sharding
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="source">数据源</param>
        /// <returns></returns>
        public static IShardingQueryable<T> ToSharding<T>(this IQueryable<T> source) where T:class, new()
        {
            return new ShardingQueryable<T>(source);
        }

        /// <summary>
        /// 转为Sharding
        /// </summary>
        /// <param name="db">数据源</param>
        /// <returns></returns>
        public static IShardingRepository ToSharding(this IRepository db)
        {
            return new ShardingRepository(db);
        }
    }
}
