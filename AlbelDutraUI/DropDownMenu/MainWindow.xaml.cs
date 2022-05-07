using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace DropDownMenu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var menuRegister = new List<SubItem>
            {
                new SubItem("Customer", new UserControlCustomers()),
                new SubItem("Providers", new UserControlProviders()),
                new SubItem("Employees", new ResponsiveHorizontalListView()),
                new SubItem("Products")
            };

            var item6 = new ItemMenu("Register", menuRegister, PackIconKind.Register);

            var menuSchedule = new List<SubItem>
            {
                new SubItem("Services"),
                new SubItem("Meetings")
            };

            var item1 = new ItemMenu("Appointments", menuSchedule, PackIconKind.Schedule);


            var menuReports = new List<SubItem>
            {
                new SubItem("Customers"),
                new SubItem("Providers"),
                new SubItem("Employees"),
                new SubItem("Products"),
                new SubItem("Stock"),
                new SubItem("Sales"),
            };

            var item2 = new ItemMenu("Reports", menuReports, PackIconKind.FileReport);


            var menuExpenses = new List<SubItem>
            {
                new SubItem("Fixed"),
                new SubItem("Variable")
            };

            var item3 = new ItemMenu("Expenses", menuExpenses, PackIconKind.ShoppingBasket);


            var menuFinancial = new List<SubItem>
            {
                new SubItem("Cash flow")
            };

            var item4 = new ItemMenu("Financial", menuFinancial, PackIconKind.ScaleBalance);

            var item0 = new ItemMenu("Dashboard", new UserControl(), PackIconKind.ViewDashboard);

            //Menu.Children.Add(new UserControlMenuItem(item0));
            Menu.Children.Add(new UserControlMenuItem(item6, this));
            Menu.Children.Add(new UserControlMenuItem(item1, this));
            Menu.Children.Add(new UserControlMenuItem(item2, this));
            Menu.Children.Add(new UserControlMenuItem(item3, this));
            Menu.Children.Add(new UserControlMenuItem(item4, this));

        }

        internal void SwitchScreen(object sender)
        {
            var screen = (UserControl)sender;
            if (screen != null)
            {
                StackPanelMain.Children.Clear();
                StackPanelMain.Children.Add(screen);
            }
        }

        private void ButtonPopUpLogout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;

            Storyboard sb = FindResource("CloseMenu") as Storyboard;
            sb.Begin();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;

            Storyboard sb = FindResource("OpenMenu") as Storyboard;
            sb.Begin();
        }
    }
}
