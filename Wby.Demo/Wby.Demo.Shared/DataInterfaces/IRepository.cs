using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wby.Demo.Shared.Collections;
using Wby.Demo.Shared.DataModel;
using Wby.Demo.Shared.Dto;
using Wby.Demo.Shared.HttpContact;
using Wby.Demo.Shared.Query;

namespace Wby.Demo.Shared.DataInterfaces
{
    public interface IRepository<T>
    {
        Task<BaseResponse<PagedList<T>>> GetAllListAsync(QueryParameters parameters);
        Task<BaseResponse<T>> GetAsync(int id);
        Task<BaseResponse> SaveAsync(T model);

        Task<BaseResponse> AddAsync(T model);

        Task<BaseResponse> DeleteAsync(int id);

        Task<BaseResponse> UpdateAsync(T model);

    }

    public interface IUserRepository : IRepository<UserDto>
    {
        Task<BaseResponse<UserInfoDto>> LoginAsync(string account, string passWord);
        Task<BaseResponse> GetUserPermByAccountAsync(string account);
        Task<BaseResponse<List<AuthItem>>> GetAuthListAsync();
    }

    public interface IGroupRepository : IRepository<GroupDto>
    {
        /// <summary>
        /// 获取菜单模块列表(包含每个菜单拥有的一些功能)
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse<List<MenuModuleGroupDto>>> GetMenuModuleListAsync();

        /// <summary>
        /// 根据ID获取用户组信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResponse<GroupDataDto>> GetGroupAsync(int id);


        /// <summary>
        /// 保存组数据
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        Task<BaseResponse> SaveGroupAsync(GroupDataDto group);

    }

    public interface IMenuRepository : IRepository<MenuDto>
    {

    }

    public interface IBasicRepository : IRepository<BasicDto>
    {

    }
}
