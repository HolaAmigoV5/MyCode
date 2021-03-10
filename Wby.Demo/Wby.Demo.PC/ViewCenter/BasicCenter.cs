using Wby.Demo.PC.View;
using Wby.Demo.Shared.Attributes;
using Wby.Demo.Shared.Common.Enums;
using Wby.Demo.Shared.Dto;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// 基础数据控制类
    /// </summary>
    [Module("基础数据", ModuleType.系统配置)]
    public class BasicCenter : ModuleCenter<BasicView, BasicDto>, IBasicCenter
    {
        public BasicCenter(IBasicViewModel viewModel) : base(viewModel)
        { }
    }
}
