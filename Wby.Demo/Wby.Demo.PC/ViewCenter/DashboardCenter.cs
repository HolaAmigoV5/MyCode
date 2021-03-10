using Wby.Demo.PC.View;
using Wby.Demo.Shared.Attributes;
using Wby.Demo.Shared.Common.Enums;
using Wby.Demo.Shared.Dto;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// 仪表板控制类
    /// </summary>
    [Module("仪表板", ModuleType.应用中心)]
    public class DashboardCenter : ComponentCenter<DashboardView>, IDashboardCenter
    {
        public DashboardCenter(IDashboardViewModel viewModel) : base(viewModel) { }
    }
}
