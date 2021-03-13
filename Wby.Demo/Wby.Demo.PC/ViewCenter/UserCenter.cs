using Wby.Demo.PC.Common;
using Wby.Demo.PC.View;
using Wby.Demo.Shared.Attributes;
using Wby.Demo.Shared.Common.Enums;
using Wby.Demo.Shared.Dto;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// 用户管理类
    /// </summary>
    [Module("用户管理", ModuleType.系统配置)]
    public class UserCenter : ModuleCenter<UserView, UserDto>, IUserCenter
    {
        public UserCenter(IUserViewModel viewModel):base(viewModel)
        {

        }

        public override void BindDataGridColumns()
        {
            VisualHelper.SetDataGridColumns(view, "Grid", typeof(UserDto));
        }
    }
}
