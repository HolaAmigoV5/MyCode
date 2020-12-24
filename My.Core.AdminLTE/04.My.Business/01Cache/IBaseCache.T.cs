using System.Collections.Generic;

namespace My.Business
{
    /// <summary>
    /// 描述：缓存接口
    /// 作者：wby 2019/11/18 15:19:36
    /// </summary>
    public interface IBaseCache<T> where T : class
    {
        T GetCache(string idKey);
        void UpdateCache(string idKey);
        void UpdateCache(List<string> idKeys);
    }
}
