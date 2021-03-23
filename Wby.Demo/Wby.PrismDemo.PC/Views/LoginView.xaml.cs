using Prism.Events;
using System;
using System.Windows;
using System.Windows.Input;
using Wby.PrismDemo.PC.Infrastructure.Common;

namespace Wby.PrismDemo.PC.Views
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView(IEventAggregator ea)
        {
            InitializeComponent();
            ea?.GetEvent<MessageSentEvent>().Subscribe(LoginViewMessageReceived);
            MouseDown += DragMove;
        }

        private void LoginViewMessageReceived(string msg)
        {
            if(msg== "CloseLoginAndShowMainView")
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                var messageQueue = SnackbarThree.MessageQueue;
                messageQueue.Enqueue(msg);
            }
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Btn_close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
