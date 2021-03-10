using Wby.Demo.PC.View;
using Wby.Demo.Shared.Attributes;
using Wby.Demo.Shared.Common.Enums;
using Wby.Demo.Shared.Dto;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// 用户组
    /// </summary>
    [Module("权限管理", ModuleType.系统配置)]
    public class GroupCenter : ModuleCenter<GroupView, GroupDto>, IGroupCenter
    {
    }
}
