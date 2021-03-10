using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.DataModel;
using Wby.Demo.Shared.Dto;
using Wby.Demo.Shared.HttpContact;
using Wby.Demo.Shared.HttpContact.Request;

namespace Wby.Demo.Service
{
    public class UserService : BaseService<UserDto>, IUserRepository
    {
        public async Task<BaseResponse<List<AuthItem>>> GetAuthListAsync()
        {
            return await new BaseServiceRequest().GetRequest<BaseResponse<List<AuthItem>>>(new AuthItemRequest(), Method.GET);
        }

        public async Task<BaseResponse> GetUserPermByAccountAsync(string account)
        {
            return await new BaseServiceRequest().GetRequest<BaseResponse>(new UserPermRequest()
            {
                Account = account
            }, Method.GET);
        }

        public async Task<BaseResponse<UserInfoDto>> LoginAsync(string account, string passWord)
        {
            return await new BaseServiceRequest().GetRequest<BaseResponse<UserInfoDto>>(new UserLoginRequest()
            {
                Parameter = new LoginDto() { Account = account, PassWord = passWord }
            }, Method.POST);
        }
    }
}
