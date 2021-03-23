using Wby.Demo.Shared.Attributes;
using Wby.Demo.Shared.Common.Enums;
using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.Dto;

namespace Wby.PrismDemo.PC.ViewModels
{
    [Module("菜单管理", ModuleType.系统配置)]
    public class MenuViewModel : BaseViewModel<MenuDto>
    {
        public MenuViewModel(IMenuRepository repository) : base(repository)
        {

        }
    }
}
