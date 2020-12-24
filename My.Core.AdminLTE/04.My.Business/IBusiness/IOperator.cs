using My.Business.IBusiness.Base_SysManage;

namespace My.Business
{
    /// <summary>
    /// 操作者接口
    /// </summary>
    public interface IOperator
    {
        /// <summary>
        /// 当前操作者
        /// </summary>
        string UserId { get; }
        Base_UserDTO Property { get; }

        #region 操作方法
        /// <summary>
        /// 是否已登录
        /// </summary>
        /// <returns></returns>
        bool Logged();

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userId">用户ID</param>
        void Login(string userId);

        /// <summary>
        /// 注销
        /// </summary>
        void Logout();

        /// <summary>
        /// 判断是否为超级管理员
        /// </summary>
        /// <returns></returns>
        bool IsAdmin(); 
        #endregion
    }
}
