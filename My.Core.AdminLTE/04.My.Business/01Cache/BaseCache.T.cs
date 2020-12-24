using My.Util;
using System.Collections.Generic;

namespace My.Business
{
    /// <summary>
    /// 描述：缓存实现
    /// 作者：wby 2019/11/18 15:22:36
    /// </summary>
    public abstract class BaseCache<T> : IBaseCache<T> where T : class
    {
        protected abstract string _moduleKey { get; }
        protected abstract T GetDbData(string key);

        public string BuildKey(string idKey)
        {
            return $"{GlobalSwitch.ProjectName}_Cache_{_moduleKey}_{idKey}";
        }

        public T GetCache(string idKey)
        {
            if (idKey.IsNullOrEmpty())
                return null;

            string cacheKey = BuildKey(idKey);
            var cache = CacheHelper.Cache.GetCache<T>(cacheKey);
            if (cache == null)
            {
                cache = GetDbData(idKey);
                if (cache != null)
                    CacheHelper.Cache.SetCache(cacheKey, cache);
            }

            return cache;
        }

        public void UpdateCache(string idKey)
        {
            CacheHelper.Cache.RemoveCache(BuildKey(idKey));
        }

        public void UpdateCache(List<string> idKeys)
        {
            idKeys.ForEach(x => UpdateCache(x));
        }
    }
}
