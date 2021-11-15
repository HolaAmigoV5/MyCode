using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace WPF3DDemo
{
    /// <summary>
    /// LCCylinder3D.xaml 的交互逻辑
    /// </summary>
    public partial class LCCylinder3D : Window
    {
        DispatcherTimer timer;
        readonly int n = 360;
        List<Brush> brushes;
        List<ModelVisual3D> modelVisual3Ds;
        List<NameValue> data;

        public LCCylinder3D()
        {
            InitializeComponent();

            timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1), IsEnabled = true };
            timer.Tick += Timer_Tick;
            timer.Start();

            brushes = new List<Brush>() { Brushes.Blue, Brushes.LightGreen, Brushes.CornflowerBlue };
            modelVisual3Ds = new List<ModelVisual3D>();
            data=new List<NameValue>();

            Init3DModel();
        }

        private void Init3DModel()
        {
            my3D.Children.Clear();
            data = GenerateDataList();
            double sum = 0;
            data.ForEach(m => sum += m.Value);
            int nStart = 0, nEnd = 0;

            for (int i = 0; i < data.Count; i++)
            {
                nEnd = nStart + (int)(n / sum * data[i].Value);
                var model3D = new ModelVisual3D()
                {
                    Content = StereoModels.DrawArcCylinder(
                    new Vector3D(0, 0, 0), 15, 20, 5, 360, nStart, nEnd, brushes[i], brushes[i], brushes[i], brushes[i]),
                    Transform = new ScaleTransform3D(1, 1, 1)
                };
                my3D.Children.Add(model3D);
                nStart = nEnd;

                modelVisual3Ds.Add(model3D);
            }
        }

        int i = 0;
        private ScaleTransform3D? preItem;
        private void Timer_Tick(object? sender, EventArgs e)
        {
            i++;
            if (i == data.Count)
                i = 0;
            if (preItem != null)
                preItem.ScaleZ = 1;

            if (modelVisual3Ds[i].Transform is ScaleTransform3D curItem)
            {
                curItem.ScaleZ = 1.68;
                preItem = curItem;
            }

            SetText(i);
        }

        private void SetText(int index)
        {
            dataName.Text = data[index].Name;
            dataValue.Text = data[index].Value.ToString();
        }

        private static List<NameValue> GenerateDataList()
        {
            return new List<NameValue>()
            {
                new NameValue(){ Name="盈浦", Value=10 },
                new NameValue(){Name="夏阳",Value=20 },
                new NameValue(){Name="徐泾",Value=30 }
            };
        }
    }

    public class NameValue
    {
        public string? Name { get; set; }
        public double Value { get; set; } = 0;
    }
}
