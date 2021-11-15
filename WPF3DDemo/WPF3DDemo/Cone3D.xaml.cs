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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace WPF3DDemo
{
    /// <summary>
    /// Cone3D.xaml 的交互逻辑
    /// </summary>
    public partial class Cone3D : Window
    {
        public Cone3D()
        {
            InitializeComponent();

            my3D.Content = StereoModels.DrawCone(15, new Vector3D(0, 0, 0), new Vector3D(0, 0, 1), 5, Brushes.Blue, Brushes.Green);
        }
    }
}
