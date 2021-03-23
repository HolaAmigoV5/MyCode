using Prism.Services.Dialogs;
using System;
using System.Windows;
using Wby.PrismDemo.PC.Infrastructure.Common;

namespace Wby.PrismDemo.PC.Views.Dialogs
{
    /// <summary>
    /// DialogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DialogWindow : Window, IDialogWindow
    {
        public DialogWindow()
        {
            InitializeComponent();
        }

        
        protected override void OnSourceInitialized(EventArgs e)
        {
            WindowHelp.RemoveIcon(this);  //使用win32函数去除Window的icon部分
        }

        public IDialogResult Result { get; set; }
    }
}
