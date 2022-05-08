using System.Windows;
using System.Windows.Input;

namespace WPFNETCoreUIDesign
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            mainGrid.Children.Clear();
            mainGrid.Children.Add(new ShoppingCart());
        }

        private void btnAlarm_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.Children.Clear();
            mainGrid.Children.Add(new TransitionsAlarms());
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnMusic_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.Children.Clear();
            mainGrid.Children.Add(new MusicPlayer());
        }

        private void BtnShop_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.Children.Clear();
            mainGrid.Children.Add(new ShoppingCart());
        }

        private void BtnMap_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.Children.Clear();
            mainGrid.Children.Add(new BingMaps());
        }
    }
}
