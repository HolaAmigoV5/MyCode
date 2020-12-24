using System;
using System.Data;

namespace My.Repository
{
    /// <summary>
    /// 描述：事务接口
    /// 作者：wby 2019/9/26 17:00:38
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// 开始事务
        /// </summary>
        ITransaction BeginTransaction();

        /// <summary>
        /// 开始事务
        /// 注:自定义事物级别
        /// </summary>
        /// <param name="isolationLevel">事物级别</param>
        ITransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// 结束事务
        /// </summary>
        /// <returns></returns>
        (bool Success, Exception ex) EndTransaction();
    }
}
