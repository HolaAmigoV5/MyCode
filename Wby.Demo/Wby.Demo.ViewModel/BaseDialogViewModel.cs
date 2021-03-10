using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wby.Demo.ViewModel
{
    /// <summary>
    /// MVVM基类
    /// </summary>
    public class BaseDialogViewModel : ObservableObject
    {
        public RelayCommand ExitCommand { get; private set; }
        private bool dialogIsOpen;
        public bool DialogIsOpen
        {
            get { return dialogIsOpen; }
            set { SetProperty(ref dialogIsOpen, value); }
        }
        public BaseDialogViewModel()
        {
            ExitCommand = new RelayCommand(Exit);
        }

        public virtual void Exit()
        {
            WeakReferenceMessenger.Default.Send("", "Exit");
        }

        /// <summary>
        /// 通知异常
        /// </summary>
        /// <param name="msg"></param>
        public void SnackBar(string msg)
        {
            WeakReferenceMessenger.Default.Send(msg, "Snackbar");
        }
    }
}
