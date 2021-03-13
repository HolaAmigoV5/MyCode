using System.Threading.Tasks;
using Wby.Demo.Shared.HttpContact.Response;

namespace Wby.Demo.Api.ApiManager
{
    public interface IAuthItemManager
    {
        Task<ApiResponse> GetAll();
    }
}
