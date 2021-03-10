using Wby.Demo.Shared.Dto;

namespace Wby.Demo.Shared.HttpContact.Request
{
    /// <summary>
    /// 用户权限请求
    /// </summary>
    public class UserPermRequest : BaseRequest
    {
        public override string Route { get => "api/User/Perm"; }

        public string Account { get; set; }
    }

    /// <summary>
    /// 用户登录请求
    /// </summary>
    public class UserLoginRequest : BaseRequest
    {
        public override string Route { get => "api/User/Login"; }

        public LoginDto Parameter { get; set; }
    }
}
