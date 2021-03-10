using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.Dto;

namespace Wby.Demo.Service
{
    /// <summary>
    /// 基础数据服务
    /// </summary>
    public partial class BasicService : BaseService<BasicDto>, IBasicRepository
    {
    }
}
