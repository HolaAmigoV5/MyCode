using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.Dto;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.ViewModel
{
    /// <summary>
    /// 菜单业务
    /// </summary>
    public class MenuViewModel : BaseRepository<MenuDto>, IMenuViewModel
    {
        public MenuViewModel(IMenuRepository repository) : base(repository)
        {

        }
    }
}
