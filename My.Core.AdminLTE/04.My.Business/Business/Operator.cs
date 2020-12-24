using My.Util;
using static My.Entity.Base_SysManage.EnumType;

namespace My.Business
{
    /// <summary>
    /// 描述：操作者
    /// 作者：wby 2019/10/12 16:24:49
    /// </summary>
    public class Operator : IOperator, ICircleDependency
    {
        public IBase_UserBusiness _sysUserBus { get; set; }

        /// <summary>
        /// 当前操作者UserId
        /// </summary>
        public string UserId
        {
            get
            {
                if (GlobalSwitch.RunModel == RunModel.LocalTest)
                    return "Admin";
                else
                    return SessionHelper.CurrentSession["UserId"]?.ToString();
            }
        }

        public Base_UserDTO Property { get => _sysUserBus.GetTheInfo(UserId); }

        /// <summary>
        /// 判断是否为超级管理员
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin()
        {
            var role = Property.RoleType;
            if (UserId == "Admin" || role.HasFlag(RoleType.超级管理员))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 是否已登录
        /// </summary>
        /// <returns></returns>
        public bool Logged()
        {
            return !UserId.IsNullOrEmpty();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userId"></param>
        public void Login(string userId)
        {
            SessionHelper.CurrentSession["UserId"] = userId;
        }

        /// <summary>
        /// 注销
        /// </summary>
        public void Logout()
        {
            SessionHelper.CurrentSession["UserId"] = null;
            SessionHelper.RemoveSessionCookie();
        }
    }
}
