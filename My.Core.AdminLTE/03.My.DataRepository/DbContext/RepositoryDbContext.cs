using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using My.Util;

namespace My.Repository
{
    /// <summary>
    /// 描述：DbContext容器
    /// 作者：wby 2019/10/11 9:58:31
    /// </summary>
    public class RepositoryDbContext : IRepositoryDbContext
    {
        #region 私有成员
        private string _conString { get; }
        private DatabaseType _dbType { get; }
        private DbTransaction _transaction { get; set; }
        private BaseDbContext _db { get; set; }
        private Type CheckModel(Type type)
        {
            Type model = DbModelFactory.GetModel(type);
            return model;
        }
        private Action<string> _HandleSqlLog { get; set; }
        #endregion

        #region 构造函数
        public RepositoryDbContext(string conString, DatabaseType dbType)
        {
            _conString = conString;
            _dbType = dbType;
            RefreshDb();
            DbModelFactory.AddObserver(this);
        }
        #endregion

        #region 接口实现
        public DatabaseFacade Database => _db.Database;

        public EntityEntry Attach(object entity)
        {
            var type = entity.GetType();
            var model = CheckModel(entity.GetType());
            object targetObj;
            if (type == model)
                targetObj = entity;
            else
                targetObj = entity.ChangeType(model);
            return _db.Attach(targetObj);
        }

        public Type CheckEntityType(Type entityType)
        {
            return CheckModel(entityType);
        }

        public EntityEntry Entry(object entity)
        {
            var type = entity.GetType();
            var model = CheckModel(type);
            object targetObj;
            if (type == model)
                targetObj = entity;
            else
                targetObj = entity.ChangeType(model);
            return _db.Entry(targetObj);
        }

        public DbContext GetDbContext()
        {
            return _db;
        }

        public IQueryable GetIQueryable(Type type)
        {
            var model = CheckModel(type);
            return _db.GetQueryable(model);
        }

        public void RefreshDb()
        {
            //重用DbConnection,使用底层相同的DbConnection,支持Model持续更新
            DbConnection con = null;
            if (_transaction != null)
                con = _transaction.Connection;
            else
                con = _db?.Database?.GetDbConnection() ?? DbProviderFactoryHelper.GetDbConnection(_conString, _dbType);

            var dBCompiledModel = DbModelFactory.GetDbCompiledModel(_conString, _dbType);
            _db = new BaseDbContext(_dbType, con, dBCompiledModel);
            _db.Database.UseTransaction(_transaction);
            disposedValue = false;
        }

        public int SaveChanges()
        {
            return _db.SaveChanges();
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return _db.Set<TEntity>();
        }

        public void UseTransaction(DbTransaction transaction)
        {
            if (_transaction == transaction)
                return;
            if (_transaction == null && _db.Database.GetDbConnection() == transaction.Connection)
                _transaction = transaction;
            if (_transaction == null && _db.Database.GetDbConnection() != transaction.Connection)
            {
                _transaction = transaction;
                RefreshDb();
            }
        } 
        #endregion

        #region Dispose
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if (disposing)
                    _db?.Dispose();
                _transaction = null;
                DbModelFactory.RemoveObserver(this);
                disposedValue = false;
            }
        }
        ~RepositoryDbContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
        #endregion
    }
}
