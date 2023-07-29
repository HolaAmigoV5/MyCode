using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TreeGrid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainViewModel();
            master.DataContext = viewModel;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            viewModel.DetailExpanded(master.SelectedIndex);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            viewModel.DetailCollapsed(master.SelectedIndex);
        }
    }
}
