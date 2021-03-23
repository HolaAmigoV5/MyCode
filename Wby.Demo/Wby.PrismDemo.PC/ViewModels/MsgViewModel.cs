using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace Wby.PrismDemo.PC.ViewModels
{
    public class MsgViewModel: BindableBase, IDialogAware
    {
        #region Properties
        private string color= "#20B2AA";
        public string Color
        {
            get { return color; }
            set { SetProperty(ref color, value); }
        }

        private string icon= "CommentQuestionOutline";
        public string Icon
        {
            get { return icon; }
            set { SetProperty(ref icon, value); }
        }

        private string msg;
        public string Msg
        {
            get { return msg; }
            set { SetProperty(ref msg, value); }
        }

        #endregion

        private DelegateCommand<string> closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand => closeDialogCommand ??= new DelegateCommand<string>(CloseDialog);
        private void CloseDialog(string obj)
        {
            ButtonResult result = ButtonResult.None;
            if (obj?.ToLower() == "true")
                result = ButtonResult.Yes;
            else if (obj?.ToLower() == "false")
                result = ButtonResult.No;

            RaiseRequestClose(new DialogResult(result));
        }

        //触发窗体关闭事件
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        #region IDialogAware
        private string title = "信息";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var msg = parameters.GetValue<string>("message");
            var info = msg.Split(':');
            if (info != null && info.Length > 1)
            {
                switch (info[1].Trim())
                {
                    case "Question":
                        Title = "提示";
                        Color = "#20B2AA";
                        Icon = "CommentQuestionOutline";
                        break;
                    case "Error":
                        Title = "错误";
                        Color = "#FF4500";
                        Icon = "Error";
                        break;
                    case "Warning":
                        Title = "警告";
                        Color = "#FF8247";
                        Icon = "CommentWarning";
                        break;
                    case "Info":
                        Title = "信息";
                        Color = "#1C86EE";
                        Icon = "CommentProcessingOutline";
                        break;
                }
                Msg = info[0];
            }
          
        }
        #endregion
    }
}
