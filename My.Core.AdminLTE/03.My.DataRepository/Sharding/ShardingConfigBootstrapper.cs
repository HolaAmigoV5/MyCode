using My.Util;
using System;
using System.Collections.Generic;

namespace My.Repository
{
    /// <summary>
    /// 描述：分库分表配置生成器
    /// 作者：wby 2019/11/15 14:15:40
    /// </summary>
    public class ShardingConfigBootstrapper : IShardingConfigBuilder, IAddPhysicDb, IAddAbstractTable, IAddPhysicTable
    {
        #region 私有成员
        public ShardingConfigBootstrapper() { }

        private ShardingConfig _config { get; } = ShardingConfig.Instance;
        private List<AbstractTable> _absTables { get; } = new List<AbstractTable>();
        private List<(string conString, ReadWriteType opType)> _physicDbs { get; } 
            = new List<(string conString, ReadWriteType opType)>();
        private List<(string physicTableName, string dataSourceName)> _physicTables { get; } 
            = new List<(string physicTableName, string dataSourceName)>();
        #endregion

        #region 接口实现
        /// <summary>
        /// 引导
        /// </summary>
        /// <returns></returns>
        public static IShardingConfigBuilder Bootstrap()
        {
            return new ShardingConfigBootstrapper();
        }

        public IShardingConfigBuilder AddAbsDb(string absDbName, Action<IAddAbstractTable> absTableBuilder)
        {
            var builder = new ShardingConfigBootstrapper();
            absTableBuilder(builder);
            var absTables = builder.GetPropertyValue("_absTables") as List<AbstractTable>;
            _config.AddAbsDatabase(absDbName, absTables);

            return this;
        }

        public void AddAbsTable(string absTableName, Action<IAddPhysicTable> physicTableBuilder, Func<object, string> findTable)
        {
            var physicBuilder = new ShardingConfigBootstrapper();
            physicTableBuilder(physicBuilder);
            var value = physicBuilder.GetPropertyValue("_physicTables") as List<(string physicTableName, string dataSourceName)>;
            _absTables.Add(new AbstractTable
            {
                AbsTableName = absTableName,
                FindTable = findTable,
                PhysicTables = value
            });
        }

        public void AddAbsTable(string absTableName, Action<IAddPhysicTable> physicTableBuilder, IShardingRule rule)
        {
            AddAbsTable(absTableName, physicTableBuilder, rule.FindTable);
        }

        public IShardingConfigBuilder AddDataSource(string dataSourceName, DatabaseType dbType, Action<IAddPhysicDb> physicDbBuilder)
        {
            IAddPhysicDb builder = new ShardingConfigBootstrapper();
            physicDbBuilder(builder);
            var value = builder.GetPropertyValue("_physicDbs") as List<(string conString, ReadWriteType opType)>;
            _config.AddDataSource(dataSourceName, dbType, value);

            return this;
        }

        public void AddPhsicDb(string conString, ReadWriteType opType)
        {
            _physicDbs.Add((conString, opType));
        }

        public void AddPhysicTable(string PhyTableName, string dataSourceName)
        {
            _physicTables.Add((PhyTableName, dataSourceName));
        }

        public ShardingConfig Build()
        {
            return _config;
        } 
        #endregion
    }

    #region 外部接口
    public interface IShardingConfigBuilder : IAddDataSource, IAddAbstractDb
    {
        /// <summary>
        /// 生成配置
        /// </summary>
        /// <returns></returns>
        ShardingConfig Build();
    }

    public interface IAddDataSource
    {
        /// <summary>
        /// 添加数据源
        /// </summary>
        /// <param name="dataSourceName">数据源名</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="physicDbBuilder">物理表构造器</param>
        /// <returns></returns>
        IShardingConfigBuilder AddDataSource(string dataSourceName, DatabaseType dbType, Action<IAddPhysicDb> physicDbBuilder);
    }

    public interface IAddAbstractDb
    {
        /// <summary>
        /// 添加抽象数据库
        /// </summary>
        /// <param name="absDbName">抽象数据库名</param>
        /// <param name="absTableBuilder">抽象表构造器</param>
        /// <returns></returns>
        IShardingConfigBuilder AddAbsDb(string absDbName, Action<IAddAbstractTable> absTableBuilder);
    }

    public interface IAddPhysicDb
    {
        /// <summary>
        /// 添加物理数据库
        /// </summary>
        /// <param name="conString">连接字符串</param>
        /// <param name="opType">数据库类型</param>
        void AddPhsicDb(string conString, ReadWriteType opType);
    }

    public interface IAddAbstractTable
    {
        /// <summary>
        /// 添加抽象表
        /// </summary>
        /// <param name="absTableName">抽象表名</param>
        /// <param name="physicTableBuilder">物理表构造器</param>
        /// <param name="findTable">找表规则</param>
        void AddAbsTable(string absTableName, Action<IAddPhysicTable> physicTableBuilder, Func<object, string> findTable);

        /// <summary>
        /// 添加抽象表
        /// </summary>
        /// <param name="absTableName">抽象表名</param>
        /// <param name="physicTableBuilder">物理表构造器</param>
        /// <param name="rule">找表规则</param>
        void AddAbsTable(string absTableName, Action<IAddPhysicTable> physicTableBuilder, IShardingRule rule);
    }

    public interface IAddPhysicTable
    {
        /// <summary>
        /// 添加物理表
        /// </summary>
        /// <param name="PhyTableName">物理表名</param>
        /// <param name="dataSourceName">数据源名</param>
        void AddPhysicTable(string PhyTableName, string dataSourceName);
    } 
    #endregion
}
