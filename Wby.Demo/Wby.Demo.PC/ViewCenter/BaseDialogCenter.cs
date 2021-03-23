using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wby.Demo.ViewModel.Common;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// 弹出窗口控制类：绑定ViewModel，默认事件注册
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public class BaseDialogCenter<TView> where TView : Window, new()
    {
        public TView view = new();
        public IBaseDialog viewModel;

        public BaseDialogCenter(IBaseDialog viewModel)
        {
            this.viewModel = viewModel;
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> ShowDialog()
        {
            //订阅消息
            this.SubscribeMessenger();

            //允许拖拽
            this.SubscribeEvent();

            //绑定ViewModel
            this.BindDefaultViewModel();

            //显示窗口
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
                view.WindowState = WindowState.Minimized;
            });
            //最大化
            WeakReferenceMessenger.Default.Register<string, string>(this, "WindowMaximize", (sender, arg) =>
            {
                if (view.WindowState == WindowState.Maximized)
                    view.WindowState = WindowState.Normal;
                else
                    view.WindowState = WindowState.Maximized;
            });

            //关闭系统
            WeakReferenceMessenger.Default.Register<string, string>(this, "Exit", async (sender, arg) =>
            {
                if (!await Msg.Question("确认退出系统?")) return;
                Environment.Exit(0);
            });
        }

        /// <summary>
        /// 绑定默认ViewModel
        /// </summary>
        protected void BindDefaultViewModel()
        {
            view.DataContext = viewModel;
        }

        public virtual void UnsubscribeMessenger()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
