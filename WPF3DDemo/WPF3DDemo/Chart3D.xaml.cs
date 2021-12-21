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

namespace WPF3DDemo
{
    /// <summary>
    /// Chart3D.xaml 的交互逻辑
    /// </summary>
    public partial class Chart3D : UserControl
    {
        public Chart3D()
        {
            InitializeComponent();
        }


        private void DrawChart()
        {
            my3DGroup.Children.Clear();
        }



        #region 公用属性

        public int AxisYStep { get; set; } = 5;
        public int AxisYMaxValue { get; set; } = 50;

        public ChartData ChartData
        {
            get { return (ChartData)GetValue(ChartDataProperty); }
            set { SetValue(ChartDataProperty, value); }
        }

        public static readonly DependencyProperty ChartDataProperty =
            DependencyProperty.Register("ChartData", typeof(ChartData), typeof(Chart3D), new PropertyMetadata(default, (d, e) =>
            {
                var dc = (Chart3D)d;
                if (e.NewValue != null && e.NewValue != e.OldValue)
                {
                    dc.DrawChart();
                }
            }));
        #endregion

    }
}
