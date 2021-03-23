using Prism.Events;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;

namespace Wby.PrismDemo.PC.Infrastructure.Common
{
    public static class SendMsgInfo
    {
        static readonly IEventAggregator ea = ContainerLocator.Current.Resolve<IEventAggregator>();

        public static void SendMsgToSnackBar(string msg)
        {
            ea.GetEvent<MessageSentEvent>().Publish(msg);
        }

        public static void SendCurrentModule(Module module)
        {
            ea.GetEvent<ModuleSentEvent>().Publish(module);
        }

        public static bool SendMsgToMsgView(string msg, Notify notify)
        {
            try
            {
                bool result = false;
                msg += $": {notify}";

                var dialogService = ContainerLocator.Current.Resolve<IDialogService>();
                dialogService.ShowDialog("MsgView", new DialogParameters($"message={msg}"), o =>
                {
                    if (o.Result.Equals(ButtonResult.OK))
                        result = true;
                });
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
