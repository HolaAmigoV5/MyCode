using My.Util;
using System.Linq;

namespace My.Business._01Cache
{
    /// <summary>
    /// 描述：Base_UserDTOCache
    /// 作者：wby 2019/11/18 15:18:05
    /// </summary>
    public class Base_UserDTOCache : BaseCache<Base_UserDTO>, IBase_UserDTOCache, IDependency
    {
        public IBase_UserBusiness SysUserBus { get => AutofacHelper.GetScopeService<IBase_UserBusiness>(); }
        protected override string _moduleKey => "Base_UserDTO";

        protected override Base_UserDTO GetDbData(string key)
        {
            return SysUserBus.GetDataList(new Pagination(), true, key).FirstOrDefault();
        }
    }
}
