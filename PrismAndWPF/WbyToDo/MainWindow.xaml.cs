using GalaSoft.MvvmLight.Messaging;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WbyToDo.Models;
using WbyToDo.ViewModel;

namespace WbyToDo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.MouseDown += (sender, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };

            Messenger.Default.Register<TaskInfo>(this, "Expand", ExpandColumn);
            this.DataContext = new MainViewModel();
        }

        private void InputText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string inputValue = inputText.Text.Trim();
                if (string.IsNullOrEmpty(inputValue)) return;

                var vm = this.DataContext as MainViewModel;
                vm.AddTaskInfo(inputValue);
                inputText.Text = string.Empty;
            }
        }

        private void ExpandColumn(TaskInfo info)
        {
            var cdf = grc.ColumnDefinitions;
            //cdf[2].Width = cdf[2].Width == new GridLength(0) ? new GridLength(280) : new GridLength(0);
            //controlSizePanel.Visibility = cdf[2].Width == new GridLength(0) ? Visibility.Visible : Visibility.Collapsed;

            if (cdf[2].Width == new GridLength(0))
            {
                cdf[2].Width = new GridLength(280);
                controlSizePanel.Visibility = Visibility.Collapsed;
            }
            
        }

        private void BtnMinClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Btnmaxclick(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Btncloseclick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnUnExpand(object sender, RoutedEventArgs e)
        {
            var cdf = grc.ColumnDefinitions;
            cdf[2].Width = new GridLength(0);
            controlSizePanel.Visibility = Visibility.Visible;
        }
    }
}
