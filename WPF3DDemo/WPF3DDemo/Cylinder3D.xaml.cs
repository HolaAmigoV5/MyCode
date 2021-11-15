using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WPF3DDemo
{
    /// <summary>
    /// Cylinder3D.xaml 的交互逻辑
    /// </summary>
    public partial class Cylinder3D : Window
    {
        public Cylinder3D()
        {
            InitializeComponent();

            my3D.Content = StereoModels.DrawCylinder(10, new Vector3D(0, 0, 0), new Vector3D(0, 0, 1), 5, Brushes.Blue, Brushes.Green, Brushes.Red);
        }
    }
}
