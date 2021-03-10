using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.Dto;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.ViewModel
{
    public class UserViewModel : BaseRepository<UserDto>, IUserViewModel
    {
        public UserViewModel(IUserRepository repository) : base(repository)
        {

        }
    }
}
