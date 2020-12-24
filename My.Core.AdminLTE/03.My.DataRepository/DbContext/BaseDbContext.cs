using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using My.Util;
using System;
using System.Data.Common;

namespace My.Repository
{
    /// <summary>
    /// 描述：BaseDbContext
    /// 作者：wby 2019/10/11 13:57:31
    /// </summary>
    public class BaseDbContext : DbContext
    {
        #region 私有成员
        private DatabaseType _dbType { get; }
        private DbConnection _dbConnection { get; }
        private IModel _model { get; }

        private static ILoggerFactory _loggerFactory = new LoggerFactory(new ILoggerProvider[] { new EFCoreSqlLogeerProvider() });
        #endregion

        #region 构造函数
        public BaseDbContext(DatabaseType dbType, DbConnection exitingConnection, IModel model)
        {
            _dbType = dbType;
            _dbConnection = exitingConnection;
            _model = model;
        }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (_dbType)
            {
                case DatabaseType.SqlServer:
                    optionsBuilder.UseSqlServer(_dbConnection, x => x.UseRowNumberForPaging());
                    break;
                case DatabaseType.MySql:
                    optionsBuilder.UseMySql(_dbConnection);
                    break;
                case DatabaseType.Oracle:
                    optionsBuilder.UseOracle(_dbConnection, x => x.UseOracleSQLCompatibility("11"));
                    break;
                case DatabaseType.PostgreSql:
                    optionsBuilder.UseNpgsql(_dbConnection);
                    break;
                default:
                    throw new Exception("暂不支持该数据库!");
            }

            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseModel(_model);
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
    }
}
