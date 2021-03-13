using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wby.Demo.Api.ApiManager;
using Wby.Demo.Shared.HttpContact.Response;

namespace Wby.Demo.Api.Controllers
{
    /// <summary>
    /// 权限相关数据控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthItemController : Controller
    {
        private readonly IAuthItemManager manager;

        public AuthItemController(IAuthItemManager manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// 获取所有功能按钮列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> GetAll() =>
            await manager.GetAll();
    }
}
