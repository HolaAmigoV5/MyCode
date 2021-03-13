using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wby.Demo.Api.ApiManager;
using Wby.Demo.Shared.Dto;
using Wby.Demo.Shared.HttpContact.Response;
using Wby.Demo.Shared.Query;

namespace Wby.Demo.Api.Controllers
{
    /// <summary>
    /// 用户数据控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly IUserManager manager;

        public UserController(IUserManager manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse> Login(LoginDto param) => await manager.Login(param);

        /// <summary>
        /// 获取用户数据信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> Get(int id) => await manager.Get(id);

        /// <summary>
        /// 获取用户数据列表信息
        /// </summary>
        /// <param name="parameters">请求参数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> GetAll([FromQuery] UserParameters parameters) => await manager.GetAll(parameters);

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse> Add([FromBody] UserDto param) => await manager.Add(param);

        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse> Save([FromBody] UserDto param) => await manager.Save(param);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ApiResponse> Delete(int id) => await manager.Delete(id);
    }
}
