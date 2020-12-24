using My.Entity.Base_SysManage;
using My.Util;
using System.Collections.Generic;
using static My.Entity.Base_SysManage.EnumType;

namespace My.Business.IBusiness.Base_SysManage
{
    /// <summary>
    /// 描述：系统角色管理
    /// 作者：wby 2019/11/20 9:35:34
    /// </summary>
    public interface IBase_SysRoleBusiness
    {
        List<Base_SysRoleDTO> GetDataList(Pagination pagination, string roleId = null, string roleName = null);
        Base_SysRole GetTheData(string Id);
        Base_SysRoleDTO GetTheInfo(string Id);
        AjaxResult AddData(Base_SysRole newData);
        AjaxResult UpdateData(Base_SysRole theData);
        AjaxResult DeleteData(List<string> ids);
        AjaxResult SavePermission(string roleId, List<string> permissions);
    }

    public class Base_SysRoleDTO: Base_SysRole
    {
        public RoleType? RoleType { get => RoleName?.ToEnum<RoleType>(); }
    }
}
