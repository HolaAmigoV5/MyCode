using My.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace My.Repository
{
    /// <summary>
    /// 描述：数据库分布式事务,跨库事务
    /// 作者：wby 2019/9/27 10:48:30
    /// </summary>
    public class DistributedTransaction : ITransaction
    {

        #region 构造函数
        public DistributedTransaction(params IRepository[] repositories)
        {
            if (repositories == null)
                throw new ArgumentNullException("repositories不能为NULL");

            if (repositories.Length > 0)
            {
                repositories.ForEach(aRepository =>
                {
                    if (!_repositorys.Contains(aRepository))
                        _repositorys.Add(aRepository);
                });
            }
        } 
        #endregion

        #region 内部成员
        private IsolationLevel? _isolationLevel { get; set; }

        private ConcurrentDictionary<string, DbTransaction> _transactionMap { get; }
            = new ConcurrentDictionary<string, DbTransaction>();

        private string GetRepositoryId(IRepository repository)
        {
            return $"{ repository.DbType.ToString() }{repository.ConnectionString}";
        }

        private SynchronizedCollection<IRepository> _repositorys { get; set; }
        = new SynchronizedCollection<IRepository>();

        private object _lock { get; } = new object(); 

        private void _BeginTransaction(params IRepository[] repositorys)
        {
            repositorys.ForEach(x => Begin(x));

            void Begin(IRepository db)
            {
                lock(_lock)
                {
                    //同一个数据库共享同一个事务
                    string id = GetRepositoryId(db);
                    if (_transactionMap.ContainsKey(id))
                        db.UseTransaction(_transactionMap[id]);
                    else
                    {
                        if (_isolationLevel == null)
                            db.BeginTransaction();
                        else
                            db.BeginTransaction(_isolationLevel.Value);

                        _transactionMap[id] = db.GetTransaction();
                    }

                }
            }
        }
        #endregion

        #region 外部接口
        public void AddRepository(params IRepository[] repositories)
        {
            List<IRepository> needBeginList = new List<IRepository>();
            if(repositories.Length>0)
            {
                repositories.ForEach(aRepository => {
                    if(!_repositorys.Contains(aRepository))
                    {
                        _repositorys.Add(aRepository);
                        needBeginList.Add(aRepository);
                    }
                });
            }

            _BeginTransaction(needBeginList.ToArray());
        }

        /// <summary>
        /// 开始事物
        /// </summary>
        public ITransaction BeginTransaction()
        {
            _BeginTransaction(_repositorys.ToArray());
            return this;
        }

        /// <summary>
        /// 开始事物
        /// 注:自定义事物级别
        /// </summary>
        /// <param name="isolationLevel">事物级别</param>
        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            _isolationLevel = isolationLevel;
            _BeginTransaction(_repositorys.ToArray());

            return this;
        }

        /// <summary>
        /// 提交事物
        /// </summary>
        public void CommitTransaction()
        {
            _transactionMap.Values.ForEach(x => x.Commit());
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTransaction()
        {
            _transactionMap.Values.ForEach(x => x.Rollback());
        }

        /// <summary>
        /// 结束事务
        /// </summary>
        /// <returns></returns>
        public (bool Success, Exception ex) EndTransaction()
        {
            if (_repositorys.Count == 0)
                throw new Exception("IRepository数量不能为0");

            bool isOk = true;
            Exception resEx = null;

            try
            {
                CommitDb();
                CommitTransaction();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                isOk = false;
                resEx = ex;
            }
            finally
            {
                Dispose();
            }

            return (isOk, resEx);
        }

        public void CommitDb()
        {
            _repositorys.ForEach(x => x.CommitDb());
        }
        #endregion

        #region Dispose
        public bool Disposed { get; set; } = false;

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (disposing)
                _repositorys.ForEach(x => x.Dispose());
            Disposed = true;
        }

        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DistributedTransaction()
        {
            Dispose(false);
        } 
        #endregion
    }
}
