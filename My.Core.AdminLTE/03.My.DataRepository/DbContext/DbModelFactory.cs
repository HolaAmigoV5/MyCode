using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using My.Util;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.Conventions;
using Oracle.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;

namespace My.Repository
{
    /// <summary>
    /// 描述：DbModelFactory
    /// 作者：wby 2019/10/11 10:02:19
    /// </summary>
    public static class DbModelFactory
    {
        #region 私有成员
        private static ConcurrentDictionary<string, Type> _modelTypeMap { get; } = new ConcurrentDictionary<string, Type>();
        private static SynchronizedCollection<IRepositoryDbContext> _observers { get; } = new SynchronizedCollection<IRepositoryDbContext>();
        private static ConcurrentDictionary<string, DbCompiledModelInfo> _dbCompiledModel { get; } = new ConcurrentDictionary<string, DbCompiledModelInfo>();
        private static object _buildCompiledModelLock { get; } = new object();

        private static DbCompiledModelInfo BuildDbCompiledModelInfo(string nameOrConStr, DatabaseType dbType)
        {
            lock (_buildCompiledModelLock)
            {
                ConventionSet conventionSet = null;
                switch (dbType)
                {
                    case DatabaseType.SqlServer:
                        conventionSet = SqlServerConventionSetBuilder.Build();
                        break;
                    case DatabaseType.MySql:
                        conventionSet = MySqlConventionSetBuilder.Build();
                        break;
                    case DatabaseType.Oracle:
                        conventionSet = OracleConventionSetBuilder.Build();
                        break;
                    case DatabaseType.PostgreSql:
                        conventionSet = NpgsqlConventionSetBuilder.Build();
                        break;
                    default:
                        throw new Exception("暂不支持该数据库!");
                }

                ModelBuilder modelBuilder = new ModelBuilder(conventionSet);
                _modelTypeMap.Values.ForEach(x => modelBuilder.Model.AddEntityType(x));

                DbCompiledModelInfo newInfo = new DbCompiledModelInfo
                {
                    ConStr = nameOrConStr,
                    DatabaseType = dbType,
                    Model = modelBuilder.FinalizeModel()
                };

                return newInfo;
            }
        }

        private static string GetCompiledModelIdentity(string conStr, DatabaseType dbType)
        {
            return $"{dbType} {conStr}";
        }

        private static void RefreshModel()
        {
            _dbCompiledModel.Values.ForEach(aModelInfo =>
            {
                aModelInfo.Model = BuildDbCompiledModelInfo(aModelInfo.ConStr, aModelInfo.DatabaseType).Model;
            });
        }

        private static void InitModelType()
        {
            var assemblies = new Assembly[] { Assembly.Load("My.Entity") };
            List<Type> allTypes = new List<Type>();
            assemblies.ForEach(aAssembly =>
            {
                allTypes.AddRange(aAssembly.GetTypes());
            });

            List<Type> types = allTypes.Where(x => x.GetCustomAttribute(typeof(TableAttribute), false) != null).ToList();

            types.ForEach(aType =>
            {
                _modelTypeMap[aType.Name] = aType;
            });
        }
        #endregion

        #region 构造函数
        static DbModelFactory()
        {
            InitModelType();
        }
        #endregion

        #region 外部接口
        public static void AddObserver(IRepositoryDbContext observer)
        {
            _observers.Add(observer);
        }

        public static void RemoveObserver(IRepositoryDbContext observer)
        {
            _observers.Remove(observer);
        }

        /// <summary>
        /// 获取DbCompiledModel
        /// </summary>
        /// <param name="conStr">数据库连接名或字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static IModel GetDbCompiledModel(string conStr, DatabaseType dbType)
        {
            string modelInfoId = GetCompiledModelIdentity(conStr, dbType);
            if (_dbCompiledModel.ContainsKey(modelInfoId))
                return _dbCompiledModel[modelInfoId].Model;
            else
            {
                var theModelInfo = BuildDbCompiledModelInfo(conStr, dbType);
                _dbCompiledModel[modelInfoId] = theModelInfo;
                return theModelInfo.Model;
            }
        }

        /// <summary>
        /// 获取模型
        /// </summary>
        /// <param name="type">原类型</param>
        /// <returns></returns>
        public static Type GetModel(Type type)
        {
            string modelName = type.Name;
            if (_modelTypeMap.ContainsKey(modelName))
                return _modelTypeMap[modelName];
            else
            {
                _modelTypeMap[modelName] = type;
                RefreshModel();

                return type;
            }
        }
        #endregion

        #region 数据结构
        class DbCompiledModelInfo
        {
            public IModel Model { get; set; }
            public string ConStr { get; set; }
            public DatabaseType DatabaseType { get; set; }
        } 
        #endregion
    }
}
