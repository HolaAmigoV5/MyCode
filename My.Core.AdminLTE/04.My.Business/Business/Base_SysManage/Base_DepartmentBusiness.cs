using My.Business.IBusiness.Base_SysManage;
using My.Entity.Base_SysManage;
using My.Util;
using System.Collections.Generic;
using System.Linq;

namespace My.Business.Business.Base_SysManage
{
    /// <summary>
    /// 描述：部门业务实现
    /// 作者：wby 2019/11/25 15:25:14
    /// </summary>
    public class Base_DepartmentBusiness : BaseBusiness<Base_Department>, IBase_DepartmentBusiness, IDependency
    {
        [DataRepeatValidate(new string[] { "Name" }, new string[] { "部门名" })]
        [DataAddLog(LogType.部门管理, "Name", "部门名")]
        public AjaxResult AddData(Base_Department newData)
        {
            Insert(newData);
            return Success();
        }

        [DataDeleteLog(LogType.部门管理, "Name", "部门名")]
        public AjaxResult DeleteData(List<string> ids)
        {
            if (GetIQueryable().Any(x => ids.Contains(x.ParentId)))
                return Error("禁止删除！请先删除所有子级！");

            Delete(ids);
            return Success();
        }

        [DataRepeatValidate(new string[] { "Name" }, new string[] { "部门名" })]
        [DataEditLog(LogType.部门管理, "Name", "部门名")]
        public AjaxResult UpdateDate(Base_Department theData)
        {
            Update(theData);
            return Success();
        }

        public List<string> GetChildrenIds(string departmentId)
        {
            var allNodes = GetIQueryable().Select(x => new TreeModel
            {
                Id = x.Id,
                ParentId = x.ParentId,
                Text = x.Name,
                Value = x.Id
            }).ToList();

            var children = TreeHelper.GetAllChildren(allNodes, allNodes.Where(x => x.Id == departmentId).FirstOrDefault())
                .Select(x => x.Id).ToList();
            return children;

            //var q = GetIQueryable().Where(x => x.ParentId == departmentId).Select(x => x.Id).ToList();
            //return q;
        }

        public List<Base_Department> GetDataList(Pagination pagination, string departmentName = null)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<Base_Department>();
            if (!departmentName.IsNullOrEmpty())
                where = where.And(x => x.Name.Contains(departmentName));
            return q.Where(where).GetPagination(pagination).ToList();
        }

        public Base_Department GetTheData(string id)
        {
            return GetEntity(id);
        }
    }
}
