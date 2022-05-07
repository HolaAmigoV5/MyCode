using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MenuAnimation
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

        //private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonOpenMenu.Visibility = Visibility.Visible;
        //    ButtonCloseMenu.Visibility = Visibility.Collapsed;
        //}

        //private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonOpenMenu.Visibility = Visibility.Collapsed;
        //    ButtonCloseMenu.Visibility = Visibility.Visible;
        //}

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        bool StateClosed = true;
        private void ButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            string resourceName = StateClosed ? "OpenMenu" : "CloseMenu";
            Storyboard sb = FindResource(resourceName) as Storyboard;
            sb.Begin();

            StateClosed = !StateClosed;
        }
    }
}
