using My.Business.IBusiness.Base_SysManage;
using My.Entity.Base_SysManage;
using My.Util;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Coldairarrow.Util;

namespace My.Business.Business.Base_SysManage
{
    /// <summary>
    /// 描述：系统角色管理
    /// 作者：wby 2019/11/20 10:00:39
    /// </summary>
    public class Base_SysRoleBusiness : BaseBusiness<Base_SysRole>, IBase_SysRoleBusiness, IDependency
    {
        #region 构造函数
        IPermissionManage _permissionManage { get; }
        public Base_SysRoleBusiness(IPermissionManage permissionManage)
        {
            _permissionManage = permissionManage;
        }
        #endregion

        #region 接口实现
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="newData">数据</param>
        /// <returns></returns>
        [DataAddLog(LogType.系统角色管理, "RoleName", "角色")]
        [DataRepeatValidate(new string[] { "RoleName" }, new string[] { "角色名" })]
        public AjaxResult AddData(Base_SysRole newData)
        {
            Insert(newData);
            return Success();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="ids">键集合</param>
        /// <returns></returns>
        [DataDeleteLog(LogType.系统角色管理, "RoleName", "角色")]
        public AjaxResult DeleteData(List<string> ids)
        {
            Delete(ids);
            return Success();
        }

        [DataEditLog(LogType.系统角色管理, "RoleName", "角色")]
        [DataRepeatValidate(new string[] { "RoleName" }, new string[] { "角色名" })]
        public AjaxResult UpdateData(Base_SysRole theData)
        {
            Update(theData);
            return Success();
        }

        public List<Base_SysRoleDTO> GetDataList(Pagination pagination,
            string roleId = null, string roleName = null)
        {
            var where = LinqHelper.True<Base_SysRole>();
            if (!roleId.IsNullOrEmpty())
                where = where.And(x => x.Id == roleId);
            if (!roleName.IsNullOrEmpty())
                where = where.And(x => x.RoleName.Contains(roleName));

            var list = GetIQueryable().Where(where).GetPagination(pagination).ToList()
                .Select(x => Mapper.Map<Base_SysRoleDTO>(x)).ToList();
            return list;
        }

        /// <summary>
        /// 获取指定的单条数据
        /// </summary>
        /// <param name="Id">主键</param>
        /// <returns></returns>
        public Base_SysRole GetTheData(string Id)
        {
            return GetEntity(Id);
        }

        /// <summary>
        /// 获取指定的单条数据
        /// </summary>
        /// <param name="Id">主键</param>
        /// <returns></returns>
        public Base_SysRoleDTO GetTheInfo(string Id)
        {
            return GetDataList(new Pagination(), Id).FirstOrDefault();
        }

        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="permissions">权限值</param>
        /// <returns></returns>
        public AjaxResult SavePermission(string roleId, List<string> permissions)
        {
            Service.Delete<Base_PermissionRole>(x => x.RoleId == roleId);
            List<Base_PermissionRole> insertList = new List<Base_PermissionRole>();
            permissions.ForEach(newPermission =>
            {
                insertList.Add(new Base_PermissionRole
                {
                    Id = IdHelper.GetId(),
                    RoleId = roleId,
                    PermissionValue = newPermission
                });
            });

            Service.Insert(insertList);
            _permissionManage.ClearUserPermissionCache();

            return Success();
        } 
        #endregion
    }
}
