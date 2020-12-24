using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text;

namespace My.Util
{
    /// <summary>
    /// 描述：数据库操作抽象帮助类
    /// 作者：wby 2019/9/19 16:53:47
    /// </summary>
    public abstract class DbHelper
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="conStr">连接名或连接字符串</param>
        public DbHelper(DatabaseType dbType, string conStr)
        {
            _dbType = dbType;
            _conStr = conStr;
        }
        #endregion

        #region 私有成员

        /// <summary>
        /// 数据库类型
        /// </summary>
        protected DatabaseType _dbType;

        /// <summary>
        /// 连接字符串
        /// </summary>
        protected string _conStr;

        /// <summary>
        /// 实体需要引用的额外命名空间
        /// </summary>
        protected string _extraUsingNameSpace { get; set; } = string.Empty; 

        /// <summary>
        /// 类型映射字典
        /// </summary>
        protected abstract Dictionary<string,Type> DbTypeDic { get; }
        #endregion

        #region 通用方法
        /// <summary>
        /// Sql查询返回DataTable，参数化查询
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <param name="parameters">参数(可选)</param>
        /// <returns></returns>
        public DataTable GetDataTableWithSql(string sql, List<DbParameter> parameters = null)
        {
            DbProviderFactory dbProviderFactory = DbProviderFactoryHelper.GetDbProviderFactory(_dbType);
            using (DbConnection conn = dbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _conStr;
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    if (parameters != null && parameters.Count > 0)
                        parameters.ForEach(item => cmd.Parameters.Add(item));

                    DbDataAdapter adapter = dbProviderFactory.CreateDataAdapter();
                    adapter.SelectCommand = cmd;
                    DataSet table = new DataSet();
                    adapter.Fill(table);
                    cmd.Parameters.Clear();
                    return table.Tables[0];
                }
            }
        }

        /// <summary>
        /// 通过数据库连接字符串和Sql语句查询返回List
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="sqlStr">Sql语句</param>
        /// <returns></returns>
        public List<T> GetListBySql<T>(string sqlStr)
        {
            return GetDataTableWithSql(sqlStr).ToList<T>();
        }

        /// <summary>
        /// 通过数据库连接字符串和Sql语句查询返回List,参数化查询
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="sqlStr">Sql语句</param>
        /// <param name="param">查询参数</param>
        public List<T> GetListBySql<T>(string sqlStr,List<DbParameter> parammeters)
        {
            return GetDataTableWithSql(sqlStr, parammeters).ToList<T>();
        }

        /// <summary>
        /// 执行无返回值的Sql语句
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <param name="parameters">查询参数（可选）</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteSql(string sql, List<DbParameter> parameters = null)
        {
            int count = 0;
            DbProviderFactory dbProviderFactory = DbProviderFactoryHelper.GetDbProviderFactory(_dbType);
            using (DbConnection conn = dbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _conStr;
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    if (parameters != null && parameters.Count > 0)
                        parameters.ForEach(param => cmd.Parameters.Add(param));
                    count = cmd.ExecuteNonQuery();
                }
            }
            return count;
        }

        /// <summary>
        /// 获取数据库中所有表
        /// </summary>
        /// <param name="schemaName">模式（架构）</param>
        /// <returns></returns>
        public abstract List<DbTableInfo> GetDbAllTables(string schemaName = null);

        /// <summary>
        /// 通过表名获取表的相关信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public abstract List<TableInfo> GetDbTableInfo(string tableName);

        /// <summary>
        /// 数据库类型转换为C#类型
        /// </summary>
        /// <param name="dbTypeStr">数据库类型</param>
        /// <returns>返回C#类型</returns>
        public virtual Type DbTypeStr_To_CsharpType(string dbTypeStr)
        {
            string _dbTypeStr = dbTypeStr.ToLower();
            Type type;
            if (DbTypeDic.ContainsKey(_dbTypeStr))
                type = DbTypeDic[_dbTypeStr];
            else
                type = typeof(string);

            return type;
        }

        /// <summary>
        /// 生成实体文件
        /// </summary>
        /// <param name="infos">表字段信息</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableDescription">表描述信息</param>
        /// <param name="filePath">文件路径（包含文件名）</param>
        /// <param name="nameSpace">实体命名空间</param>
        /// <param name="schemaName">架构（模式）名</param>
        public virtual void SaveEntityToFile(List<TableInfo> infos, string tableName,
            string tableDescription, string filePath, string nameSpace, string schemaName = null)
        {
            StringBuilder sb = new StringBuilder();
            string schema = "";
            if (!schemaName.IsNullOrEmpty())
                schema = $@", Schema=""{schemaName}""";
            infos.ForEach((item) =>
            {
                Type type = DbTypeStr_To_CsharpType(item.ColumnType);
                sb.AppendLine(GenerateEntityProperty(item, type));
            });

            var content = ReadTemplate("ModelTemplate.txt");
            content = content.Replace("{GeneratorTime}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                .Replace("{ModelsNamespace}", nameSpace)
                .Replace("{Author}", "wby")
                .Replace("{Comment}", tableDescription)
                .Replace("{ModelName}", tableName)
                .Replace("{ModelProperties}", sb.ToString());

            FileHelper.WriteTxt(content, filePath, Encoding.UTF8, FileMode.Create);
        }

        /// <summary>
        /// 生成属性
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="column">列</param>
        /// <returns></returns>
        private static string GenerateEntityProperty(TableInfo tableInfo,Type columnType)
        {
            var sb = new StringBuilder();
            if (tableInfo != null && columnType != null)
            {
                sb.AppendLine("\t\t/// <summary>");
                sb.AppendLine("\t\t/// " + tableInfo.ColumnDescription);
                sb.AppendLine("\t\t/// </summary>");

                if(tableInfo.IsKey)
                {
                    sb.AppendLine("\t\t[Key]");
                    sb.AppendLine($"\t\tpublic {columnType} Id " + "{get;set;}");
                }
                else
                {
                    if(tableInfo.IsNullable)
                        sb.AppendLine($"\t\tpublic {columnType.Name}? {tableInfo.ColumnName} " + "{get;set;}");
                    else
                    {
                        sb.AppendLine("\t\t[Required]");
                        sb.AppendLine($"\t\tpublic {columnType.Name} {tableInfo.ColumnName} " + "{get;set;}");
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 读取模板
        /// </summary>
        /// <param name="templateName">模板名称</param>
        /// <returns></returns>
        private string ReadTemplate(string templateName)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var content = string.Empty;
            using (var stream = currentAssembly.GetManifestResourceStream
                ($"{currentAssembly.GetName().Name}.CodeTemplate.{templateName}"))
            {
                if (stream != null)
                    using (var reader = new StreamReader(stream))
                        content = reader.ReadToEnd();
            }
            return content;
        }
        #endregion
    }
}
