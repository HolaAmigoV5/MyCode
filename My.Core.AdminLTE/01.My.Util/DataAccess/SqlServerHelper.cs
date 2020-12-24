using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace My.Util
{
    /// <summary>
    /// 描述：SqlServer数据库操作帮助类
    /// 作者：wby 2019/9/19 16:52:19
    /// </summary>
    public class SqlServerHelper : DbHelper
    {
        #region 构造函数
        public SqlServerHelper(string nameOrConStr) : base(DatabaseType.SqlServer, nameOrConStr) { }
        #endregion

        #region 私有成员
        protected override Dictionary<string, Type> DbTypeDic { get; } = new Dictionary<string, Type>()
        {
            {"int",typeof(int)},
            {"text",typeof(string)},
            {"bigint",typeof(long)},
            {"binary",typeof(byte[])},
            {"bit",typeof(bool)},
            {"char",typeof(string)},
            {"date",typeof(DateTime)},
            {"datetime",typeof(DateTime)},
            {"datetime2",typeof(DateTime)},
            {"decimal",typeof(decimal)},
            {"float",typeof(double)},
            {"image",typeof(byte[])},
            {"money",typeof(decimal)},
            {"nchar",typeof(string)},
            {"ntext",typeof(string)},
            {"numeric",typeof(decimal)},
            {"nvarchar",typeof(string)},
            {"real",typeof(float)},
            {"smalldatetime",typeof(DateTime)},
            {"smallint",typeof(short)},
            {"smallmoney",typeof(decimal)},
            {"timestamp",typeof(DateTime)},
            {"tinyint",typeof(byte)},
            {"varbinary",typeof(byte[])},
            { "varchar", typeof(string)},
            { "variant", typeof(object)},
            { "uniqueidentifier", typeof(Guid)}
        };
        #endregion

        #region 外部方法
        /// <summary>
        /// 获取数据库中的所有表
        /// </summary>
        /// <param name="schemaName">模式（架构）</param>
        /// <returns></returns>
        public override List<DbTableInfo> GetDbAllTables(string schemaName = null)
        {
            if (schemaName.IsNullOrEmpty())
                schemaName = "dbo";
            string sql = @"select [TableName] = a.name, [Description] = g.value
                            from
                                sys.tables a left join sys.extended_properties g
                                on (a.object_id = g.major_id AND g.minor_id = 0 AND g.name= 'MS_Description')
                            union
                          select [TableName] = a.name, [Description] = g.value
                            from
                                sys.views a left join sys.extended_properties g
                                on (a.object_id = g.major_id AND g.minor_id = 0 AND g.name= 'MS_Description')";

            return GetListBySql<DbTableInfo>(sql);
        }

        /// <summary>
        /// 根据表名获取表数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public override List<TableInfo> GetDbTableInfo(string tableName)
        {
            string sql = @"SELECT a.name AS ColumnName, 
	                        CONVERT(bit, (CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1 THEN 1 ELSE 0 END)) AS IsIdentity, 
	                        CONVERT(bit, (CASE WHEN(SELECT COUNT(*) FROM sysobjects WHERE (name IN
                                (SELECT name FROM sysindexes WHERE (id = a.id) AND (indid IN 
				                (SELECT indid FROM sysindexkeys WHERE (id = a.id) AND (colid IN
                                (SELECT colid FROM syscolumns WHERE (id = a.id) AND (name = a.name))))))) AND (xtype = 'PK')) > 0 THEN 1 ELSE 0 END)) AS IsKey, 
				                b.name AS ColumnType, COLUMNPROPERTY(a.id, a.name, 'PRECISION') AS ColumnLength, 
				                CONVERT(bit, (CASE WHEN a.isnullable = 1 THEN 1 ELSE 0 END)) AS IsNullable, ISNULL(e.text, '') AS DefaultValue, 
				                ISNULL(g.value, ' ') AS ColumnDescription
                            FROM sys.syscolumns AS a LEFT OUTER JOIN
                                sys.systypes AS b ON a.xtype = b.xusertype INNER JOIN
                                sys.sysobjects AS d ON a.id = d.id AND d.xtype = 'U' AND d.name <> 'dtproperties' LEFT OUTER JOIN
                                sys.syscomments AS e ON a.cdefault = e.id LEFT OUTER JOIN
                                sys.extended_properties AS g ON a.id = g.major_id AND a.colid = g.minor_id LEFT OUTER JOIN
                                sys.extended_properties AS f ON d.id = f.class AND f.minor_id = 0
                            WHERE (b.name IS NOT NULL) AND (d.name = @table_name)
                            ORDER BY a.id, a.colorder";
            return GetListBySql<TableInfo>(sql, new List<DbParameter>() { new SqlParameter("@table_name", tableName) });
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
        public override void SaveEntityToFile(List<TableInfo> infos, string tableName, string tableDescription, string filePath, string nameSpace, string schemaName = null)
        {
            base.SaveEntityToFile(infos, tableName, tableDescription, filePath, nameSpace, schemaName);
        } 
        #endregion
    }
}
