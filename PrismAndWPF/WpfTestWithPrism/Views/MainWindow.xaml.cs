using System.Windows;

namespace WpfTestWithPrism.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn.Name == "btnTime")
            {
                timePopup.IsOpen = true;
            }
            else
            {
                speedPopup.IsOpen = true;
            }
            e.Handled = true;
        }
    }
}
