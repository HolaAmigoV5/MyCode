using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wby.Demo.Api.ApiManager;
using Wby.Demo.Shared.Dto;
using Wby.Demo.Shared.HttpContact.Response;
using Wby.Demo.Shared.Query;

namespace Wby.Demo.Api.Controllers
{
    /// <summary>
    /// 菜单数据控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MenuController : Controller
    {
        private readonly IMenuManager manager;

        public MenuController(IMenuManager manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="parameters">请求参数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> GetAll([FromQuery] QueryParameters parameters)
            => await manager.GetAll(parameters);

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="parameters">请求参数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> Get(int id) => await manager.Get(id);

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="param">用户信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse> Add([FromBody] MenuDto param) => await manager.Add(param);

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ApiResponse> Delete(int id) => await manager.Delete(id);
    }
}
