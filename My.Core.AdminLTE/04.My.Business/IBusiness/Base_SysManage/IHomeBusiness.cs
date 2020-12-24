using My.Util;

namespace My.Business.IBusiness.Base_SysManage
{
    /// <summary>
    /// 描述：IHomeBusiness
    /// 作者：wby 2019/11/25 16:22:00
    /// </summary>
    public interface IHomeBusiness
    {
        AjaxResult SubmitLogin(string userName, string pwd);
    }
}
