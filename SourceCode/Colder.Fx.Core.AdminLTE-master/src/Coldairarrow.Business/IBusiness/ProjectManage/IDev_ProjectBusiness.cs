using Coldairarrow.Entity.ProjectManage;
using Coldairarrow.Util;
using System.Collections.Generic;

namespace Coldairarrow.Business.ProjectManage
{
    public interface IDev_ProjectBusiness
    {
        List<Dev_Project> GetDataList(Pagination pagination, string condition, string keyword);
        Dev_Project GetTheData(string id);
        AjaxResult AddData(Dev_Project newData);
        AjaxResult UpdateData(Dev_Project theData);
        AjaxResult DeleteData(List<string> ids);
    }
}