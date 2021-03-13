using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;
using Wby.Demo.PC.Common;
using Wby.Demo.PC.Template;
using Wby.Demo.Shared.Common;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// 首页控制类
    /// </summary>
    public class MainCenter : BaseDialogCenter<MaterialDesignMainWindow>, IMainCenter
    {
        private new readonly IMainViewModel viewModel;

        public MainCenter(IMainViewModel viewModel):base(viewModel)
        {
            this.viewModel = viewModel;
        }

        public override void SubscribeMessenger()
        {
            //非阻塞式窗口提示消息
            WeakReferenceMessenger.Default.Register<string, string>(view, "Snackbar", (send, arg) => {
                var messageQueue = view.SnackbarThree.MessageQueue;
                messageQueue.Enqueue(arg);
            });

            //阻塞式窗口提示消息
            WeakReferenceMessenger.Default.Register<MsgInfo, string>(view, "UpdateDialog", (sender, m) =>
            {
                if (m.IsOpen)
                    _ = DialogHost.Show(new SplashScreenView()
                    {
                        DataContext = new { m.Msg }
                    }, "Root");
                else
                {
                    DialogHost.Close("Root");
                }
            });
            //菜单执行相关动画及模板切换
            WeakReferenceMessenger.Default.Register<string, string>(view, "ExpandMenu", (sender, arg) =>
            {
                if (view.MENU.Width < 200)
                    AnimationHelper.CreateWidthChangedAnimation(view.MENU, 60, 200, new TimeSpan(0, 0, 0, 0, 300));
                else
                    AnimationHelper.CreateWidthChangedAnimation(view.MENU, 200, 60, new TimeSpan(0, 0, 0, 0, 300));

                //由于...
                var template = view.IC.ItemTemplateSelector;
                view.IC.ItemTemplateSelector = null;
                view.IC.ItemTemplateSelector = template;
            });
            base.SubscribeMessenger();
        }

        /// <summary>
        /// 首页重写弹窗
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> ShowDialog()
        {
            await viewModel.InitDefaultView();
            return await base.ShowDialog();
        }
    }
}
