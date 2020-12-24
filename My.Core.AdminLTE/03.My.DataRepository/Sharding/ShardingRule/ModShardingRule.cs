using My.Util;

namespace My.Repository
{
    /// <summary>
    /// 描述：取模分片规则
    /// 说明:根据某字段的HASH,然后取模后得到表名后缀
    /// 举例:Base_User_0,Base_User为抽象表名,_0为后缀
    /// 警告:使用简单,但是扩容后需要大量数据迁移,不推荐使用
    /// 作者：wby 2019/11/15 16:37:59
    /// </summary>
    public class ModShardingRule : IShardingRule
    {
        protected string _absTableName { get; }
        protected string _keyField { get; }
        protected int _mod { get; }

        public ModShardingRule(string absTableName,string keyField,int mod)
        {
            _absTableName = absTableName;
            _keyField = keyField;
            _mod = mod;
        }

        public string FindTable(object obj)
        {
            string key = obj.GetPropertyValue(_keyField).ToString();

            return $"{_absTableName}_{(uint)(key.ToMurmurHash() % _mod)}";
        }
    }
}
