using AspectInjector.Broker;
using System;
using System.Threading.Tasks;

namespace Wby.Demo.Shared.Common.Aop
{
    /// <summary>
    /// 全局进度
    /// </summary>
    [Aspect(Scope.Global)]
    [Injection(typeof(GlobalProgress))]
    public class GlobalProgress : Attribute
    {
        [Advice(Kind.Before, Targets = Target.Method)]
        public void Start([Argument(Source.Name)] string name)
        {
            UpdateLoading(true);
        }

        [Advice(Kind.After, Targets = Target.Method)]
        public async void End([Argument(Source.Name)] string name)
        {
            await Task.Delay(300);
            UpdateLoading(false);
        }

        void UpdateLoading(bool isOpen, string msg = "Loading...")
        {
            //Messenger.Default.Send(new MsgInfo()
            //{
            //    IsOpen = isOpen,
            //    Msg = msg
            //}, "UpdateDialog");
        }
    }
}
