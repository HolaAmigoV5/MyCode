using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wby.Demo.ViewModel.Common;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// View/ViewModel 弹出式控制基类
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public class BaseDialogCenter<TView> where TView : Window, new()
    {
        public BaseDialogCenter() { }

        public BaseDialogCenter(ViewModel.Interfaces.IBaseDialog viewModel)
        {
            this.viewModel = viewModel;
        }

        public TView view = new TView();
        public ViewModel.Interfaces.IBaseDialog viewModel;

        /// <summary>
        /// 绑定默认ViewModel
        /// </summary>
        protected void BindDefaultViewModel()
        {
            view.DataContext = viewModel;
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> ShowDialog()
        {
            this.SubscribeMessenger();
            this.SubscribeEvent();
            this.BindDefaultViewModel();
            var result = view.ShowDialog();
            return await Task.FromResult((bool)result);
        }

        /// <summary>
        /// 注册默认事件
        /// </summary>
        public void SubscribeEvent()
        {
            view.MouseDown += (sender, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    view.DragMove();
            };
        }

        public virtual void SubscribeMessenger()
        {
            //最小化
            WeakReferenceMessenger.Default.Register<string, string>(this, "WindowMinimize", (sender, arg) =>
            {
                view.WindowState = System.Windows.WindowState.Minimized;
            });
            //最大化
            WeakReferenceMessenger.Default.Register<string, string>(this, "WindowMaximize", (sender, arg) =>
            {
                if (view.WindowState == System.Windows.WindowState.Maximized)
                    view.WindowState = System.Windows.WindowState.Normal;
                else
                    view.WindowState = System.Windows.WindowState.Maximized;
            });
            //关闭系统
            WeakReferenceMessenger.Default.Register<string, string>(this, "Exit", async (sender, arg) =>
            {
                if (!await Msg.Question("确认退出系统?")) return;
                Environment.Exit(0);
            });
        }

        public virtual void UnsubscribeMessenger()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
