using My.Entity.Base_SysManage;
using My.Util;
using System.Collections.Generic;

namespace My.Business.IBusiness.Base_SysManage
{
    /// <summary>
    /// 描述：部门相关业务
    /// 作者：wby 2019/11/25 15:18:54
    /// </summary>
    public interface IBase_DepartmentBusiness
    {
        List<Base_Department> GetDataList(Pagination pagination, string departmentName = null);
        Base_Department GetTheData(string id);
        List<string> GetChildrenIds(string departmentId);
        AjaxResult AddData(Base_Department newData);
        AjaxResult DeleteData(List<string> ids);
        AjaxResult UpdateDate(Base_Department theData);
    }
}
