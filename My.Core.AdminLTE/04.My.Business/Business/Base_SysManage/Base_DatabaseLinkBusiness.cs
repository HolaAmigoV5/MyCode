using My.Business.IBusiness.Base_SysManage;
using My.Entity.Base_SysManage;
using My.Util;
using System.Collections.Generic;
using System.Linq;

namespace My.Business.Business.Base_SysManage
{
    /// <summary>
    /// 描述：数据库连接
    /// 作者：wby 2019/11/25 15:12:25
    /// </summary>
    public class Base_DatabaseLinkBusiness : BaseBusiness<Base_DatabaseLink>, IBase_DatabaseLinkBusiness, IDependency
    {
        #region 接口实现
        public AjaxResult AddData(Base_DatabaseLink newData)
        {
            Insert(newData);
            return Success();
        }

        public AjaxResult DeleteData(List<string> ids)
        {
            Delete(ids);
            return Success();
        }

        public AjaxResult UpdateData(Base_DatabaseLink theData)
        {
            Update(theData);
            return Success();
        }

        public List<Base_DatabaseLink> GetDataList(Pagination pagination)
        {
            return GetIQueryable().GetPagination(pagination).ToList();
        }

        public Base_DatabaseLink GetTheData(string id)
        {
            return GetEntity(id);
        }
        #endregion
    }
}
