using My.Util;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace My.Repository
{
    /// <summary>
    /// 描述：MySql的实现
    /// 作者：wby 2019/11/7 15:39:11
    /// </summary>
    public class MySqlRepository : DbRepository
    {
        #region 构造函数
        public MySqlRepository() : base(null, DatabaseType.MySql) { }

        public MySqlRepository(string conStr) : base(conStr, DatabaseType.MySql) { }
        #endregion

        protected override string FormatFieldName(string name)
        {
            return $"'{name}'";
        }

        /// <summary>
        /// 使用Bulk批量插入数据（适合大数据量，速度非常快）
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">数据</param>
        public override void BulkInsert<T>(List<T> entities)
        {
            DataTable dt = entities.ToDataTable();
            using (MySqlConnection conn = new MySqlConnection())
            {
                conn.ConnectionString = ConnectionString;
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                string tableName = string.Empty;
                var tableAttribute = typeof(T).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
                if (tableAttribute != null)
                    tableName = ((TableAttribute)tableAttribute).Name;
                else
                    tableName = typeof(T).Name;

                int insertCount = 0;
                string tmpPath = Path.Combine(Path.GetTempPath(),
                    DateTime.Now.ToCstTime().Ticks.ToString() + "_" + Guid.NewGuid().ToString() + ".tmp");
                string csv = dt.ToCsvStr();
                File.WriteAllText(tmpPath, csv, Encoding.UTF8);

                using (MySqlTransaction tran = conn.BeginTransaction())
                {
                    MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
                    {
                        FieldTerminator = ",",
                        FieldQuotationCharacter = '"',
                        EscapeCharacter = '"',
                        LineTerminator = "\r\n",
                        FileName = tmpPath,
                        NumberOfLinesToSkip = 0,
                        TableName = tableName
                    };

                    try
                    {
                        bulk.Columns.AddRange(dt.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToList());
                        insertCount = bulk.Load();
                        tran.Commit();
                    }
                    catch (MySqlException ex)
                    {
                        if (tran != null)
                            tran.Rollback();
                        throw ex;
                    }
                }

                File.Delete(tmpPath);
            }
        }

        /// <summary>
        /// 删除所有数据
        /// </summary>
        /// <typeparam name="T">数据泛型</typeparam>
        public override void DeleteAll<T>()
        {
            Delete(GetList<T>());
        }
    }
}
