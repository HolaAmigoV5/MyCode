using Axi3dRenderEngine;
using DaJuTestDemo.Core;
using i3dCommon;
using i3dFdeCore;
using i3dFdeGeometry;
using i3dMath;
using i3dRenderEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms.Integration;

namespace I3DMapOperation
{
    public class MapOperation : IMapOperation
    {
        /// <summary>
        /// 投影坐标系
        /// </summary>
        public const string WKT = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137.0,298.257223563]]," +
            "PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433],AUTHORITY[\"EPSG\",4326]]";

        private readonly AxRenderControl _axRenderControl = null;
        private readonly IGeometryFactory _geoFactory = null;
        private readonly Dictionary<IFeatureClass, List<string>> _featureClassMapping;
        private IList<FeatureLayerMap> _featureLayerMaps;
        private IDataSourceFactory dsFactory;
        private IPoint px = null;
        string currentDir = Environment.CurrentDirectory;
        ConnectionInfo _ci;
        ISpatialCRS _spatialCRS;

        StringBuilder sb;

        //public event Action<(string, NotationType)> SelectNotationFinished;
        public MapOperation()
        {
            _axRenderControl = new AxRenderControl();
            _featureClassMapping = new Dictionary<IFeatureClass, List<string>>();
            _featureLayerMaps = new List<FeatureLayerMap>();

            _spatialCRS = new CRSFactory().CreateFromWKT(WKT) as ISpatialCRS;

            _geoFactory = new GeometryFactoryClass();
            px = _geoFactory.CreatePoint(i3dVertexAttribute.i3dVertexAttributeZ);
            px.SpatialCRS = _spatialCRS;
            sb = new StringBuilder();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="i3dmPath"></param>
        /// <returns></returns>
        public void InitializationAxRenderControl(WindowsFormsHost host)
        {
            // 初始化RenderControl控件
            InitializeRenderControl(host);

            // 设置默认天空盒
            SetDefaultSkyBox();

            // 获取连接信息
            GetConnectionInfo();

            FeatureDataSetMapToFeatureClassMap();

            // 创建图层
            CreateFeautureLayer();

            // 设置摄像机初始位置
            InitlizedCameraPosition();

            CreateCarModelPoint();
        }

        /// <summary>
        /// 设置飞行时间
        /// </summary>
        /// <param name="flyTime"></param>
        public void SetFlyTime(double flyTime)
        {
            _axRenderControl.Camera.FlyTime = flyTime;
        }

        /// <summary>
        /// 获取相机位置
        /// </summary>
        /// <returns></returns>
        public string GetCameraPosition()
        {
            IVector3 vector3;
            IEulerAngle angle;
            _axRenderControl.Camera.GetCamera(out vector3, out angle);
            string res = $"Vector.X={vector3.X}，Y={vector3.Y}, Z={vector3.Z}; Angle.Heading={angle.Heading},Tilt={angle.Tilt}, Roll={angle.Roll}";
            return res;
        }

        public (double X, double Y, double Z) GetCameraPositionVector()
        {
            IVector3 vector3;
            IEulerAngle angle;

            _axRenderControl.Camera.GetCamera(out vector3, out angle);
            return (vector3.X, vector3.Y, vector3.Z);
        }

        public void InitlizedCameraPosition()
        {
            SetCameraValues(121.29538604418703, 31.16980507423275, 3847.716093963012, heading: 340.2329560679867, tilt: -72.7, isLookAt: false);
        }

        public void SetCameraValues(double x, double y, double z, double heading = 0, 
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

        /// <summary>
        /// 选取模式
        /// </summary>
        public void SetSelectMode(string modeName)
        {
            switch (modeName)
            {
                case "漫游":
                    SetInteractMode(i3dInteractMode.i3dInteractNormal);
                    break;
                case "仅点选":
                    SetInteractMode(i3dInteractMode.i3dInteractSelect);
                    break;
                case "仅框选":
                    SetInteractMode(i3dInteractMode.i3dInteractSelect, i3dMouseSelectMode.i3dMouseSelectDrag);
                    break;
                case "点选+框选":
                    {
                        var selectMode = i3dMouseSelectMode.i3dMouseSelectClick | i3dMouseSelectMode.i3dMouseSelectDrag;
                        SetInteractMode(i3dInteractMode.i3dInteractSelect, selectMode);
                    }
                    break;
                default:
                    SetInteractMode(i3dInteractMode.i3dInteractNormal);
                    break;
            }
        }

        private void SetInteractMode(i3dInteractMode interactMode, i3dMouseSelectMode mouseSelectMode = i3dMouseSelectMode.i3dMouseSelectClick)
        {
            _axRenderControl.InteractMode = interactMode;
            _axRenderControl.MouseSelectMode = mouseSelectMode;
        }

        /// <summary>
        /// 注册控件拾取事件
        /// </summary>
        public void RegisterRcSelectEvent()
        {
            _axRenderControl.RcMouseClickSelect += axRenderControl1_RcMouseClickSelect;
            _axRenderControl.MouseSelectObjectMask = i3dMouseSelectObjectMask.i3dSelectRenderGeometry;
        }

        /// <summary>
        /// 取消控件拾取事件
        /// </summary>
        public void CancelRcSelectEvent()
        {
            _axRenderControl.RcMouseClickSelect -= axRenderControl1_RcMouseClickSelect;
        }

        private void axRenderControl1_RcMouseClickSelect(object sender, _IRenderControlEvents_RcMouseClickSelectEvent e)
        {
            try
            {
                if (e.eventSender != i3dMouseSelectMode.i3dMouseSelectClick)
                    return;
                if (e.pickResult != null)
                {

                    string id = string.Empty;
                    switch (e.pickResult)
                    {
                        case IRenderPointPickResult renderPoint:
                            id = renderPoint.Point.ClientData;
                            break;
                        case IRenderModelPointPickResult modelPoint:
                            id = modelPoint.ModelPoint.ClientData;
                            break;
                    }

                    if (string.IsNullOrEmpty(id))
                        return;

                    //var type = (NotationType)GetNotationById(id).Type;
                    //SelectNotationFinished?.Invoke((id, type));

                    var angle = SetAngle(0, -50, 0);
                    SetCamera(e.intersectPoint.Position, angle, true, 100);
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error(ex, "执行axRenderControl1_RcMouseClickSelect错误");
                throw ex;
            }
        }

        // 创建车模型
        private void CreateCarModelPoint()
        {
            string carImgPath = Path.Combine(Environment.CurrentDirectory, @"data\TrashCar\SSCMX01.osg");
            try
            {
                IModelPoint mp = (IModelPoint)_geoFactory.CreateGeometry(i3dGeometryType.i3dGeometryModelPoint, i3dVertexAttribute.i3dVertexAttributeZ);
                mp.ModelName = carImgPath;
                mp.SpatialCRS = _spatialCRS;
                mp.SetCoords(121.27170, 31.17965, 1.5, 0, 0);
                CarRmp = _axRenderControl.ObjectManager.CreateRenderModelPoint(mp, null);
                CarRmp.MaxVisibleDistance = 15000;
                CarRmp.MinVisiblePixels = 10;
                CarRmp.ClientData = "111";
            }
            catch (Exception e)
            {
                LoggerHelper.Logger.Error(e, "执行SetCarModelPoint错误");
                throw e;
            }
        }

        #region 轨迹相关
        IVector3 position1 = new Vector3() { X = 0, Y = 0, Z = 0 };
        IRenderModelPoint CarRmp;
        private IDynamicObject traceDynamicObj;
        private IRenderPolyline tracePolyline;
        private ISimplePointSymbol symbol;
        /// <summary>
        /// 轨迹回放
        /// </summary>
        /// <param name="traceInfo">轨迹数据</param>
        /// <param name="speedTimes">回放倍数</param>
        /// <param name="traceChanged">轨迹是否改变</param>
        public void RenderVehicleTrajectory(IList<Trajectory> traceInfos, int speedTimes, bool traceChanged = true)
        {
            try
            {
                if (traceChanged)
                {
                    if (traceDynamicObj == null)
                    {
                        traceDynamicObj = _axRenderControl.ObjectManager.CreateDynamicObject();
                        traceDynamicObj.AutoRepeat = false;
                        traceDynamicObj.CrsWKT = WKT;
                        traceDynamicObj.TurnSpeed = 300;
                        AppendGuidToStringBuilder(traceDynamicObj.Guid);

                        symbol = new SimplePointSymbolClass()
                        {
                            FillColor = 0xff0000ff,
                            Size = 10
                        };

                        #region 创建线
                        IPoint point = _geoFactory.CreatePoint(i3dVertexAttribute.i3dVertexAttributeZ);
                        IPolyline line = (IPolyline)_geoFactory.CreateGeometry(i3dGeometryType.i3dGeometryPolyline, i3dVertexAttribute.i3dVertexAttributeZ);
                        line.SpatialCRS = _spatialCRS;
                        

                        foreach (Trajectory trajectory in traceInfos)
                        {
                            IVector3 position = new Vector3();
                            position.Set(trajectory.Longitude, trajectory.Latitude, 1);
                            point.Position = position;
                            point.SpatialCRS = _spatialCRS;

                            line.AppendPoint(point);
                            traceDynamicObj.AddWaypoint2(point, Convert.ToDouble(30 * speedTimes));
                            
                            var rPoint= _axRenderControl.ObjectManager.CreateRenderPoint(point, symbol);
                            rPoint.MaxVisibleDistance = 12500;
                            rPoint.ViewingDistance = 50;

                            AppendGuidToStringBuilder(rPoint.Guid);
                            _axRenderControl.Camera.FlyToObject(rPoint.Guid, i3dActionCode.i3dActionFollowBehindAndAbove);
                        }
                        tracePolyline = _axRenderControl.ObjectManager.CreateRenderPolyline(line, new CurveSymbol { Color = 0xFFFFFF00, Width = 0.5f });
                        tracePolyline.VisibleMask = i3dViewportMask.i3dViewAllNormalView;
                        tracePolyline.MaxVisibleDistance = 12500;
                        tracePolyline.HeightStyle = i3dHeightStyle.i3dHeightAbsolute;

                        AppendGuidToStringBuilder(tracePolyline.Guid);
                        #endregion
                    }
                }
                else
                {
                    if (traceDynamicObj == null)
                        return;

                    traceDynamicObj.Stop();
                    traceDynamicObj.TurnSpeed = 300 * speedTimes;
                    for (int i = 0; i < traceInfos.Count; i++)
                    {
                        traceDynamicObj.GetWaypoint2(i, out IPoint point, out double speed);
                        traceDynamicObj.ModifyWaypoint2(i, point, (double)traceInfos[i].Speed * speedTimes);
                    }
                }

                TraceObjectsPlay("111", traceDynamicObj, true);
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error(ex, $"执行RenderVehicleTrajectory错误");
                throw ex;
            }
        }

        public void RenderTrajectory(List<Trajectory> traceInfos, uint pointColor = 0xAA90EE90, uint lineColor = 0xAA0CE6E5)
        {
            try
            {
                symbol = new SimplePointSymbolClass()
                {
                    FillColor = pointColor,
                    Size = 10
                };

                #region 创建线
                IPoint point = _geoFactory.CreatePoint(i3dVertexAttribute.i3dVertexAttributeZ);
                IPolyline line = (IPolyline)_geoFactory.CreateGeometry(i3dGeometryType.i3dGeometryPolyline, i3dVertexAttribute.i3dVertexAttributeZ);
                line.SpatialCRS = _spatialCRS;


                foreach (Trajectory trajectory in traceInfos)
                {
                    point.SetCoords(trajectory.Longitude, trajectory.Latitude, 1, 0, 1);
                    point.SpatialCRS = _spatialCRS;

                    line.AppendPoint(point);
                    var rPoint = _axRenderControl.ObjectManager.CreateRenderPoint(point, symbol);
                    rPoint.MaxVisibleDistance = 1000;
                    rPoint.ViewingDistance = 50;
                    AppendGuidToStringBuilder(rPoint.Guid);
                }
                tracePolyline = _axRenderControl.ObjectManager.CreateRenderPolyline(line, new CurveSymbol { Color = lineColor, Width = -2 });
                tracePolyline.VisibleMask = i3dViewportMask.i3dViewAllNormalView;
                tracePolyline.MaxVisibleDistance = 1000;
                tracePolyline.HeightStyle = i3dHeightStyle.i3dHeightAbsolute;

                AppendGuidToStringBuilder(tracePolyline.Guid);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error(ex, $"执行RenderTrajectory错误");
                throw ex;
            }
        }

        private void TraceObjectsPlay(string objID, IDynamicObject dObj, bool setFollowing)
        {
            if (CarRmp is IMotionable cMotion)
                cMotion.Bind2(dObj, position1, 0, 0, 0);
            if (setFollowing)
                SetFollowingPerspective(i3dActionCode.i3dActionFollowAbove, CarRmp);
            dObj.Play();
        }

        public void StopVehicleTrajectory()
        {
            if (traceDynamicObj != null)
            {
                traceDynamicObj.Stop();
            }
        }

        public void PlayOrStopVehicleTrajectory(bool play)
        {
            if (traceDynamicObj != null)
            {
                if (play)
                {
                    traceDynamicObj.Play();
                    SetFollowingPerspective(i3dActionCode.i3dActionFollowBehindAndAbove, CarRmp);
                }  
                else
                    traceDynamicObj.Stop();
            }
        }

        public void MeasureDistance()
        {
            _axRenderControl.InteractMode = i3dInteractMode.i3dInteractMeasurement;
            _axRenderControl.MeasurementMode = i3dMeasurementMode.i3dMeasureAerialDistance;
        }

        public void Normal()
        {
            _axRenderControl.InteractMode = i3dInteractMode.i3dInteractNormal;
        }

        /// <summary>
        /// 设置相机飞到或跟随模型的方式
        /// </summary>
        /// <param name="actionCode">方式</param>
        /// <param name="modelPoint">模型</param>
        private void SetFollowingPerspective(i3dActionCode actionCode, IRenderGeometry modelPoint)
        {
            if (modelPoint != null)
            {
                switch (actionCode)
                {
                    case i3dActionCode.i3dActionFollowAbove: // 上帝视角
                        modelPoint.ViewingDistance = 500;
                        break;
                    case i3dActionCode.i3dActionFollowBehindAndAbove:  // 第三人称
                        modelPoint.ViewingDistance = 50;
                        break;
                    default:
                        modelPoint.ViewingDistance = 100;
                        break;
                }
                _axRenderControl.Camera.FlyToObject(modelPoint.Guid, actionCode);
            }
        }

        public void ClearAllRenderObj()
        {
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                ResetDynamicObj();

                string str = sb.ToString().Substring(0, sb.Length - 1);
                if (!string.IsNullOrEmpty(str))
                {
                    string[] guids = str.Split(',');
                    foreach (string guid in guids)
                    {
                        _axRenderControl.ObjectManager.DeleteObject(Guid.Parse(guid));
                    }
                }
            }
        }

        private void ResetDynamicObj()
        {
            if (traceDynamicObj != null)
            {
                traceDynamicObj.Stop();
                traceDynamicObj.ClearWaypoints();
                traceDynamicObj = null;
            }
        }

        private void AppendGuidToStringBuilder(Guid guid)
        {
            sb.Append($"{guid},");
        }

        #endregion

        #region 初始化
        /// <summary>
        ///  初始化RenderControl控件
        /// </summary>
        private void InitializeRenderControl(WindowsFormsHost host)
        {
            _axRenderControl.BeginInit();
            host.Child = _axRenderControl;
            _axRenderControl.EndInit();

            PropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", i3dRenderSystem.i3dRenderOpenGL);

            //初始化三维窗口。isPlanarTerrain（true:平面地形，false：地球形）, params（配置参数）
            _axRenderControl.Initialize(false, ps);

            SetFlyTime(3);
        }

        /// <summary>
        /// 设置默认天空盒
        /// </summary>
        private void SetDefaultSkyBox()
        {
            //SetSkyBox(SkyBoxType.CKPR);
        }

        private void SetSkyBox(SkyBoxType sky)
        {
            //try
            //{
            //    string skyVal = ((int)sky).ToString();
            //    if (skyVal == "0" || skyVal == "4")
            //        skyVal = "0" + skyVal;
            //    string _skyBoxPath = Path.Combine(currentDir, GlobalCaches.SkyBoxPath ?? @"Images\skybox");

            //    // 获取天空盒
            //    ISkyBox _skyBox = _axRenderControl.ObjectManager.GetSkyBox(0);
            //    _skyBox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBack, Path.Combine(_skyBoxPath, skyVal + "_BK.jpg"));
            //    _skyBox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBottom, Path.Combine(_skyBoxPath, skyVal + "_DN.jpg"));
            //    _skyBox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageFront, Path.Combine(_skyBoxPath, skyVal + "_FR.jpg"));
            //    _skyBox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageLeft, Path.Combine(_skyBoxPath, skyVal + "_LF.jpg"));
            //    _skyBox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageRight, Path.Combine(_skyBoxPath, skyVal + "_RT.jpg"));
            //    _skyBox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageTop, Path.Combine(_skyBoxPath, skyVal + "_UP.jpg"));
            //}
            //catch (Exception e)
            //{
            //    LoggerHelper.Logger.Error(e, "执行SetSkyBox错误");
            //    throw e;
            //}
        }

        /// <summary>
        /// 获取连接信息
        /// </summary>
        /// <param name="i3dmFilePath"></param>
        /// <param name="localData"></param>
        private void GetConnectionInfo(bool localData = true)
        {
            _ci = new ConnectionInfo();
            if (localData)
            {
                // 加载本地数据
                _ci.ConnectionType = i3dConnectionType.i3dConnectionFireBird2x;
                string i3dmFilePath = Path.Combine(currentDir, @"data\XJJD.3DM");
                _ci.Database = i3dmFilePath;
            }
            else
            {
                // 加载服务数据
                _ci.ConnectionType = i3dConnectionType.i3dConnectionCms7Http;
                _ci.Server = "192.168.2.90";
                _ci.Port = 8040;
                _ci.Database = "point";
            }
        }

        private void FeatureDataSetMapToFeatureClassMap()
        {
            try
            {
                if (_ci == null) return;
                dsFactory = new DataSourceFactory();
                IDataSource ds = dsFactory.OpenDataSource(_ci);

                string[] setnames = (string[])ds.GetFeatureDatasetNames();
                if (setnames == null || setnames.Length == 0) return;

                foreach (string name in setnames)
                {
                    IFeatureDataSet dataSet = ds.OpenFeatureDataset(name);
                    string[] fcnames = (string[])dataSet.GetNamesByType(i3dDataSetType.i3dDataSetFeatureClassTable);
                    if (fcnames == null || fcnames.Length == 0)
                        continue;
                    foreach (string fcname in fcnames)
                    {
                        IFeatureClass fc = dataSet.OpenFeatureClass(fcname);
                        if (fc != null)
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
                                _featureClassMapping.Add(fc, geoNames);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error(ex, "执行FeatureDataSetMapToFeatureClassMap错误");
            }
        }

        private void CreateFeautureLayer()
        {
            bool hasfly = false;
            foreach (FeatureClass fc in _featureClassMapping.Keys)
            {
                List<string> geoNames = _featureClassMapping[fc];
                foreach (string geoName in geoNames)
                {
                    if (!geoName.Equals("Geometry"))
                        continue;
                    IFeatureLayer featureLayer = _axRenderControl.ObjectManager.CreateFeatureLayer(fc, geoName, null, null);
                    featureLayer.MaxVisibleDistance = 500000000;
                    _featureLayerMaps.Add(new FeatureLayerMap(featureLayer, fc));
                    if (!hasfly)
                    {
                        IFieldInfoCollection fieldinfos = fc.GetFields();
                        IFieldInfo fieldinfo = fieldinfos.Get(fieldinfos.IndexOf(geoName));
                        IGeometryDef geometryDef = fieldinfo.GeometryDef;
                        IEnvelope env = geometryDef.Envelope;
                        if (env == null || (env.MaxX == 0.0 && env.MaxY == 0.0 && env.MaxZ == 0.0 && env.MinX == 0.0 && env.MinY == 0.0 && env.MinZ == 0.0))
                            continue;
                        //var angle = SetAngle(0, -20, 0);
                        //SetCameraLookAt(env.Center, 1000, angle);
                    }
                    hasfly = true;
                }
            }
        }

        #endregion
    }
}
