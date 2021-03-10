using My.Util;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace My.Repository
{
    /// <summary>
    /// 描述：SqlServer的实现
    /// 作者：wby 2019/11/15 13:16:12
    /// </summary>
    public class SqlServerRepository : DbRepository
    {
        public SqlServerRepository() : base(null, DatabaseType.SqlServer) { }
        public SqlServerRepository(string conStr) : base(conStr, DatabaseType.SqlServer) { }

        protected override string FormatFieldName(string name)
        {
            return $"[{name}]";
        }

        public override void BulkInsert<T>(List<T> entities)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                string tableName = string.Empty;
                var tableAttribute = typeof(T).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
                if (tableAttribute != null)
                    tableName = ((TableAttribute)tableAttribute).Name;
                else
                    tableName = typeof(T).Name;

                SqlBulkCopy sqlBC = new SqlBulkCopy(conn)
                {
                    BatchSize = 100000,
                    BulkCopyTimeout = 0,
                    DestinationTableName = tableName
                };

                using (sqlBC)
                {
                    sqlBC.WriteToServer(entities.ToDataTable());
                }
            }
        }
    }
}
