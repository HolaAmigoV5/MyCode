using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wby.Demo.Shared.Common;
using Wby.Demo.Shared.DataInterfaces;
using Wby.PrismDemo.PC.Infrastructure.Common;
using Wby.PrismDemo.PC.Views;

namespace Wby.PrismDemo.PC.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        public LoginViewModel(IUserRepository repository, IEventAggregator ea)
        {
            this.repository = repository;
            _ea = ea;
        }

        #region Properties
        private readonly IEventAggregator _ea;

        private IUserRepository repository;

        private string userName;
        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }

        private bool dialogIsOpen;
        public bool DialogIsOpen
        {
            get { return dialogIsOpen; }
            set { SetProperty(ref dialogIsOpen, value); }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private bool snackBarIsActive = false;
        public bool SnackBarIsActive
        {
            get { return snackBarIsActive; }
            set { SetProperty(ref snackBarIsActive, value); }
        }
        #endregion

        #region Command
        private DelegateCommand<PasswordBox> _loginCommand;
        public DelegateCommand<PasswordBox> LoginCommand => _loginCommand ??= new DelegateCommand<PasswordBox>(Login);

        #endregion

        #region Methods
        private async void Login(PasswordBox pwd)
        {
            try
            {
                if (DialogIsOpen) return;
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(pwd.Password))
                {
                    SendMessage("请输入用户名密码!");
                    return;
                }
                DialogIsOpen = true;
                await Task.Delay(300);
                var loginResult = await repository.LoginAsync(UserName, pwd.Password);

                if (loginResult?.StatusCode != 200)
                {
                    SendMessage(loginResult.Message);
                    return;
                }
                var authResult = await repository.GetAuthListAsync();
                if (authResult?.StatusCode != 200)
                {
                    SendMessage(loginResult.Message);
                    return;
                }
                //关联用户信息/缓存

                Contract.Account = loginResult.Result.User.Account;
                Contract.UserName = loginResult.Result.User.UserName;
                Contract.IsAdmin = loginResult.Result.User.FlagAdmin == 1;
                Contract.Menus = loginResult.Result.Menus; //用户包含的权限信息
                Contract.AuthItems = authResult.Result;

                //登录成功，显示主界面
                ShellSwitcher.Switch<LoginView, MainWindow>();
                //SendMessage("CloseLoginAndShowMainView");
            }
            catch (Exception ex)
            {
                SendMessage(ex.Message);
            }
            finally
            {
                dialogIsOpen = false;
            }
        }

        private void SendMessage(string msg)
        {
            _ea.GetEvent<MessageSentEvent>().Publish(msg);
        }

        #endregion
    }
}
