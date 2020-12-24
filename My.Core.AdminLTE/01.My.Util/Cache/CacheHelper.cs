using System;

namespace My.Util
{
    /// <summary>
    /// 描述：缓存帮助类
    /// 作者：wby 2019/10/12 16:42:16
    /// </summary>
    public class CacheHelper
    {
        public static ICache Cache { get; }
        public static ICache SystemCache { get; set; }
        public static ICache RedisCache { get; set; }

        /// <summary>
        /// 静态构造函数，初始化缓存类型
        /// </summary>
        static CacheHelper()
        {
            SystemCache = new SystemCache();
            if (!GlobalSwitch.RedisConfig.IsNullOrEmpty())
            {
                try
                {
                    RedisCache = new RedisCache(GlobalSwitch.RedisConfig);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            switch (GlobalSwitch.CacheType)
            {
                case CacheType.SystemCache:
                    Cache = SystemCache;
                    break;
                case CacheType.RedisCache:
                    Cache = RedisCache;
                    break;
                default:
                    throw new Exception("请指定缓存类型！");
            }
        }
    }
}
