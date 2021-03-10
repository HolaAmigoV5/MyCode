namespace Wby.Demo.Shared.HttpContact.Request
{
    /// <summary>
    /// 获取功能按钮请求
    /// </summary>
    public class AuthItemRequest : BaseRequest
    {
        public override string Route { get => "api/AuthItem/GetAll"; }
    }
}
