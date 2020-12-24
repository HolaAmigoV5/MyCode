﻿using My.Util;
using System.Collections.Generic;

namespace My.Repository
{
    /// <summary>
    /// 描述：一致性HASH分片规则
    /// 作者：wby 2019/11/15 16:28:45
    /// 优点:数据扩容时数据迁移量较小,表越多扩容效果越明显
    /// 缺点:扩容时需要进行数据迁移,比较复杂
    /// 建议:若雪花分片不满足则采用本方案,此方案为分片规则中的"核弹"
    /// </summary>
    ///<seealso cref="My.Repository.IShardingRule" />
    public class ConsistentHashShardingRule : IShardingRule
    {
        protected List<string> _tables { get; }
        protected ConsistentHash<string> _consistentHash { get; } = new ConsistentHash<string>();

        public ConsistentHashShardingRule(List<string> tables)
        {
            _tables = tables;
            _consistentHash.Init(tables);
        }
        public string FindTable(object obj)
        {
            string key = obj.GetPropertyValue("Id")?.ToString();
            return _consistentHash.GetNode(key);
        }
    }
}
