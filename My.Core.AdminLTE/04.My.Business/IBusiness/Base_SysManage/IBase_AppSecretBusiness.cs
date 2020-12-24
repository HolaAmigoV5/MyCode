using My.Entity.Base_SysManage;
using My.Util;
using System.Collections.Generic;

namespace My.Business
{
    /// <summary>
    /// 描述：应用密钥接口
    /// 作者：wby 2019/11/25 14:20:13
    /// </summary>
    public interface IBase_AppSecretBusiness
    {
        List<Base_AppSecret> GetDataList(Pagination pagination, string keyword);
        Base_AppSecret GetTheData(string id);
        string GetAppSecret(string appId);
        AjaxResult AddData(Base_AppSecret newData);
        AjaxResult UpdateData(Base_AppSecret theData);
        AjaxResult DeleteData(List<string> ids);
        AjaxResult SavePermission(string appId, List<string> permissions);
    }
}
