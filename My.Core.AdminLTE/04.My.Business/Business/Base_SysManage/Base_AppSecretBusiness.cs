using Coldairarrow.Util;
using My.Entity.Base_SysManage;
using My.Util;
using System.Collections.Generic;
using System.Linq;

namespace My.Business.Business.Base_SysManage
{
    /// <summary>
    /// 描述：应用密匙
    /// 作者：wby 2019/11/25 14:29:43
    /// </summary>
    public class Base_AppSecretBusiness : BaseBusiness<Base_AppSecret>, IBase_AppSecretBusiness, IDependency
    {
        #region 接口实现
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="newData">数据</param>
        [DataRepeatValidate(new string[] { "AppId" }, new string[] { "应用Id" })]
        [DataAddLog(LogType.接口密钥管理, "AppId", "应用Id")]
        public AjaxResult AddData(Base_AppSecret newData)
        {
            Insert(newData);
            return Success();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="theData">删除的数据</param>
        /// 
        [DataDeleteLog(LogType.接口密钥管理, "AppId", "应用Id")]
        public AjaxResult DeleteData(List<string> ids)
        {
            Delete(ids);
            return Success();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        [DataRepeatValidate(new string[] { "AppId" }, new string[] { "应用Id" })]
        [DataEditLog(LogType.接口密钥管理, "AppId", "应用Id")]
        public AjaxResult UpdateData(Base_AppSecret theData)
        {
            Update(theData);
            return Success();
        }

        /// <summary>
        /// 获取应用密匙
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public string GetAppSecret(string appId)
        {
            return GetIQueryable().Where(x => x.AppId == appId).FirstOrDefault()?.AppSecret;
        }

        /// <summary>
        /// 关键字查询查询数据
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public List<Base_AppSecret> GetDataList(Pagination pagination, string keyword)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<Base_AppSecret>();
            if (!keyword.IsNullOrEmpty())
                where = where.And(x => x.AppId.Contains(keyword) || x.AppName.Contains(keyword) || x.AppSecret.Contains(keyword));
            return q.GetPagination(pagination).ToList();
        }

        /// <summary>
        /// 获取指定单条数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public Base_AppSecret GetTheData(string id)
        {
            return GetEntity(id);
        }

        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <param name="permissions">权限值</param>
        /// <returns></returns>
        public AjaxResult SavePermission(string appId, List<string> permissions)
        {
            Service.Delete<Base_PermissionAppId>(x => x.AppId == appId);
            List<Base_PermissionAppId> insertList = new List<Base_PermissionAppId>();
            permissions.ForEach(aPermission =>
            {
                insertList.Add(new Base_PermissionAppId
                {
                    Id = IdHelper.GetId(),
                    AppId = appId,
                    PermissionValue = aPermission
                });
            });

            Service.Insert(insertList);
            return Success();
        }
        #endregion
    }
}
