using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;
using Wby.Demo.PC.Template;
using Wby.Demo.Shared.Common;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// 消息提示
    /// </summary>
    public class MsgCenter : IMsgCenter
    {
        public async Task<bool> Show(MsgInfo msgInfo)
        {
            object result = await DialogHost.Show(new MsgView()
            {
                DataContext = msgInfo
            }, "Root");
            return (bool)result;
        }
    }
}
