using Wby.Demo.Shared.Dto;

namespace Wby.Demo.Shared.HttpContact.Request
{
    /// <summary>
    /// 用户组模板信息请求
    /// </summary>
    public class GroupModuleRequest:BaseRequest
    {
        public override string Route { get => "api/Group/GetMenuModules"; }
    }

    /// <summary>
    /// 组明细数据请求
    /// </summary>
    public class GroupInfoRequest : BaseRequest
    {
        public override string Route { get => "api/Group/GetGroupInfo"; }

        public int id { get; set; }
    }

    /// <summary>
    /// 保存组数据请求
    /// </summary>
    public class GroupSaveRequest : BaseRequest
    {
        public override string Route { get => "api/Group/Save"; }

        public GroupDataDto groupDto { get; set; }

    }
}
