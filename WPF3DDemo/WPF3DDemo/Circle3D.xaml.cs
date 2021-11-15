using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WPF3DDemo
{
    /// <summary>
    /// Circle3D.xaml 的交互逻辑
    /// </summary>
    public partial class Circle3D : Window
    {
        public Circle3D()
        {
            InitializeComponent();

            InitModels();
        }

        private void InitModels()
        {
            my3D.Content = StereoModels.DrawCircle(new Vector3D(0, 0, 0), new Vector3D(0, 0, 1), 5, Brushes.Blue, Brushes.Green);
        }
    }
}
