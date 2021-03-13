using System.Threading.Tasks;
using Wby.Demo.Shared.Dto;
using Wby.Demo.Shared.HttpContact.Response;
using Wby.Demo.Shared.Query;

namespace Wby.Demo.Api.ApiManager
{
    public interface IUserManager
    {
        Task<ApiResponse> Login(LoginDto param);

        Task<ApiResponse> GetAll(QueryParameters param);

        Task<ApiResponse> Get(int id);

        Task<ApiResponse> Add(UserDto param);

        Task<ApiResponse> Delete(int id);

        Task<ApiResponse> Save(UserDto param);
    }
}
