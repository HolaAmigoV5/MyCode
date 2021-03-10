using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.Dto;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.ViewModel
{
    /// <summary>
    /// 基础数据
    /// </summary>
    public class BasicViewModel : BaseRepository<BasicDto>, IBasicViewModel
    {
        public BasicViewModel(IBasicRepository repository) : base(repository)
        {
        }
    }
}
