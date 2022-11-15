using Axi3dRenderEngine;
using i3dCommon;
using i3dFdeCore;
using i3dFdeGeometry;
using i3dMath;
using i3dRenderEngine;
using i3dResource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommonMapLib
{
    public class MapOperation
    {
        private AxRenderControl _axRenderControl;
        private ConnectionInfo _ci;
        private IPoint selectPoint;
        private GeometryFactory gfactory;
        private ISpatialCRS crs;
        ISimplePointSymbol symbol;
        IVector3 scale;

        private readonly string rootPath = Path.GetFullPath(@"../../../data/Model/");
        //private const string tmpFDBPath = @"D:\GitHub\MyCode\SkyvisonPracticeDemo\data\3dm\JD.3DM";
        //private const string carBPath = @"D:\GitHub\MyCode\SkyvisonPracticeDemo\data\TrashCar\WetRubbishVehicle.osg";
        private const string carBPath = @"D:\GitHub\MyCode\SkyvisonPracticeDemo\data\osg\WetRubbishVehicle.osg";
        private readonly string tmpSkyboxPath = @"C:\Program Files\LC\SkySceneryX64\skybox\";   //天空盒图片位置（SkyScenery安装位置）
        private const string WKT = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137.0,298.257223563]]," +
    "PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433],AUTHORITY[\"EPSG\",4326]]";

        private bool _CTRL = false;

        /// <summary>
        /// 标记拾取时是否支持“ctrl”键用于多选
        /// </summary>
        public bool CTRL
        {
            get { return _CTRL; }
            set { _CTRL = value; }
        }

        public void InitializationMapControl(AxRenderControl axRenderControl, string _3dmname = "JD.3DM")
        {
            _axRenderControl = axRenderControl;
            gfactory = new GeometryFactory();
            notationDic = new Dictionary<string, IDynamicObject>();
            posOffSet = new Vector3() { X = 0, Y = 0, Z = 1.8 };
            symbol = new SimplePointSymbolClass() { FillColor = 0xAA0000FF, Size = 10 };

            scale = new Vector3();//创建向量
            scale.Set(1, 1, 1);//设置模型比例尺

            // 初始化RenderControl控件
            InitializeRenderControl();

            // 设置天空盒
            SetSkyBox(SkyBoxType.BTXY);

            // 设置连接信息
            SetConnectionInfo(_3dmname);

            // 连接3DM并创建图层
            Open3DMAndCreateFeatureLayer();

            // 设置选择模式
            SetSelectModeAndRegisterEvent();

            SetI3dObjectType();

        }

        private void SetI3dObjectType()
        {
            crs = new CRSFactory().CreateFromWKT(WKT) as ISpatialCRS;
        }

        private void InitializeRenderControl()
        {
            PropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", i3dRenderSystem.i3dRenderOpenGL);

            //初始化三维窗口。isPlanarTerrain（true:平面地形，false：地球形）, params（配置参数）
            _axRenderControl.Initialize(false, ps);

            //_axRenderControl.Initialize(true, ps);

            _axRenderControl.Camera.FlyTime = 3;
        }

        private void SetConnectionInfo(string mapname)
        {
            _ci = new ConnectionInfo
            {
                ConnectionType = i3dConnectionType.i3dConnectionFireBird2x,
                Database = GetFullMapPath(mapname)
            };
        }

        private string GetFullMapPath(string mapname)
        {
            return Path.Combine(@"D:\GitHub\MyCode\SkyvisonPracticeDemo\data\3dm", mapname);
        }

        private void Open3DMAndCreateFeatureLayer()
        {
            try
            {
                var dsFactory = new DataSourceFactory();
                var ds = dsFactory.OpenDataSource(_ci);

                string[] setnames = (string[])ds.GetFeatureDatasetNames();
                if (setnames == null || setnames.Length == 0) return;

                foreach (string name in setnames)
                {
                    var dataSet = ds.OpenFeatureDataset(name);
                    var fcnames = (string[])dataSet.GetNamesByType(i3dDataSetType.i3dDataSetFeatureClassTable);
                    if (fcnames == null || fcnames.Length == 0) continue;

                    foreach (string fcname in fcnames)
                    {
                        ReadFieldAndDrawMap(dataSet, fcname);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ReadFieldAndDrawMap(IFeatureDataSet dataSet, string fcname)
        {
            IFeatureClass fc = dataSet.OpenFeatureClass(fcname);
            IFeatureLayer featureLayer = _axRenderControl.ObjectManager.CreateFeatureLayer(fc, "Geometry", null, null);
            SetLookAt(fc);
            featureLayer.MaxVisibleDistance = 50000;
        }

        private void SetLookAt(IFeatureClass fc)
        {
            IFieldInfoCollection fieldinfos = fc.GetFields();
            IFieldInfo fieldinfo = fieldinfos.Get(fieldinfos.IndexOf("Geometry"));
            IGeometryDef geometryDef = fieldinfo.GeometryDef;
            IEnvelope env = geometryDef.Envelope;
            if (env == null || (env.MaxX == 0.0 && env.MaxY == 0.0 && env.MaxZ == 0.0 && env.MinX == 0.0 && env.MinY == 0.0 && env.MinZ == 0.0))
                return;
            EulerAngle angle = new EulerAngle();
            angle.Set(0, -20, 0);
            _axRenderControl.Camera.LookAt(env.Center, 100, angle);
        }

        private void SetSelectModeAndRegisterEvent()
        {
            _axRenderControl.InteractMode = i3dInteractMode.i3dInteractSelect;
            _axRenderControl.MouseSelectObjectMask = i3dMouseSelectObjectMask.i3dSelectAll;
            _axRenderControl.MouseSelectMode = i3dMouseSelectMode.i3dMouseSelectClick;
            _axRenderControl.RcMouseClickSelect += _axRenderControl_RcMouseClickSelect; ;
        }

        private void _axRenderControl_RcMouseClickSelect(object sender, _IRenderControlEvents_RcMouseClickSelectEvent e)
        {
            FeatureLayerPick(e.pickResult, e.intersectPoint, e.eventSender);
        }

        private void FeatureLayerPick(IPickResult pickResult, IPoint position, i3dMouseSelectMode selectedModel)
        {
            UnhighlightFeature(pickResult);

            if (selectedModel == i3dMouseSelectMode.i3dMouseSelectClick)
            {
                selectPoint = position;
                //ShowAnimationOSG2("蝴蝶1.OSG");
            }
        }

        private void UnhighlightFeature(object obj, i3dModKeyMask modKeyMask = i3dModKeyMask.i3dModKeyDblClk)
        {
            if (obj == null && CTRL && modKeyMask == i3dModKeyMask.i3dModKeyCtrl)
                return;

            if (!CTRL || (CTRL && modKeyMask != i3dModKeyMask.i3dModKeyCtrl))  //ctrl键
            {
                _axRenderControl.FeatureManager.UnhighlightAll();
            }
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

        public void SetSkyBox(SkyBoxType sky)
        {
            string skyVal = ((int)sky).ToString();
            if (skyVal == "0" || skyVal == "4")
                skyVal = "0" + skyVal;

            // 获取天空盒
            var skybox = _axRenderControl.ObjectManager.GetSkyBox(0);
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBack, Path.Combine(tmpSkyboxPath, skyVal + "_BK.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBottom, Path.Combine(tmpSkyboxPath, skyVal + "_DN.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageFront, Path.Combine(tmpSkyboxPath, skyVal + "_FR.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageLeft, Path.Combine(tmpSkyboxPath, skyVal + "_LF.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageRight, Path.Combine(tmpSkyboxPath, skyVal + "_RT.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageTop, Path.Combine(tmpSkyboxPath, skyVal + "_UP.jpg"));
        }

        public void InitlizedCameraPosition(bool flag = false)
        {
            double tilt = flag ? -90 : -33.85;
            SetCameraValues(121.30204548415803, 31.18139334303109, 331.7257668711245, heading: 317.2000527037274, tilt: tilt, isLookAt: false);
        }

        public void SetCameraValues(double x, double y, double z, double heading = 0, double tilt = -60, double roll = 0, double distance = 2000, bool isLookAt = true)
        {
            IVector3 position = new Vector3() { X = x, Y = y, Z = z };
            var angle = SetAngle(heading, tilt, roll);
            SetCamera(position, angle, isLookAt, distance);
        }

        public void ShowAnimationOSG2(string osgName)
        {
            try
            {
                if (gfactory == null)
                    gfactory = new GeometryFactoryClass();
                string osgPath = Path.Combine(rootPath, osgName);
                string modelName = osgName.Split('.')[0];

                IResourceFactory pResFactory = new ResourceFactory();
                pResFactory.CreateModelAndImageFromFileEx(osgPath, out IPropertySet pProset, out _, out IModel pModel, out IMatrix pMaxtrx);

                //IVector3 pcenter = pMaxtrx.GetTranslate();
                //pcenter.Set(selectPoint.X, selectPoint.Y, osgName == "22.osg" ? 10 : selectPoint.Z);
                //pMaxtrx.SetTranslate(pcenter);

                IObjectManager objManager = _axRenderControl.ObjectManager;
                objManager.AddModel(modelName, pModel);
                string[] names = (string[])pProset.GetAllKeys();
                for (int i = 0; i < names.Length; i++)
                {
                    string name = names[i];
                    IImage image = pProset.GetProperty(name) as IImage;
                    if (image.ImageType == i3dImageType.i3dImageDynamic)
                    {
                        image.FrameInterval = 1000 / 6;
                    }
                    objManager.AddImage(name, image);
                }

                IModelPoint pModelPoint = gfactory.CreateGeometry(i3dGeometryType.i3dGeometryModelPoint,
                    i3dVertexAttribute.i3dVertexAttributeZ) as IModelPoint;
                pModelPoint.SpatialCRS = crs;
                pModelPoint.ModelName = modelName;
                pModelPoint.SetCoords(selectPoint.X, selectPoint.Y, osgName == "22.osg" ? 10 : 2, 0, 0);
                //pModelPoint.FromMatrix(pMaxtrx);
                pModelPoint.ModelEnvelope = pModel.Envelope;

                IRenderModelPoint rmp = objManager.CreateRenderModelPoint(pModelPoint, null);
                if (rmp != null)
                    rmp.MaxVisibleDistance = 50000;
                _axRenderControl.Camera.FlyToObject(rmp.Guid, i3dActionCode.i3dActionFlyTo);

                pResFactory = null;
                objManager = null;
                rmp = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ShowSkeletonAmimation(string xName)
        {
            string fullPath = Path.Combine(rootPath, xName);

            if (gfactory == null)
                gfactory = new GeometryFactory();

            IModelPoint mp = gfactory.CreateGeometry(i3dGeometryType.i3dGeometryModelPoint, i3dVertexAttribute.i3dVertexAttributeZ) as IModelPoint;
            mp.SpatialCRS = crs;
            mp.ModelName = fullPath;
            IMatrix matrix = new Matrix();
            matrix.MakeIdentity();
            Vector3 vector = new Vector3()
            {
                X = selectPoint.X,
                Y = selectPoint.Y,
                Z = 20
            };
            matrix.SetTranslate(vector);
            mp.FromMatrix(matrix);
            var obj = _axRenderControl.ObjectManager.CreateSkinnedMesh(mp);
            if (obj == null)
                return;
            obj.Loop = true;
            obj.Play();
            obj.MaxVisibleDistance = 50000;
            obj.ViewingDistance = 100;
            obj.MinVisiblePixels = 1;

            _axRenderControl.Camera.FlyToObject(obj.Guid, i3dActionCode.i3dActionFlyTo);
        }

        #region 轨迹相关

        private IRenderPolyline renderPolyline;
        public IRenderPolyline RenderPolyline
        {
            get => renderPolyline;
            set => renderPolyline = value;
        }

        ICurveSymbol lineSymbol;
        public void CreatePolyline()
        {
            var line = (IPolyline)gfactory.CreateGeometry(i3dGeometryType.i3dGeometryPolyline, i3dVertexAttribute.i3dVertexAttributeZ);
            line.SpatialCRS = crs;
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

        IVector3 posOffSet;
        public void PlayVehicleTrajectory(List<double[]> coordinates)
        {
            if (coordinates != null && coordinates.Count > 1 && carModelPoint != null)
            {
                AddPointToMotionPath(coordinates);
                //将创建的模型捆绑到运动路径上
                carModelPoint.Position = new Vector3() { X = coordinates[0][0], Y = coordinates[0][1], Z = 0 };
                IMotionable m = carModelPoint as IMotionable;//使模型可移动
                //m.Bind(motionPath, posOffSet, 0, 0, 0);//将模型绑定到路径上
                m.Bind2(dynamicObj, posOffSet, 0, 0, 0);

                ICurveSymbol cur = new CurveSymbol
                {
                    Color = 0xFFFF0000,//线颜色
                    Width = 2//线宽
                };//创建线符号

                _axRenderControl.ObjectManager.CreateRenderPolyline(line, cur);//显示辅助线

                _axRenderControl.Camera.FlyToObject(carModelPoint.Guid, i3dActionCode.i3dActionFollowBehind);

                //动画播放方法
                //motionPath.Play();  //根据路径播放动画
                dynamicObj.Play();
            }
        }

        public void StopVehicleTrajectory()
        {
            if (motionPath != null)
                motionPath.Stop();
        }

        public void PauseVehicleTrajectory()
        {
            if (motionPath != null)
                motionPath.Pause();

            if (dynamicObj != null)
                dynamicObj.Pause();
        }

        public void ContinuePlayVT()
        {
            if (motionPath != null)
                motionPath.Play();

            if (dynamicObj != null)
                dynamicObj.Play();
        }

        IMotionPath motionPath;
        IDynamicObject dynamicObj;
        IPolyline line;
        private void AddPointToMotionPath(List<double[]> coordinates)
        {
            if (gfactory == null)
                gfactory = new GeometryFactory();
            if (line == null)
                line = (IPolyline)gfactory.CreateGeometry(i3dGeometryType.i3dGeometryPolyline, i3dVertexAttribute.i3dVertexAttributeZ);
            line.SpatialCRS = crs;
            IPoint point = gfactory.CreatePoint(i3dVertexAttribute.i3dVertexAttributeZ);

            //创建运动路径
            //motionPath = _axRenderControl.ObjectManager.CreateMotionPath();//创建运动路径
            //motionPath.CrsWKT = WKT;

            dynamicObj = _axRenderControl.ObjectManager.CreateDynamicObject();
            dynamicObj.CrsWKT = WKT;
            dynamicObj.AutoRepeat = true;
            dynamicObj.TurnSpeed = 200;

            double[] currentPoint;
            double[] nextPoint;

            for (int i = 0; i < coordinates.Count - 1; i++)
            {
                currentPoint = coordinates[i];
                nextPoint = coordinates[i + 1];
                SetEulerAngle(currentPoint, nextPoint);
                SetIPoint(point, coordinates[i][0], coordinates[i][1], coordinates[i][2]);

                //dynamicObj.AddWaypoint2(point, i % 100 == 0 ? 0 : 20);

                dynamicObj.AddWaypoint2(point, 20);

                line.AppendPoint(point);

                //motionPath.AddWaypoint2(point, angle, scale, i * 0.1);//添加运动路径节点
            }
        }

        IVector3 position;
        private void SetIPoint(IPoint point, double x, double y, double z)
        {
            if (position == null)
                position = new Vector3();

            if (point != null)
            {
                position.Set(x, y, z);
                point.Position = position;
                point.SpatialCRS = crs;
            }
        }

        IEulerAngle angle;
        IVector3 pos1 = new Vector3();//向量1
        IVector3 pos2 = new Vector3(); //向量2
        private void SetEulerAngle(double[] currentPoint, double[] nextPoint)
        {
            pos1.Set(currentPoint[0], currentPoint[1], currentPoint[2]);
            pos2.Set(nextPoint[0], nextPoint[1], nextPoint[2]);
            angle = _axRenderControl.Camera.GetAimingAngles(pos1, pos2);//获取起点与终点夹角
        }

        IRenderModelPoint carModelPoint;
        double z = 1.8;
        public void LoadCarModel(string name)
        {
            //加载模型
            if (string.IsNullOrEmpty(name))
                name = "hddk07car015.osg";

            string path = Path.GetFullPath($@"../../../data/CarMDB/{name}");
            IGeometryFactory geoFac = new GeometryFactory();//创建几何工厂
            IModelPoint modelpoint = geoFac.CreateGeometry(i3dGeometryType.i3dGeometryModelPoint, i3dVertexAttribute.i3dVertexAttributeZ) as IModelPoint;// 构造ModelPoint
            modelpoint.ModelName = path;   //将模型绑定到ModelPoint上
            //modelpoint.SelfScale(0.1,0.1,0.1);
            modelpoint.SpatialCRS = crs;
            IVector3 pos1 = new Vector3();
            if (name == "WetRubbishVehicle_s.osg")
                z = 0;
            else
                z = 1.8;
            if (selectPoint != null)
                pos1.Set(selectPoint.X, selectPoint.Y, z);
            else
                pos1.Set(121.29864020314436, 31.18334045401004, z);
            modelpoint.Position = pos1;//模型起始位置
            carModelPoint = _axRenderControl.ObjectManager.CreateRenderModelPoint(modelpoint, null);//显示模型
            _axRenderControl.Camera.FlyToObject(carModelPoint.Guid, i3dActionCode.i3dActionFollowAbove);
        }

        // 实时轨迹
        public void RenderRealTimeTrajectory(Trajectory notation)
        {
            //System.Diagnostics.Debug.WriteLine($"VehicleNo={notation.VehicleNo}， 经度={notation.LongitudeWgs84}，纬度={notation.LatitudeWgs84}，GpsTime={notation.GPSTime}");
            if (carModelPoint != null)
            {
                AddPointToDynamicObject(notation);
                RenderPointInMap(notation);
            }
        }

        Dictionary<string, IDynamicObject> notationDic;
        IPoint po;
        string tracePlayID = string.Empty; // 当前正在进行轨迹回放的Id
        // 实时轨迹相关
        private void AddPointToDynamicObject(Trajectory notationDto)
        {
            if (notationDic.Keys.Contains(notationDto.VehicleNo))
            {
                dynamicObj = notationDic[notationDto.VehicleNo];
                if (dynamicObj.WaypointsNumber > 3)
                    dynamicObj.DeleteWaypoint(0);

                AddAndSetPointsToDynamicObj(notationDto);

                if (notationDto.VehicleNo == tracePlayID)
                {
                    dynamicObj.Pause();
                }
            }
            else
            {
                dynamicObj = _axRenderControl.ObjectManager.CreateDynamicObject();
                SetDynamicObject(dynamicObj);
                notationDic[notationDto.VehicleNo] = dynamicObj;

                AddAndSetPointsToDynamicObj(notationDto);

                IMotionable m = carModelPoint as IMotionable;//使模型可移动
                m.Bind2(dynamicObj, posOffSet, 0, 0, 0);
            }

            if (dynamicObj.WaypointsNumber > 2 && notationDto.VehicleNo != tracePlayID)
            {
                dynamicObj.Play();
            }
        }

        private void AddAndSetPointsToDynamicObj(Trajectory notationDto)
        {
            if (gfactory == null)
                gfactory = new GeometryFactory();

            if (po == null)
            {
                po = gfactory.CreatePoint(i3dVertexAttribute.i3dVertexAttributeZ);
            }

            SetIPoint(po, notationDto.LongitudeWgs84, notationDto.LatitudeWgs84, notationDto.Altitude * 0.1);

            dynamicObj.AddWaypoint2(po, 20);
        }

        private void SetDynamicObject(IDynamicObject obj, bool autoRepeat = false)
        {
            if (obj != null)
            {
                obj.AutoRepeat = autoRepeat;
                obj.CrsWKT = WKT;
                obj.TurnSpeed = 200;
                obj.MotionStyle = i3dDynamicMotionStyle.i3dDynamicMotionGroundVehicle;
            }
        }

       
        public void RenderPointInMap(Trajectory notationDto)
        {
            if (gfactory == null)
                gfactory = new GeometryFactory();

            SetRenderPoint(notationDto.LongitudeWgs84, notationDto.LatitudeWgs84, notationDto.Altitude * 0.1);
            //SetRenderPolyLine();
        }

        IPoint rPoint;
        
        private void SetRenderPoint(double x, double y, double z)
        {
            if (rPoint == null)
            {
                rPoint = gfactory.CreatePoint(i3dVertexAttribute.i3dVertexAttributeZ);
            }
            rPoint.SetCoords(x, y, z, 0, 1);
            rPoint.SpatialCRS = crs;

            var renderPoint = _axRenderControl.ObjectManager.CreateRenderPoint(rPoint, symbol);
            renderPoint.MaxVisibleDistance = 10000;
        }

        private void SetRenderPolyLine()
        {
            if (line == null)
                line = (IPolyline)gfactory.CreateGeometry(i3dGeometryType.i3dGeometryPolyline, i3dVertexAttribute.i3dVertexAttributeZ);

            line.SpatialCRS = crs;
            line.AppendPoint(rPoint);

            var rPolyline = _axRenderControl.ObjectManager.CreateRenderPolyline(line, new CurveSymbol { Color = 0xAA0CE6E5, Width = 2 });
            rPolyline.MaxVisibleDistance = 10000;
            rPolyline.HeightStyle = i3dHeightStyle.i3dHeightAbsolute;
        }
        #endregion
    }
}
