using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.Dto;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.ViewModel
{
    public class UserViewModel : BaseRepository<UserDto>, IUserViewModel
    {
        public string SelectPageTitle { get; } = "用户管理";
        public UserViewModel(IUserRepository repository) : base(repository)
        {

        }
    }
}
