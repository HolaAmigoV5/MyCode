using System.Windows;

namespace WPFNETCoreUIDesign
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SignIn : Window
    {
        public SignIn()
        {
            InitializeComponent();
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(email.Text) && !string.IsNullOrEmpty(pwd.Password))
            {
                LoginWindow login = new LoginWindow();
                login.Show();
                this.Close();
            }
        }
    }
}
