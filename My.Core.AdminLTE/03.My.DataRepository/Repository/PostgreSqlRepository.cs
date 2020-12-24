using System;
using System.Collections.Generic;

namespace My.Repository
{
    /// <summary>
    /// 描述：PostgreSql的实现
    /// 作者：wby 2019/11/15 10:44:12
    /// </summary>
    public class PostgreSqlRepository : DbRepository
    {
        public PostgreSqlRepository() : base(null, Util.DatabaseType.PostgreSql) { }
        public PostgreSqlRepository(string conStr) : base(conStr, Util.DatabaseType.PostgreSql) { }

        protected override string FormatFieldName(string name)
        {
            return $"\"{name}\"";
        }

        public override void BulkInsert<T>(List<T> entities)
        {
            throw new Exception("抱歉！暂不支持PostgreSql！");
        }
    }
}
