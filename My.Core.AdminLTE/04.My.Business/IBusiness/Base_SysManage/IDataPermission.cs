using My.Entity.Base_SysManage;
using My.Repository;
using System.Linq;

namespace My.Business.IBusiness.Base_SysManage
{
    /// <summary>
    /// 描述：数据权限控制接口
    /// 作者：wby 2019/11/21 16:21:08
    /// </summary>
    public interface IDataPermission
    {
        IQueryable<Base_User> GetIQ_Base_User(IRepository repository);
    }
}
