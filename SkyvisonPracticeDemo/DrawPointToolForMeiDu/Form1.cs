using i3dFdeGeometry;
using i3dRenderEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DrawPointToolForMeiDu
{
    public partial class Form1 : Form
    {
        private string i3dmPath = Environment.CurrentDirectory + "\\data\\XJJD.3DM";
        MapOperation mapOperation;
        IPolyline tempPolyline;
        IPolyline polyline;
        List<IPoint> points = new List<IPoint>();
        RoadPoints roadPoints;
        ISimplePointSymbol symbol;

        double gid = 1, class_id = 1000, source = 10000, target = 10001, totalCount;
        string path = Environment.CurrentDirectory;
        Geometry geometry;
        string savePath = string.Empty;

        Dictionary<Guid, string> dic = new Dictionary<Guid, string>();
        StringBuilder sb = new StringBuilder();

        public Form1()
        {
            try
            {
                InitializeComponent();

                savePath = Path.Combine(path, "data\\road.geojson");
                mapOperation = new MapOperation(axRenderControl1, i3dmPath);
                mapOperation.InitializationAxRenderControl();

                axRenderControl1.RcObjectEditing += AxRenderControl1_RcObjectEditing;
                axRenderControl1.RcObjectEditFinish += AxRenderControl1_RcObjectEditFinish;
                axRenderControl1.RcMouseClickSelect += AxRenderControl1_RcMouseClickSelect;

                roadPoints = new RoadPoints
                {
                    features = new List<Feature>()
                };
                ReadGeoJson();

                symbol = new SimplePointSymbolClass() { FillColor = 0xAA0000FF, Size = 10 };
                offSetDis = Math.Sqrt(Math.Pow(xOffset, 2) + Math.Pow(yOffset, 2));
                comboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }    
        }

        private void ReadGeoJson()
        {
            if (File.Exists(savePath))
            {
                var json = File.ReadAllText(savePath);
                roadPoints = JsonConvert.DeserializeObject<RoadPoints>(json);



                richTextBox1.Text = json;

                //foreach (var item in roadPoints.features)
                //{
                //    var gid = int.Parse(item.properties.Gid);
                //    item.properties.Gid = (gid + 250).ToString();
                //}
                //richTextBox1.Text = JsonConvert.SerializeObject(roadPoints);

                RendPolylinesAndPoints();
            }
        }

        private void RendPolylinesAndPoints()
        {
            double count = 0;
            foreach (var item in roadPoints.features)
            {
                mapOperation.CreateRenderPolylines(item, dic);
                LoadLabel(item);

                count = item.geometry.coordinates.Count;
                totalCount += item.geometry.coordinates.Count;
            }

            UpdateLabelCount(count, totalCount);
            LoadFormData();
        }

        private void LoadLabel(Feature feature)
        {
            if (feature == null) return;
            var pointCount = feature.geometry.coordinates.Count;
            var position = feature.geometry.coordinates[pointCount > 1 ? pointCount / 2 - 1 : 0];
            if (position != null)
            {
                string showText = $"Gid={feature.properties.Gid}, source={feature.properties.Source}, target={feature.properties.Target}";
                mapOperation.CreateLabel(position[0], position[1], 5, showText, feature.Id);
            }
        }

        private void LoadFormData()
        {
            if (roadPoints.features.Count == 0)
                return;
            var feature = roadPoints.features[roadPoints.features.Count - 1];
            if (feature != null)
            {
                double id = double.Parse(feature.properties.Gid);
                UpdateFormView(id + 1, feature.properties.Source + 1, feature.properties.Target + 1);

                gid = double.Parse(labelID.Text);
                source = double.Parse(txtSource.Text);
                target = double.Parse(txtTarget.Text);
            }
        }

        IRenderPolyline selectedPolyline;
        IRenderPoint selectedPoint;
        IRenderPolyline prePolyline;
        IRenderPoint prePoint;
        private void AxRenderControl1_RcMouseClickSelect(object sender, Axi3dRenderEngine._IRenderControlEvents_RcMouseClickSelectEvent e)
        {
            UnHightlightObj();
            ResetSelectedObj();

            var pickResult = e.pickResult;
            if (pickResult.Type == i3dObjectType.i3dObjectRenderPolyline)
            {
                var flpr = pickResult as IRenderPolylinePickResult;
                selectedPolyline = flpr.Polyline;
                selectedPolyline.Highlight(0xFFFF0000);
                prePolyline = selectedPolyline;
            }
            else if (pickResult.Type == i3dObjectType.i3dObjectRenderPoint)
            {
                var res = pickResult as IRenderPointPickResult;
                selectedPoint = res.Point;
                selectedPoint.Highlight(0xFFFF0000);
                prePoint = selectedPoint;
            }
        }

        private void UnHightlightObj()
        {
            prePolyline?.Unhighlight();
            prePoint?.Unhighlight();
        }

        private void ResetSelectedObj()
        {
            selectedPoint = null;
            selectedPolyline = null;
        }

        private void AxRenderControl1_RcObjectEditFinish(object sender, EventArgs e)
        {
            try
            {
                if (polyline != null)
                {
                    sb.Clear();
                    var count = polyline.PointCount;
                    totalCount += count;

                    if (count < 2)
                        return;

                    UpdateLabelCount(count, totalCount);
                    UpdateStartPointAndEndPointValue();

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
                        // 添加到RichTextBox显示
                        geometry = new Geometry() { coordinates = new List<double[]>() };
                        points.ForEach(m => geometry.coordinates.Add(new double[] { m.X, m.Y }));

                        source = checkBox1.Checked ? double.Parse(txtSource.Text.Trim()) : source;
                        target = checkBox1.Checked ? double.Parse(txtTarget.Text.Trim()) : target;
                        Property property = new Property()
                        {
                            Gid = gid.ToString(),
                            Class_Id = "DL" + class_id.ToString(),
                            Source = source,
                            Target = target,
                            Oneway = comboBox.SelectedIndex == 0,
                            Priority = float.Parse(txtPriority.Text.Trim()),
                            MaxForwardSpeed = float.Parse(txtMaxForwardSpeed.Text.Trim()),
                            MaxBackwardSpeed = float.Parse(txtMaxBackwardSpeed.Text.Trim())
                        };

                        string showTxt = $"Gid={gid}, source={source}, target={target}";

                        var midPoint = polyline.Midpoint;
                        if (midPoint != null)
                            mapOperation.CreateLabel(midPoint.X, midPoint.Y, 5, showTxt, mapOperation.RenderPolyline.Guid);
                        else
                            mapOperation.CreateLabel(points[0].X, points[0].Y, 5, showTxt, mapOperation.RenderPolyline.Guid);

                        gid++;
                        class_id++;
                        source = target;
                        target++;

                        UpdateFormView(gid, source, target);

                        roadPoints.features.Add(new Feature() { Id = mapOperation.RenderPolyline.Guid, properties = property, geometry = geometry });
                        ShowInRichTextBox();

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

        private void AddDataToDic()
        {
            dic.Add(mapOperation.RenderPolyline.Guid, sb.ToString().Substring(0, sb.Length - 1));
        }

        private void ShowInRichTextBox()
        {
            string str = JsonConvert.SerializeObject(roadPoints);
            richTextBox1.Text = str;
        }

        private void UpdateFormView(double Id, double source, double target)
        {
            labelID.Text = Id.ToString();
            txtSource.Text = source.ToString();
            txtTarget.Text = target.ToString();
        }

        private void UpdateLabelCount(double count, double totalCount)
        {
            labelPointCount.Text = count < 0 ? "0" : count.ToString();
            labelTotalPointCount.Text = totalCount < 0 ? "0" : totalCount.ToString();
        }

        private void UpdateStartPointAndEndPointValue()
        {
            var sPoint = polyline.StartPoint;
            var ePoint = polyline.EndPoint;
            if(sPoint!=null && ePoint != null)
            {
                labelStartPointX.Text = sPoint.X.ToString();
                labelStartPointY.Text = sPoint.Y.ToString();

                labelEndPointX.Text = ePoint.X.ToString();
                labelEndPointY.Text = ePoint.Y.ToString();
            }
        }

        private void CreateRenderPoint(IPoint point)
        {
            var rPoint = axRenderControl1.ObjectManager.CreateRenderPoint(point, symbol);
            //rPoint.ViewingDistance = 50;
            rPoint.MaxVisibleDistance = 121313;
            //axRenderControl1.Camera.FlyToObject(rPoint.Guid, i3dActionCode.i3dActionFollowBehindAndAbove);
            sb.Append($"{rPoint.Guid},");
        }

        IPoint startPoint;
        IPoint endPoint;
        int i = 0;
        private readonly double xOffset = 0.00001141;
        private readonly double yOffset = 0.00000899;
        double offSetDis;

        // 经度（东西方向）1M实际度：360°/ 31544206M = 1.141255544679108e-5 = 0.00001141
        // 纬度（南北方向）1M实际度：360°/ 40030173M = 8.993216192195822e-6 = 0.00000899
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
                Debug.WriteLine($"startPoint: {startPoint.X}, {startPoint.Y}; endPoint: {endPoint.X}, {endPoint.Y}");
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
                x = (offSetDis * i * (endPoint.X - startPoint.X) / r) + startPoint.X;
                y = (offSetDis * i * (endPoint.Y - startPoint.Y) / r) + startPoint.Y;

                //Debug.WriteLine($"Point: {x}, {y}, OffSetDis: {offSetDis}");
                temPont.SetCoords(x, y, temPont.Z, temPont.M, temPont.Id);
                //temPont.SetCoords(x, y, 5, temPont.M, temPont.Id);
                polyline.AppendPoint(temPont);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                if (MessageBox.Show("确定退出系统吗", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Close();
                    Process.GetCurrentProcess().Kill();
                }
            }
            else if (keyData == Keys.F1)
            {
                mapOperation.InitlizedCameraPosition();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // 漫游
        private void button1_Click(object sender, EventArgs e)
        {
            SetInteractNormal();
        }

        private void SetInteractNormal()
        {
            axRenderControl1.InteractMode = i3dInteractMode.i3dInteractNormal;
            UnHightlightObj();
            selectedPoint = null;
            selectedPolyline = null;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtSource.ReadOnly = !checkBox1.Checked;
            txtTarget.ReadOnly = !checkBox1.Checked;
        }

        // 坐标
        private void button2_Click(object sender, EventArgs e)
        {
            axRenderControl1.InteractMode = i3dInteractMode.i3dInteractMeasurement;
            axRenderControl1.MeasurementMode = i3dMeasurementMode.i3dMeasureCoordinate;
        }

        // 画线
        private void button3_Click(object sender, EventArgs e)
        {
            mapOperation.CreatePolyline();
        }

        // 保存
        private void button4_Click(object sender, EventArgs e)
        {
            Save();
            MessageBox.Show($"保存成功，位置在{savePath}");
        }

        private void Save()
        {
            File.WriteAllText(savePath, richTextBox1.Text.Trim());
        }

        // 选取
        private void button5_Click(object sender, EventArgs e)
        {
            var selectMode = i3dMouseSelectMode.i3dMouseSelectClick;
            SetInteractMode(i3dInteractMode.i3dInteractSelect, selectMode);
        }

        private void SetInteractMode(i3dInteractMode interactMode, i3dMouseSelectMode mouseSelectMode = i3dMouseSelectMode.i3dMouseSelectClick,
            i3dMouseSelectObjectMask mouseSelectObjectMask = i3dMouseSelectObjectMask.i3dSelectAll)
        {
            axRenderControl1.InteractMode = interactMode;
            axRenderControl1.MouseSelectMode = mouseSelectMode;
            axRenderControl1.MouseSelectObjectMask = mouseSelectObjectMask;
        }

        // 删除
        private void button6_Click(object sender, EventArgs e)
        {
            // 删除点
            if (selectedPoint != null)
            {
                IPoint point = selectedPoint.GetFdeGeometry() as IPoint;
                foreach (var item in roadPoints.features)
                {
                    for (int i = 0; i < item.geometry.coordinates.Count; i++)
                    {
                        var coordinate = item.geometry.coordinates[i];
                        if (coordinate[0] == point.X && coordinate[1] == point.Y)
                        {
                            item.geometry.coordinates.RemoveAt(i);
                            break;
                        }
                    }
                }

                axRenderControl1.ObjectManager.DeleteObject(selectedPoint.Guid);

                int count = int.Parse(labelPointCount.Text) - 1;
                int totalCount = int.Parse(labelTotalPointCount.Text) - 1;
                UpdateLabelCount(count, totalCount);
            }
            if (selectedPolyline != null)       // 删除线
            {
                for (int i = 0; i < roadPoints.features.Count; i++)
                {
                    var feature = roadPoints.features[i];
                    if (feature.Id == selectedPolyline.Guid)
                    {
                        roadPoints.features.RemoveAt(i);

                        var delCount = feature.geometry.coordinates.Count;
                        int count = int.Parse(labelPointCount.Text) - delCount;
                        int totalCount = int.Parse(labelTotalPointCount.Text) - delCount;
                        UpdateLabelCount(count, totalCount);
                        break;
                    }
                }

                // todo: 删除界面所有对应的RenderPoint
                var temp = dic.FirstOrDefault(m => m.Key == selectedPolyline.Guid);
                dic.Remove(temp.Key);

                string[] guids = temp.Value.Split(',');
                foreach (var item in guids)
                {
                    axRenderControl1.ObjectManager.DeleteObject(Guid.Parse(item));
                }

                // 删除label
                var labels = mapOperation.RenderLabels;
                var label = labels.FirstOrDefault(m => m.ClientData == selectedPolyline.Guid.ToString());
                if (label != null)
                    axRenderControl1.ObjectManager.DeleteObject(label.Guid);

                axRenderControl1.ObjectManager.DeleteObject(selectedPolyline.Guid);
            }

            ShowInRichTextBox();
        }
    }
}
