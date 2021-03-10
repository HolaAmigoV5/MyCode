using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.Dto;
using Wby.Demo.Shared.HttpContact;
using Wby.Demo.Shared.HttpContact.Request;

namespace Wby.Demo.Service
{
    public partial class GroupService:BaseService<GroupDto>, IGroupRepository
    {
        public async Task<BaseResponse<List<MenuModuleGroupDto>>> GetMenuModuleListAsync()
        {
            return await new BaseServiceRequest().GetRequest<BaseResponse<List<MenuModuleGroupDto>>>(new GroupModuleRequest(), Method.GET);
        }

        public async Task<BaseResponse<GroupDataDto>> GetGroupAsync(int id)
        {
            return await new BaseServiceRequest().GetRequest<BaseResponse<GroupDataDto>>(new GroupInfoRequest() { id = id }, Method.GET);
        }

        public async Task<BaseResponse> SaveGroupAsync(GroupDataDto group)
        {
            var r = await new BaseServiceRequest().GetRequest<BaseResponse>(new GroupSaveRequest()
            { groupDto = group }, Method.POST);
            return r;
        }
    }
}
