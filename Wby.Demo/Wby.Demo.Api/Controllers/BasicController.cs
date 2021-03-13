using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wby.Demo.Api.ApiManager;
using Wby.Demo.Shared.Dto;
using Wby.Demo.Shared.HttpContact.Response;
using Wby.Demo.Shared.Query;

namespace Wby.Demo.Api.Controllers
{
    /// <summary>
    /// 基础数据控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BasicController : Controller
    {
        private readonly IBasicManager manager;

        public BasicController(IBasicManager manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// 获取基础数据列表
        /// </summary>
        /// <param name="param">请求参数</param>
        /// <returns>结果</returns>
        [HttpGet]
        public async Task<ApiResponse> GetAll([FromQuery] QueryParameters param) =>
          await manager.GetAll(param);

        /// <summary>
        /// 新增基础数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns>结果</returns>
        [HttpPost]
        public async Task<ApiResponse> Add([FromBody] BasicDto param) =>
            await manager.Add(param);

        /// <summary>
        /// 更新基础数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse> Update([FromBody] BasicDto param) =>
              await manager.Save(param);

        /// <summary>
        /// 删除基础数数据
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>结果</returns>
        [HttpDelete]
        public async Task<ApiResponse> Delete(int id) =>
            await manager.Delete(id);
    }
}
