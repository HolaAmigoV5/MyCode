using Axi3dRenderEngine;
using i3dCommon;
using i3dFdeCore;
using i3dFdeGeometry;
using i3dMath;
using i3dRenderEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DrawPointToolForMeiDu
{
    public class MapOperation
    {
        /// <summary>
        /// 投影坐标系
        /// </summary>
        public const string WKT = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137.0,298.257223563]]," +
            "PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433],AUTHORITY[\"EPSG\",4326]]";

        private string _i3dmPath;
        private AxRenderControl _axRenderControl = null;
        ConnectionInfo ci;
        ISpatialCRS _spatialCRS;
        private IGeometryFactory gfactory = null;

        public bool DrayPoint { get; set; } = false;

        public MapOperation(AxRenderControl axRenderControl, string i3dmPath)
        {
            _axRenderControl = axRenderControl;
            _i3dmPath = i3dmPath;
            _spatialCRS = new CRSFactory().CreateFromWKT(WKT) as ISpatialCRS;
            gfactory = new GeometryFactory();

            textAttribute = new TextAttribute()
            {
                TextColor = 0xffffff00,
                TextSize = 12,
                Underline = true,
                Font = "楷体"
            };

            textSymbol = new TextSymbol()
            {
                TextAttribute = textAttribute,
                VerticalOffset = 10,
                DrawLine = true,
                MarginColor = 0x8800ffff
            };
        }

        public void InitializationAxRenderControl()
        {
            // 初始化RenderControl控件
            InitializeRenderControl();

            // 加载数据
            LoadData(_i3dmPath);

            // 图层创建
            FeatureLayerVisualize();
            CreateFeautureLayer();

            InitlizedCameraPosition();
        }

        public void InitlizedCameraPosition()
        {
            SetCameraValues(121.29538604418703, 31.16980507423275, 3847.716093963012, heading: 340.2329560679867, tilt: -72.7, isLookAt: false);
        }

        #region Private Method
        private void InitializeRenderControl()
        {
            PropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", i3dRenderSystem.i3dRenderOpenGL);

            //初始化三维窗口。isPlanarTerrain（true:平面地形，false：地球形）, params（配置参数）
            _axRenderControl.Initialize(false, ps);

            _axRenderControl.Camera.FlyTime = 3;
        }

        private void SetCameraValues(double x, double y, double z, double heading = 0,
            double tilt = -60, double roll = 0, double distance = 2000, bool isLookAt = true)
        {
            IVector3 position = new Vector3() { X = x, Y = y, Z = z };
            IEulerAngle angle = SetAngle(heading, tilt, roll);
            SetCamera(position, angle, isLookAt, distance);
        }

        private IEulerAngle SetAngle(double heading, double tilt, double roll)
        {
            IEulerAngle angle = new EulerAngle();
            angle.Set(heading, tilt, roll);
            return angle;
        }

        private void SetCamera(IVector3 position, IEulerAngle angle, bool isLookAt, double distance)
        {
            if (isLookAt)
            {
                _axRenderControl.Camera.LookAt(position, distance, angle);
            }
            else
            {
                _axRenderControl.Camera.SetCamera(position, angle, i3dSetCameraFlags.i3dSetCameraNoFlags);
            }
        }

        private void FeatureLayerVisualize()
        {
            FeatureDataSetMapToFeatureClass();
            FeatureClassMapToFieldInfor();
            //FieldinfoMapToGeometryDef();
        }

        public void LoadData(string i3dmFilePath)
        {
            // 加载本地数据
            ci = new ConnectionInfo
            {
                ConnectionType = i3dConnectionType.i3dConnectionFireBird2x,
                Database = i3dmFilePath
            };
        }

        private IDataSourceFactory dsFactory = null;
        private Dictionary<IFeatureDataSet, List<IFeatureClass>> featureDataSetMapping;
        private List<IFeatureClass> featureClasses;
        private void FeatureDataSetMapToFeatureClass()
        {
            try
            {
                if (ci == null) return;
                dsFactory = new DataSourceFactory();
                IDataSource ds = dsFactory.OpenDataSource(ci);
                
                string[] setnames = (string[])ds.GetFeatureDatasetNames();
                if (setnames == null || setnames.Length == 0) return;

                featureDataSetMapping = new Dictionary<IFeatureDataSet, List<IFeatureClass>>(setnames.Length);
                foreach (string name in setnames)
                {
                    IFeatureDataSet dataSet = ds.OpenFeatureDataset(name);
                    string[] fcnames = (string[])dataSet.GetNamesByType(i3dDataSetType.i3dDataSetFeatureClassTable);
                    if (fcnames == null || fcnames.Length == 0)
                        continue;
                    featureClasses = new List<IFeatureClass>(fcnames.Length);
                    foreach (string fcname in fcnames)
                    {
                        IFeatureClass fc = dataSet.OpenFeatureClass(fcname);
                        if (fc != null)
                            featureClasses.Add(fc);
                    }
                    featureDataSetMapping.Add(dataSet, featureClasses);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Dictionary<IFeatureClass, List<string>> featureClassMapping;

        private List<IFieldInfoCollection> fieldInfoCollections;
        private void FeatureClassMapToFieldInfor()
        {
            try
            {
                featureClassMapping = new Dictionary<IFeatureClass, List<string>>();
                fieldInfoCollections = new List<IFieldInfoCollection>();
                foreach (IFeatureClass fc in featureClasses)
                {
                    List<string> geoNames = new List<string>();
                    IFieldInfoCollection fieldinfos = fc.GetFields();
                    if (fieldinfos != null && fieldinfos.Count > 0)
                    {
                        for (int i = 0; i < fieldinfos.Count; i++)
                        {
                            IFieldInfo field = fieldinfos.Get(i);
                            if (field != null && field.GeometryDef != null)
                                geoNames.Add(field.Name);
                        }
                        featureClassMapping.Add(fc, geoNames);
                        fieldInfoCollections.Add(fieldinfos);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Dictionary<IFieldInfo, IGeometryDef> fieldInfoMapping;
        private void FieldinfoMapToGeometryDef()
        {
            fieldInfoMapping = new Dictionary<IFieldInfo, IGeometryDef>();
            foreach (IFieldInfoCollection fieldinfos in fieldInfoCollections)
            {
                for (int i = 0; i < fieldinfos.Count; i++)
                {
                    IFieldInfo fieldinfo = fieldinfos.Get(i);
                    if (fieldinfo != null)
                    {
                        IGeometryDef geometryDef = fieldinfo.GeometryDef;
                        if (geometryDef != null)
                        {
                            fieldInfoMapping.Add(fieldinfo, geometryDef);
                        }
                    }
                }
            }
        }

        private void CreateFeautureLayer()
        {
            bool hasfly = false;
            foreach (FeatureClass fc in featureClassMapping.Keys)
            {
                List<string> geoNames = featureClassMapping[fc];
                foreach (string geoName in geoNames)
                {
                    if (!geoName.Equals("Geometry"))
                        continue;
                    IFeatureLayer featureLayer = _axRenderControl.ObjectManager.CreateFeatureLayer(fc, geoName, null, null);
                    if (!hasfly)
                    {
                        IFieldInfoCollection fieldinfos = fc.GetFields();
                        IFieldInfo fieldinfo = fieldinfos.Get(fieldinfos.IndexOf(geoName));
                        IGeometryDef geometryDef = fieldinfo.GeometryDef;
                        IEnvelope env = geometryDef.Envelope;
                        if (env == null || (env.MaxX == 0.0 && env.MaxY == 0.0 && env.MaxZ == 0.0 && env.MinX == 0.0 && env.MinY == 0.0 && env.MinZ == 0.0))
                            continue;

                        EulerAngle angle = new EulerAngle();
                        angle.Set(0, -20, 0);
                        _axRenderControl.Camera.LookAt(env.Center, 1000, angle);
                    }
                    hasfly = true;
                }
            }
        }

        CurveSymbolClass lineSymbol;

        private IRenderPolyline renderPolyline;
        public IRenderPolyline RenderPolyline
        {
            get => renderPolyline;
            set => renderPolyline = value;
        }

        public void CreatePolyline()
        {
            var line = (IPolyline)gfactory.CreateGeometry(i3dGeometryType.i3dGeometryPolyline, i3dVertexAttribute.i3dVertexAttributeZ);
            line.SpatialCRS = _spatialCRS;
            renderPolyline = _axRenderControl.ObjectManager.CreateRenderPolyline(line, lineSymbol ?? new CurveSymbolClass()
            {
                Color = 0xFFFFFF00,  // 黄色
                Width = 1
            });
            renderPolyline.MaxVisibleDistance = 10000;
            renderPolyline.HeightStyle = i3dHeightStyle.i3dHeightAbsolute;

            _axRenderControl.InteractMode = i3dInteractMode.i3dInteractEdit;
            _axRenderControl.ObjectEditor.StartEditRenderGeometry(renderPolyline, i3dGeoEditType.i3dGeoEditCreator);
        }

        public void CreateRenderPolylines(Feature feature, Dictionary<Guid, string> dic)
        {
            StringBuilder sb = new StringBuilder();

            var line = (IPolyline)gfactory.CreateGeometry(i3dGeometryType.i3dGeometryPolyline, i3dVertexAttribute.i3dVertexAttributeZ);
            line.SpatialCRS = _spatialCRS;
            ISimplePointSymbol symbol = new SimplePointSymbolClass() { FillColor = 0xAA0000FF, Size = 10 };

            IPoint point = gfactory.CreatePoint(i3dVertexAttribute.i3dVertexAttributeZ);

            foreach (var item in feature.geometry.coordinates)
            {
                point.SetCoords(item[0], item[1], 1, 0, 0);
                point.SpatialCRS = _spatialCRS;
                line.AppendPoint(point);

                var rPoint = _axRenderControl.ObjectManager.CreateRenderPoint(point, symbol);
                rPoint.MaxVisibleDistance = 12500;
                rPoint.ViewingDistance = 50;
                sb.Append($"{rPoint.Guid},");
            }

            var  rLine = _axRenderControl.ObjectManager.CreateRenderPolyline(line, lineSymbol ?? new CurveSymbolClass()
            {
                Color = 0xFFFFFF00,  // 黄色
                Width = 1
            });
            rLine.MaxVisibleDistance = 10000;
            rLine.HeightStyle = i3dHeightStyle.i3dHeightAbsolute;
            dic.Add(rLine.Guid, sb.ToString().Substring(0, sb.Length - 1));
            feature.Id = rLine.Guid;
        }

        private TextAttribute textAttribute;
        private TextSymbol textSymbol;
        private IPoint labelPoint;

        public void CreateLabel(double x, double y, double z, string labelTxt, Guid clientId)
        {
            ILabel label = _axRenderControl.ObjectManager.CreateLabel();

            if (labelPoint == null)
                labelPoint = gfactory.CreatePoint(i3dVertexAttribute.i3dVertexAttributeZ);

            labelPoint.SetCoords(x, y, 1, 0, 1);
            label.Text = labelTxt;
            label.Position = labelPoint;
            label.TextSymbol = textSymbol;
            label.ClientData = clientId.ToString();

            renderLabels.Add(label);
        }

        private List<ILabel> renderLabels = new List<ILabel>();
        public List<ILabel> RenderLabels
        {
            get => renderLabels;
            set => renderLabels = value;
        }
        #endregion
    }
}
