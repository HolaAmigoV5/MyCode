using My.Util.DI;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace My.Util
{
    /// <summary>
    /// 描述：数据库操作提供源工厂帮助类
    /// 作者：wby 2019/9/20 8:26:25
    /// </summary>
    public class DbProviderFactoryHelper
    {
        /// <summary>
        /// 获取提供工厂
        /// </summary>
        /// <param name="type">数据库类型</param>
        /// <returns></returns>
        public static DbProviderFactory GetDbProviderFactory(DatabaseType type)
        {
            DbProviderFactory factory = null;
            switch (type)
            {
                case DatabaseType.SqlServer:
                    factory = SqlClientFactory.Instance;
                    break;
                case DatabaseType.MySql:
                    factory = MySqlClientFactory.Instance;
                    break;
                case DatabaseType.Oracle:
                    factory = OracleClientFactory.Instance;
                    break;
                case DatabaseType.PostgreSql:
                    factory = NpgsqlFactory.Instance;
                    break;
                default:
                    throw new Exception("请传入有效的数据库!");
            }
            return factory;
        }

        /// <summary>
        /// 获取数据库连接对象DbConnection
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static DbConnection GetDbConnection(DatabaseType dbType)
        {
            var con = GetDbProviderFactory(dbType).CreateConnection();

            //请求结束自动释放
            try
            {
                AutofacHelper.GetScopeService<IDisposableContainer>().AddDisposableObj(con);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            return con;
        }

        public static DbConnection GetDbConnection(string conStr,DatabaseType dbType)
        {
            if (conStr.IsNullOrEmpty())
                conStr = GlobalSwitch.DefaultDbConName;
            DbConnection dbConnection = GetDbConnection(dbType);
            dbConnection.ConnectionString = GetConStr(conStr);

            return dbConnection;
        }

        /// <summary>
        /// 获取DbParameter
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static DbCommand GetDbCommand(DatabaseType dbType)
        {
            return GetDbProviderFactory(dbType).CreateCommand();
        }

        /// <summary>
        /// 获取DbParameter
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static DbParameter GetDbParameter(DatabaseType dbType)
        {
            return GetDbProviderFactory(dbType).CreateParameter();
        }

        /// <summary>
        /// 获取DataAdapter
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static DataAdapter GetDataAdapter(DatabaseType dbType)
        {
            return GetDbProviderFactory(dbType).CreateDataAdapter();
        }

        /// <summary>
        /// 将数据库类型字符串转换为对应的数据库类型
        /// </summary>
        /// <param name="dbTypeStr">数据库类型字符串</param>
        /// <returns></returns>
        public static DatabaseType DbTypeStrToDbType(string dbTypeStr)
        {
            if (dbTypeStr.IsNullOrEmpty())
                throw new ArgumentNullException("请输入数据库类型字符串！");
            else
            {
                switch (dbTypeStr.ToLower())
                {
                    case "sqlserver":
                        return DatabaseType.SqlServer;
                    case "oracle":
                        return DatabaseType.Oracle;
                    case "mysql":
                        return DatabaseType.MySql;
                    case "postgresql":
                        return DatabaseType.PostgreSql;
                    default:
                        throw new Exception("请输入合法的数据库类型字符串！");
                }
            }
        }

        /// <summary>
        /// 将数据库类型转换为对应的数据库类型字符串
        /// </summary>
        /// <param name="dbTypeStr">数据库类型字符串</param>
        /// <returns></returns>
        public static string DbTypeToDbTypeStr(DatabaseType dbType)
        {
            if (dbType.IsNullOrEmpty())
                throw new ArgumentNullException("请输入数据库类型！");
            else
                return dbType.ToString();
        }

        /// <summary>
        /// 通过连接名或连接字符串获取连接字符串
        /// </summary>
        /// <param name="nameOrconStr">连接名或者连接字符串</param>
        /// <returns></returns>
        public static string GetConStr(string nameOrconStr)
        {
            string conStr = string.Empty;
            string nameOfDbcon = string.Empty;

            //若为连接字符串
            if (nameOrconStr.Contains(";"))
                conStr = nameOfDbcon;
            //若为"name=BaseDb"形式
            else if (nameOfDbcon.Contains("name="))
            {
                var strArray = nameOfDbcon.Split("=".ToArray());
                nameOfDbcon = strArray[1];
            }
            else
                nameOfDbcon = nameOrconStr;
            if (!nameOfDbcon.IsNullOrEmpty())
                conStr = ConfigHelper.GetConnectionString(nameOfDbcon);
            return conStr;
        }
    }
}
