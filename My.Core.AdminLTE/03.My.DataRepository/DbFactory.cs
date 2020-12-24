using Microsoft.EntityFrameworkCore;
using My.Util;
using My.Util.DI;
using System;

namespace My.Repository
{
    /// <summary>
    /// 描述：数据库工厂
    /// 作者：wby 2019/9/26 16:56:22
    /// </summary>
    public class DbFactory
    {
        /// <summary>
        /// 根据配置文件获取数据库类型，并返回对应的工厂接口
        /// </summary>
        /// <param name="conString">链接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static IRepository GetRepository(string conString=null, DatabaseType? dbType=null)
        {
            conString = conString.IsNullOrEmpty() ? GlobalSwitch.DefaultDbConName : conString;
            //通过连接名或者连接字符串获取
            conString = DbProviderFactoryHelper.GetConStr(conString);
            dbType = dbType.IsNullOrEmpty() ? GlobalSwitch.DatabaseType : dbType;

            Type dbReposityType = Type.GetType("My.Repository." + DbProviderFactoryHelper.DbTypeToDbTypeStr(dbType.Value) + "Repository");
            var repository = Activator.CreateInstance(dbReposityType, new object[] { conString }) as IRepository;

            //请求结束自动释放
            try
            {
                AutofacHelper.GetScopeService<IDisposableContainer>().AddDisposableObj(repository);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            return repository;
        }

        /// <summary>
        /// 获取ShardingRepository
        /// </summary>
        /// <returns></returns>
        public static IShardingRepository GetShardingRepository()
        {
            return new ShardingRepository(GetRepository());
        }

        /// <summary>
        /// 根据参数获取数据库的DbContext
        /// </summary>
        /// <param name="conString">初始化参数，可为连接字符串或者DbContext</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static IRepositoryDbContext GetDbContext(string conString,DatabaseType dbType)
        {
            IRepositoryDbContext dbContext = new RepositoryDbContext(conString, dbType);
            dbContext.Database.SetCommandTimeout(5 * 60);

            return dbContext;
        }
    }
}
