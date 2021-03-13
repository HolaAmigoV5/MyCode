using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wby.Demo.EFCore;
using Wby.Demo.Shared.DataModel;
using Wby.Demo.Shared.Dto;
using Wby.Demo.Shared.HttpContact.Response;
using Wby.Demo.Shared.Query;

namespace Wby.Demo.Api.ApiManager
{
    public class MenuManager : IMenuManager
    {
        private readonly ILogger<MenuManager> logger;
        private readonly IUnitOfWork work;
        private readonly IMapper mapper;

        public MenuManager(ILogger<MenuManager> logger, IUnitOfWork work, IMapper mapper)
        {
            this.logger = logger;
            this.work = work;
            this.mapper = mapper;
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<ApiResponse> Add(MenuDto param)
        {
            try
            {
                var menu = mapper.Map<Menu>(param);
                work.GetRepository<Menu>().Insert(menu);
                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(200, "");
                return new ApiResponse(201, "");
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "");
                return new ApiResponse(201, "");
            }
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                var repository = work.GetRepository<Menu>();
                var user = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id == id);
                if (user == null)
                {
                    return new ApiResponse(201, "The menu was not found");
                }
                repository.Delete(user);
                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(200, "");
                return new ApiResponse(201, $"Deleting post { id } failed when saving.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "删除菜单错误");
                return new ApiResponse(201, "");
            }
        }


        /// <summary>
        /// 查询菜单（根据ID）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiResponse> Get(int id)
        {
            try
            {
                var model = await work.GetRepository<Menu>().GetFirstOrDefaultAsync(predicate: x => x.Id == id);
                return new ApiResponse(200, model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"根据菜单ID:{id}获取对象异常!");
                return new ApiResponse(201, "获取菜单数据异常!");
            }
        }

        /// <summary>
        /// 获取所有菜单数据(根据条件)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<ApiResponse> GetAll(QueryParameters param)
        {
            try
            {
                var models = await work.GetRepository<Menu>().GetPagedListAsync(
                    predicate: x =>
                        string.IsNullOrWhiteSpace(param.Search) ? true : x.MenuCode.Contains(param.Search) ||
                        string.IsNullOrWhiteSpace(param.Search) ? true : x.MenuName.Contains(param.Search),
                    pageIndex: param.PageIndex,
                    pageSize: param.PageSize);
                return new ApiResponse(200, models);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取所有菜单数据(根据条件)错误");
                return new ApiResponse(201, "");
            }
        }

        /// <summary>
        /// 保存菜单数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<ApiResponse> Save(MenuDto param)
        {
            throw new NotImplementedException();
        }
    }
}
