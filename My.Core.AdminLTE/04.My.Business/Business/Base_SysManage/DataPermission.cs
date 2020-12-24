using My.Business.IBusiness.Base_SysManage;
using My.Entity.Base_SysManage;
using My.Repository;
using My.Util;
using System.Linq;
using static My.Entity.Base_SysManage.EnumType;

namespace My.Business.Business.Base_SysManage
{
    /// <summary>
    /// 描述：数据权限控制
    /// 作者：wby 2019/11/25 16:04:31
    /// </summary>
    public class DataPermission : IDataPermission, IDependency
    {
        public IOperator Operator { get => AutofacHelper.GetScopeService<IOperator>(); }
        public IBase_DepartmentBusiness DepartmentBus { get => AutofacHelper.GetScopeService<IBase_DepartmentBusiness>(); }

        public IQueryable<Base_User> GetIQ_Base_User(IRepository repository)
        {
            //根据角色来控制数据权限，超级管理员能看到所有用户，部门管理员能看到自己部门及下属机构用户
            var theUser = Operator.Property;
            var role = Operator.Property.RoleType;
            var where = LinqHelper.False<Base_User>();
            if (Operator.IsAdmin())
                where = where.Or(x => true);
            if(role.HasFlag(RoleType.部门管理员))
            {
                var departmentIdList = DepartmentBus.GetChildrenIds(theUser.DepartmentId);
                where = where.Or(x => departmentIdList.Contains(x.DepartmentId));
            }

            return repository.GetIQueryable<Base_User>().Where(where);
        }
    }
}
