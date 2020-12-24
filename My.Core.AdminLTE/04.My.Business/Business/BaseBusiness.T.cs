using My.Repository;
using My.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace My.Business
{
    /// <summary>
    /// 描述：基础业务操作
    /// 作者：wby 2019/11/18 16:11:09
    /// </summary>
    public class BaseBusiness<T> : IBaseBusiness<T>, IDependency where T : class, new()
    {
        #region 成员构造
        public ILogger Logger { protected get; set; }
        private string _conString { get; }
        private DatabaseType? _dbType { get; }
        private IRepository _service { get; set; }
        private object _serviceLock = new object();

        public BaseBusiness() { }
        public BaseBusiness(string conStr)
        {
            _conString = conStr;
        }

        public BaseBusiness(string conStr, DatabaseType dbType)
        {
            _conString = conStr;
            _dbType = dbType;
        } 

        /// <summary>
        /// 底层仓储接口，支持跨表操作
        /// </summary>
        public IRepository Service
        {
            get
            {
                if (_service == null)  //双if+lock
                {
                    lock (_serviceLock)
                    {
                        if (_service == null)
                            _service = DbFactory.GetRepository(_conString, _dbType);
                    }
                }

                return _service;
            }
        }
        #endregion

        #region 事务相关
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns></returns>
        public ITransaction BeginTransaction()
        {
            return Service.BeginTransaction();
        }

        /// <summary>
        /// 开始事务
        /// 注：自定义事务级别
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return Service.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
        {
            Service.CommitTransaction();
        }

        /// <summary>
        /// 结束事务
        /// </summary>
        /// <returns></returns>
        public (bool Success, Exception ex) EndTransaction()
        {
            return Service.EndTransaction();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTransaction()
        {
            Service.RollbackTransaction();
        }
        #endregion

        #region 添加数据
        public void BulkInsert(List<T> entities)
        {
            Service.BulkInsert(entities);
        }

        public void Insert(T entity)
        {
            Service.Insert(entity);
        }

        public void Insert(List<T> entities)
        {
            Service.Insert(entities);
        }
        #endregion

        #region 删除数据
        public void Delete(string key)
        {
            Service.Delete<T>(key);
        }

        public void Delete(List<string> keys)
        {
            Service.Delete<T>(keys);
        }

        public void Delete(T entity)
        {
            Service.Delete(entity);
        }

        public void Delete(List<T> entities)
        {
            Service.Delete(entities);
        }

        public void Delete(Expression<Func<T, bool>> condition)
        {
            Service.Delete(condition);
        }

        public void DeleteAll()
        {
            Service.DeleteAll<T>();
        }

        public int Delete_Sql(Expression<Func<T, bool>> where)
        {
            return Service.Delete_Sql<T>(where);
        }
        #endregion

        #region 修改数据
        public void Update(T entity)
        {
            Service.Update(entity);
        }

        public void Update(List<T> entities)
        {
            Service.Update(entities);
        }

        public void UpdateAny(T entity, List<string> properties)
        {
            Service.UpdateAny(entity, properties);
        }

        public void UpdateAny(List<T> entities, List<string> properties)
        {
            Service.UpdateAny(entities, properties);
        }

        public void UpdateWhere(Expression<Func<T, bool>> whereExpre, Action<T> set)
        {
            Service.UpdateWhere(whereExpre, set);
        }

        public int UpdateWhere_Sql(Expression<Func<T, bool>> where, params (string field, object value)[] values)
        {
            return Service.UpdateWhere_Sql(where, values);
        }
        #endregion

        #region 查询数据
        public DataTable GetDataTableWithSql(string sql)
        {
            return Service.GetDataTableWithSql(sql);
        }

        public DataTable GetDataTableWithSql(string sql, List<DbParameter> parameters)
        {
            return Service.GetDataTableWithSql(sql, parameters);
        }

        public T GetEntity(params object[] keyValue)
        {
            return Service.GetEntity<T>(keyValue);
        }

        public virtual IQueryable<T> GetIQueryable()
        {
            return Service.GetIQueryable<T>();
        }

        public List<T> GetList()
        {
            return Service.GetList<T>();
        }

        public List<U> GetListBySql<U>(string sqlStr) where U : class, new()
        {
            return Service.GetListBySql<U>(sqlStr);
        }

        public List<U> GetListBySql<U>(string sqlStr, List<DbParameter> param) where U : class, new()
        {
            return Service.GetListBySql<U>(sqlStr, param);
        }
        #endregion

        #region 执行SQL
        public int ExecuteSql(string sql)
        {
            return Service.ExecuteSql(sql);
        }

        public int ExecuteSql(string sql, List<DbParameter> parameters)
        {
            return Service.ExecuteSql(sql, parameters);
        }
        #endregion

        #region 业务操作
        public AjaxResult Success()
        {
            AjaxResult res = new AjaxResult
            {
                Success = true,
                Msg = "请求成功！",
                Data = null
            };

            return res;
        }

        public AjaxResult Success(string msg)
        {
            AjaxResult res = new AjaxResult { Success = true, Msg = msg, Data = null };
            return res;
        }

        public AjaxResult Success(object data)
        {
            AjaxResult res = new AjaxResult { Success = true, Msg = "请求成功!", Data = data };
            return res;
        }

        public AjaxResult Success(string msg, object data)
        {
            AjaxResult res = new AjaxResult { Success = true, Msg = msg, Data = data };
            return res;
        }

        public AjaxResult Error()
        {
            AjaxResult res = new AjaxResult { Success = false, Msg = "请求失败！", Data = null };
            return res;
        }

        public AjaxResult Error(string msg)
        {
            AjaxResult res = new AjaxResult { Success = false, Msg = msg, Data = null };
            return res;
        }

        /// <summary>
        /// 构建前端Select远程搜索数据
        /// </summary>
        /// <param name="selectedValueJson">已选择的项，JSON数组</param>
        /// <param name="q">查询关键字</param>
        /// <param name="textField">文本字段</param>
        /// <param name="valueField">值字段</param>
        /// <returns></returns>
        public virtual List<T> BuildSelectResult(string selectedValueJson, string q, string textField, string valueField)
        {
            return BuildSelectResult(selectedValueJson, q, textField, valueField, null);
        }

        /// <summary>
        /// 构建前端Select远程搜索数据
        /// </summary>
        /// <param name="selectedValueJson">已选择的项，JSON数组</param>
        /// <param name="q">查询关键字</param>
        /// <param name="textField">文本字段</param>
        /// <param name="valueField">值字段</param>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual List<T> BuildSelectResult(string selectedValueJson, string q,
            string textField, string valueField, IQueryable<T> source = null)
        {
            Pagination pagination = new Pagination { PageRows = 10 };

            List<T> selectedList = new List<T>();
            List<T> newQList = new List<T>();
            var iq = new BaseBusiness<T>().GetIQueryable();
            string where = " 1=1";
            List<string> ids = selectedValueJson?.ToList<string>() ?? new List<string>();
            if (ids.Count > 0)
            {
                selectedList = GetNewQ().Where($"@0.Contains({valueField})", ids).ToList();
                where += $" && !@0.Contains({valueField})";
            }

            if (!q.IsNullOrEmpty())
            {
                where += $" && it.{textField}.Contains(@1)";
            }

            newQList = GetNewQ().Where(where, ids, q).GetPagination(pagination).ToList();

            return selectedList.Concat(newQList).ToList();

            IQueryable<T> GetNewQ()
            {
                return source ?? GetIQueryable();
            }
        } 
        #endregion

        #region Dispose
        public void Dispose()
        {
            _service?.Dispose();
        }
        #endregion
    }
}
