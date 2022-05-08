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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFNETCoreUIDesign
{
    /// <summary>
    /// MusicPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class MusicPlayer : UserControl
    {
        public MusicPlayer()
        {
            InitializeComponent();
        }

        private void Anterior_Click(object sender, RoutedEventArgs e)
        {
            if (c1.Offset >= 0)
            {
                c1.Offset -= 0.01;
                c2.Offset -= 0.01;
            }
            else
            {
                c1.Offset = 1;
                c2.Offset = 0.89;
            }
        }

        private void Proxima_Click(object sender, RoutedEventArgs e)
        {
            if (c2.Offset <= 1)
            {
                c1.Offset += 0.01;
                c2.Offset += 0.01;
            }
            else
            {
                c1.Offset = 0.11;
                c2.Offset = 0;
            }
        }

        private void ButtonFechar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
