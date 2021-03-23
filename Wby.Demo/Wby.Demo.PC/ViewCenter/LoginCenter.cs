using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using Wby.Demo.PC.View;
using Wby.Demo.Shared.Common;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// 登录控制类
    /// </summary>
    public class LoginCenter : BaseDialogCenter<LoginView>, ILoginCenter
    {
        public LoginCenter(ILoginViewModel viewModel) : base(viewModel) { }

        public override void SubscribeMessenger()
        {
            WeakReferenceMessenger.Default.Register<string, string>(view, "Snackbar", (sender, arg) =>
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var messageQueue = view.SnackbarThree.MessageQueue;
                    messageQueue.Enqueue(arg);
                }));
            });

            //登陆成功后，调用MainCenter，显示主窗体
            WeakReferenceMessenger.Default.Register<string, string>(view, "NavigationPage", async (sender, arg) =>
            {
                var dialog = NetCoreProvider.ResolveNamed<IMainCenter>("MainCenter");
                this.UnsubscribeMessenger();
                view.Close();
                await dialog.ShowDialog();
            });
            base.SubscribeMessenger();
        }
    }
}
