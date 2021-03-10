using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;
using Wby.Demo.PC.Template;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// 消息提示
    /// </summary>
    public class MsgCenter : IMsgCenter
    {
        public async Task<bool> Show(object obj)
        {
            object result = await DialogHost.Show(new MsgView()
            {
                DataContext = new { obj }
            }, "Root");
            return (bool)result;
        }
    }
}
