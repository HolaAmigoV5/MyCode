using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFNETCoreUIDesign
{
    /// <summary>
    /// ValidateDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ValidateDataWindow : Window
    {
        int hash;
        bool discardChanges;
        public ValidateDataWindow()
        {
            InitializeComponent();
            discardChanges=false;
            var contact = new Contact("wby","abc@qq.com","好好学习天天向上！");
            hash= contact.GetHashCode();

            this.DataContext = contact;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(this.DataContext.GetHashCode()!=hash && !discardChanges)
            {
                SnackbarUnsavedChanges.IsActive = true;
                e.Cancel = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            SnackbarUnsavedChanges.IsActive = false;
            discardChanges = true;
            Close();
        }
    }
}
