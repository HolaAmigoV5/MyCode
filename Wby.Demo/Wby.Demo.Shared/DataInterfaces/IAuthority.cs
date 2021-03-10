using System.Collections.ObjectModel;
using Wby.Demo.Shared.Common;

namespace Wby.Demo.Shared.DataInterfaces
{
    /// <summary>
    /// 权限接口
    /// </summary>
    public interface IAuthority
    {
        /// <summary>
        /// 初始化权限
        /// </summary>
        /// <param name="AuthValue"></param>
        void InitPermissions(int AuthValue);

        ObservableCollection<CommandStruct> ToolBarCommandList { get; set; }
    }
}
