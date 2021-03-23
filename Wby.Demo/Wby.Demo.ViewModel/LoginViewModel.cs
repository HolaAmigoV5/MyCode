using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;
using Wby.Demo.Shared.Common;
using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.ViewModel
{
    public class LoginViewModel : BaseDialogViewModel, ILoginViewModel
    {
        private readonly IUserRepository repository;
        public LoginViewModel(IUserRepository repository)
        {
            this.repository = repository;
            LoginCommand = new RelayCommand(Login);
        }

        #region Property
        private string userName;
        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }

        private string password;
        public string PassWord
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }
        
        #endregion

        #region Command
        public RelayCommand LoginCommand { get; private set; }

        private async void Login()
        {
            try
            {
                if (DialogIsOpen) return;
                if(string.IsNullOrWhiteSpace(UserName)|| string.IsNullOrWhiteSpace(PassWord))
                {
                    SnackBar("请输入用户名密码!");
                    return;
                }
                DialogIsOpen = true;
                await Task.Delay(300);
                var loginResult = await repository.LoginAsync(UserName, PassWord);
                if (loginResult.StatusCode != 200)
                {
                    SnackBar(loginResult.Message);
                    return;
                }
                var authResult = await repository.GetAuthListAsync();
                if (authResult.StatusCode != 200)
                {
                    SnackBar(authResult.Message);
                    return;
                }

                #region 关联用户信息/缓存
                Contract.Account = loginResult.Result.User.Account;
                Contract.UserName = loginResult.Result.User.UserName;
                Contract.IsAdmin = loginResult.Result.User.FlagAdmin == 1;
                Contract.Menus = loginResult.Result.Menus; //用户包含的菜单信息
                Contract.AuthItems = authResult.Result;
                #endregion

                //这行代码会发射到首页,Center中会定义所有的Messenger
                WeakReferenceMessenger.Default.Send(string.Empty, "NavigationPage");
            }
            catch (Exception ex)
            {
                SnackBar(ex.Message);
            }
            finally
            {
                DialogIsOpen = false;
            }
        }
        #endregion
    }
}
