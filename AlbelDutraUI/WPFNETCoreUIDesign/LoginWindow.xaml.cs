using System;
using System.Collections.Generic;
using System.Text;
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
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            if(uname.Text=="admin" && pwd.Password == "123")
            {
                MainWindow mv = new MainWindow();
                mv.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("登录失败！");
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            SignIn sign = new SignIn();
            sign.Show();
        }
    }
}
