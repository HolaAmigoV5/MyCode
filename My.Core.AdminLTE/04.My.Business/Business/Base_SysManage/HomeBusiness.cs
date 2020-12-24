using My.Business.IBusiness.Base_SysManage;
using My.Entity.Base_SysManage;
using My.Util;
using System.Linq;

namespace My.Business.Business.Base_SysManage
{
    /// <summary>
    /// 描述：HomeBusiness
    /// 作者：wby 2019/11/25 16:21:33
    /// </summary>
    public class HomeBusiness : BaseBusiness<Base_User>, IHomeBusiness, IDependency
    {
        private IOperator _operator { get; }
        public HomeBusiness(IOperator theOperator)
        {
            _operator = theOperator;
        }

        public AjaxResult SubmitLogin(string userName, string pwd)
        {
            if (userName.IsNullOrEmpty() || pwd.IsNullOrEmpty())
                return Error("账号或密码不能为空！");
            pwd = pwd.ToMD5String();
            var theUser = GetIQueryable().Where(x => x.UserName == userName && x.Password == pwd).FirstOrDefault();
            if (theUser != null)
            {
                _operator.Login(theUser.Id);
                return Success();
            }
            else
                return Error("账号或密码不正确！");
        }
    }
}
