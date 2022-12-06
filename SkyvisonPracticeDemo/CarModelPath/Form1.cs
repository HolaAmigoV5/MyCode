using CommonMapLib;
using i3dFdeGeometry;
using i3dRenderEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarModelPath
{
    public partial class Form1 : Form
    {
        MapOperation controlOperation = null;
        ISimplePointSymbol symbol;
        private readonly double xOffset = 0.00001141;
        private readonly double yOffset = 0.00000899;

        public Form1()
        {
            InitializeComponent();

            controlOperation = new MapOperation();
            controlOperation.InitializationMapControl(axRenderControl1, "SH.3DM");
            controlOperation.InitlizedCameraPosition();

            axRenderControl1.RcObjectEditing += AxRenderControl1_RcObjectEditing;
            axRenderControl1.RcObjectEditFinish += AxRenderControl1_RcObjectEditFinish;

            symbol = new SimplePointSymbolClass() { FillColor = 0xAA0000FF, Size = 10 };
            offSetDis = Math.Sqrt(Math.Pow(xOffset, 2) + Math.Pow(yOffset, 2));
            roadPoints = new RoadPoints
            {
                features = new List<Feature>()
            };

            comboBox1.SelectedIndex = 0;
        }

        // 开始播放轨迹
        private void button1_Click(object sender, EventArgs e)
        {
            var coo = ReadGeoJson();
            controlOperation.PlayVehicleTrajectory(coo);
            
        }

        private List<double[]> ReadGeoJson()
        {
            if (string.IsNullOrEmpty(savePath))
                savePath = Path.Combine(Environment.CurrentDirectory, "data\\road.geojson");
            if (File.Exists(savePath))
            {
                var json = File.ReadAllText(savePath);
                var points = JsonConvert.DeserializeObject<RoadPoints>(json);
                if (points != null)
                {
                    return points.features[0].geometry.coordinates;
                }
            }
            return null;
        }

        // 暂停播放轨迹
        private void button2_Click(object sender, EventArgs e)
        {
            controlOperation.PauseVehicleTrajectory();
        }

        // 继续播放轨迹
        private void button3_Click(object sender, EventArgs e)
        {
            controlOperation.ContinuePlayVT();
        }

        // 画轨迹
        private void button4_Click(object sender, EventArgs e)
        {
            controlOperation.CreatePolyline();
        }

        // 轨迹保存
        private void button5_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            controlOperation.LoadCarModel(comboBox1.Text);
        }

        // 漫游模式
        bool normal = false;
        private void button7_Click(object sender, EventArgs e)
        {
            axRenderControl1.InteractMode = normal == true ? i3dInteractMode.i3dInteractNormal : i3dInteractMode.i3dInteractSelect;
            normal = !normal;
        }

        // 初始化位置
        private void button8_Click(object sender, EventArgs e)
        {
            controlOperation.InitlizedCameraPosition();
        }

        IPolyline tempPolyline;
        IPolyline polyline;
        IPoint startPoint;
        IPoint endPoint;
        int i = 0;
        double offSetDis;
        private void AxRenderControl1_RcObjectEditing(object sender, Axi3dRenderEngine._IRenderControlEvents_RcObjectEditingEvent e)
        {
            i++;
            tempPolyline = e.geometry as IPolyline;
            startPoint = i < 3 ? tempPolyline.StartPoint : endPoint;
            endPoint = tempPolyline.EndPoint;
            if (polyline == null)
                polyline = tempPolyline;


            if (startPoint != null && endPoint != null)
            {
                Debug.WriteLine($"startPoint: x={startPoint.X}, y={startPoint.Y},z={startPoint.Z}; endPoint: x={endPoint.X}, y={endPoint.Y},z={endPoint.Z}");
                AddPoints(startPoint, endPoint);
                Debug.WriteLine($"点数量：{polyline.PointCount}");
            }
        }

        private void AddPoints(IPoint startPoint, IPoint endPoint)
        {
            double x, y, r;
            IPoint temPont = (IPoint)startPoint.Clone();
            r = Math.Sqrt(Math.Pow(startPoint.X - endPoint.X, 2) + Math.Pow(startPoint.Y - endPoint.Y, 2));
            int step = (int)(r / offSetDis);

            for (int i = 1; i < step; i++)
            {
                x = offSetDis * i * (endPoint.X - startPoint.X) / r + startPoint.X;
                y = offSetDis * i * (endPoint.Y - startPoint.Y) / r + startPoint.Y;

                //Debug.WriteLine($"Point: {x}, {y}, OffSetDis: {offSetDis}");
                temPont.SetCoords(x, y, temPont.Z, temPont.M, temPont.Id);
                //temPont.SetCoords(x, y, 5, temPont.M, temPont.Id);
                polyline.AppendPoint(temPont);
            }
        }

        StringBuilder sb = new StringBuilder();
        List<IPoint> points = new List<IPoint>();
        Dictionary<Guid, string> dic = new Dictionary<Guid, string>();
        double gid = 1, class_id = 1000, source = 10000, target = 10001;
        Geometry geometry;
        private void AxRenderControl1_RcObjectEditFinish(object sender, EventArgs e)
        {
            try
            {
                if (polyline != null)
                {
                    sb.Clear();
                    var count = polyline.PointCount;

                    if (count < 2)
                        return;

                    points.Clear();

                    for (int i = 0; i < count; i++)
                    {
                        IPoint p = polyline.GetPoint(i);
                        points.Add(p);
                        CreateRenderPoint(p);
                    }

                    AddDataToDic();

                    if (points.Count > 0)
                    {
                        geometry = new Geometry() { coordinates = new List<double[]>() };
                        points.ForEach(m => geometry.coordinates.Add(new double[] { m.X, m.Y, m.Z }));

                        Property property = new Property()
                        {
                            Gid = gid.ToString(),
                            Class_Id = "DL" + class_id.ToString(),
                            Source = source,
                            Target = target,
                            Oneway = true,
                            MaxForwardSpeed = 100,
                            MaxBackwardSpeed = 100
                        };
                        roadPoints.features.Add(new Feature() { Id = controlOperation.RenderPolyline.Guid, properties = property, geometry = geometry });
                        Save();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }

            // 漫游
            SetInteractNormal();
            polyline = null;
            i = 0;
        }

        string savePath = string.Empty;
        RoadPoints roadPoints;
        private void Save()
        {
            savePath = Path.Combine(Environment.CurrentDirectory, "data\\road.geojson");
            File.WriteAllText(savePath, JsonConvert.SerializeObject(roadPoints));
        }

        // 拾取地图元素
        bool isNormal = false;
        private void button9_Click(object sender, EventArgs e)
        {
            isNormal = !isNormal;
            if (isNormal)
            {
                axRenderControl1.InteractMode = i3dInteractMode.i3dInteractNormal;
            }
            else
            {
                axRenderControl1.InteractMode = i3dInteractMode.i3dInteractMeasurement;
                axRenderControl1.MeasurementMode = i3dMeasurementMode.i3dMeasureCoordinate;
            }
        }

        private void CreateRenderPoint(IPoint point)
        {
            var rPoint = axRenderControl1.ObjectManager.CreateRenderPoint(point, symbol);
            //Debug.WriteLine($"x={point.X},y={point.Y},z={point.Z}");
            rPoint.MaxVisibleDistance = 121313;
            //axRenderControl1.Camera.FlyToObject(rPoint.Guid, i3dActionCode.i3dActionFollowBehindAndAbove);
        }

        private void AddDataToDic()
        {
            dic.Add(controlOperation.RenderPolyline.Guid, sb.ToString().Substring(0, sb.Length - 1));
        }

        private void SetInteractNormal()
        {
            axRenderControl1.InteractMode = i3dInteractMode.i3dInteractNormal;
        }
    }
}
