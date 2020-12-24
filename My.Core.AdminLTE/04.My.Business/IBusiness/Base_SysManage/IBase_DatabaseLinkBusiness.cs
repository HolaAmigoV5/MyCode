using My.Entity.Base_SysManage;
using My.Util;
using System.Collections.Generic;

namespace My.Business.IBusiness.Base_SysManage
{
    /// <summary>
    /// 描述：数据库连接
    /// 作者：wby 2019/11/25 15:02:39
    /// </summary>
    public interface IBase_DatabaseLinkBusiness
    {
        List<Base_DatabaseLink> GetDataList(Pagination pagination);
        Base_DatabaseLink GetTheData(string id);
        AjaxResult AddData(Base_DatabaseLink newData);
        AjaxResult UpdateData(Base_DatabaseLink theData);
        AjaxResult DeleteData(List<string> ids);
    }
}
