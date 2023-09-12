using Axi3dRenderEngine;
using i3dCommon;
using i3dFdeCore;
using i3dFdeDataInterop;
using i3dFdeGeometry;
using i3dMath;
using i3dRenderEngine;
using i3dResource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;

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
        private readonly string tmpSkyboxPath = @"C:\Program Files\LC\SkySceneryX64\skybox\";   //天空盒图片位置（SkyScenery安装位置）
        //private const string WKT = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137.0,298.257223563]]," +
    //"PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433],AUTHORITY[\"EPSG\",4326]]";

        private const string WKT = "PROJCS[\"CGCS2000_3_degree_Gauss_Kruger_CM_114E\",GEOGCS[\"GCS_China_Geodetic_Coordinate_System_2000\",DATUM[\"D_unknown\",SPHEROID[\"unretrievable_using_WGS84\",6378137.0,298.257223563]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"false_easting\",500000.0],PARAMETER[\"false_northing\",0.0],PARAMETER[\"central_meridian\",114.0],PARAMETER[\"scale_factor\",1.0],PARAMETER[\"latitude_of_origin\",0.0],UNIT[\"Meter\",1.0]]";
        private const uint modifyColor = 1694498815;
        //1694498815
        private bool _CTRL = false;

        private Dictionary<string, IFeatureLayer> fls = new Dictionary<string, IFeatureLayer>();
        public Dictionary<string, IFeatureLayer> Fls
        {
            get { return fls; }
            set { fls=value; }
        }
        /// <summary>
        /// 标记拾取时是否支持“ctrl”键用于多选
        /// </summary>
        public bool CTRL
        {
            get { return _CTRL; }
            set { _CTRL = value; }
        }

        /// <summary>
        /// 在WinForm中初始化地图
        /// </summary>
        /// <param name="axRenderControl">地图展现控件</param>
        /// <param name="_3dmname">3DM名称</param>
        /// <param name="isPlanarTerrain">true:平面地形，false：地球形（默认）</param>
        public void InitializationMapControl(AxRenderControl axRenderControl, string _3dmname = "JD.3DM", bool isPlanarTerrain = false)
        {
            try
            {
                _axRenderControl = axRenderControl ?? new AxRenderControl();

                gfactory = new GeometryFactory();
                notationDic = new Dictionary<string, IDynamicObject>();
                posOffSet = new Vector3() { X = 0, Y = 0, Z = 1.8 };
                symbol = new SimplePointSymbolClass() { FillColor = 0xAA0000FF, Size = 10 };

                scale = new Vector3();//创建向量
                scale.Set(1, 1, 1);//设置模型比例尺

                // 初始化RenderControl控件
                InitializeRenderControl(isPlanarTerrain);

                // 设置连接信息
                SetConnectionInfo(_3dmname);

                // 连接3DM并创建图层
                Open3DMAndCreateFeatureLayer();

                // 设置选择模式
                SetSelectModeAndRegisterEvent();

                // 设置天空盒
                SetSkyBox(SkyBoxType.BTXY);

                SetSpatialCRS(isPlanarTerrain);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetSpatialCRS(bool isPlanarTerrain)
        {
            var crsFactory = new CRSFactory();
            //crs = isPlanarTerrain == false ? crsFactory.CreateWGS84() :
            //    crsFactory.CreateCRS(i3dCoordinateReferenceSystemType.i3dCrsProject) as ISpatialCRS;

            crs = isPlanarTerrain == false ? crsFactory.CreateWGS84() :
               crsFactory.CreateFromWKT(WKT) as ISpatialCRS;
        }

        /// <summary>
        /// 初始化三维窗口
        /// </summary>
        /// <param name="isPlanarTerrain">true:平面地形，false：地球形</param>
        private void InitializeRenderControl(bool isPlanarTerrain)
        {
            PropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", i3dRenderSystem.i3dRenderOpenGL);

            //初始化三维窗口。isPlanarTerrain（true:平面地形，false：地球形）, params（配置参数）
            _axRenderControl.Initialize(isPlanarTerrain, ps);
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
            return $"{Path.GetFullPath(@"../../../data/3dm/")}{mapname}";
        }

        IDataSource ds;
        string dsName;
        private void Open3DMAndCreateFeatureLayer()
        {
            try
            {
                var dsFactory = new DataSourceFactory();
                ds = dsFactory.OpenDataSource(_ci);

                string[] setnames = (string[])ds.GetFeatureDatasetNames();
                if (setnames == null || setnames.Length == 0) return;

                foreach (string name in setnames)
                {
                    dsName = name;
                    var dataSet = ds.OpenFeatureDataset(name);
                    var fcnames = (string[])dataSet.GetNamesByType(i3dDataSetType.i3dDataSetFeatureClassTable);
                    if (fcnames == null || fcnames.Length == 0) continue;

                    foreach (string fcname in fcnames)
                    {
                        var fc = dataSet.OpenFeatureClass(fcname);
                        DrawMapByFc(fc, fcname);
                        SetLookAt(fc);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DrawMapByFc(IFeatureClass fc, string fcname, string geoField = "Geometry")
        {
            var featureLayer = _axRenderControl.ObjectManager.CreateFeatureLayer(fc, geoField, null, null);
            if (featureLayer != null)
                featureLayer.MaxVisibleDistance = 50000;

            

            if (!fls.ContainsKey(fcname))
                fls.Add(fcname, featureLayer);
        }

        private void SetLookAt(IFeatureClass fc)
        {
            IFieldInfoCollection fieldinfos = fc.GetFields();
            IFieldInfo fieldinfo = fieldinfos.Get(fieldinfos.IndexOf("Geometry"));
            //IFieldInfo fieldinfo = fieldinfos.Get(fieldinfos.IndexOf(geoField));
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

        public void SetMapModel(bool isNormal)
        {
            _axRenderControl.InteractMode = isNormal ? i3dInteractMode.i3dInteractNormal : i3dInteractMode.i3dInteractSelect;
        }

        public void MeasureCoordinate(bool isNormal)
        {
            if (isNormal)
            {
                _axRenderControl.InteractMode = i3dInteractMode.i3dInteractNormal;
            }
            else
            {
                _axRenderControl.InteractMode = i3dInteractMode.i3dInteractMeasurement;
                _axRenderControl.MeasurementMode = i3dMeasurementMode.i3dMeasureCoordinate;
            }
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
                FeaturePickOrCreate(pickResult, position);
                selectPoint = position;
                //ShowAnimationOSG2("蝴蝶1.OSG");
            }
        }

        public event Action<string> CallbackMsg;

        public virtual void RaiseCallbackMsg(string msg)
        {
            CallbackMsg?.Invoke(msg);
        }

        public bool IsGetPlatePos { get; set; } = false;

        public CreateObjType ObjType { get; set; } = CreateObjType.CreateLabel;
        private void FeaturePickOrCreate(IPickResult pr, IPoint position)
        {
            if (pr == null)
                return;

            string msg = string.Empty;
            switch (pr)
            {
                case ILabelPickResult label:
                    msg = "拾取到" + label.Type + "类型，内容为" + label.Label.Text;
                    break;
                case IRenderModelPointPickResult model:
                    msg = IsGetPlatePos ? $"X={position.X - carVec.X}, Y={position.Y - carVec.Y}, Z={position.Z - carVec.Z}" :
                        $"拾取到{model.Type}类型，模型名称为{model.ModelPoint.ModelName}";
                    break;
                case IRenderPointPickResult point:
                    msg = "拾取到" + point.Type + "类型，大小为" + point.Point.Symbol.Size;
                    break;
                case IRenderPolylinePickResult polyLine:
                    msg = "拾取到" + polyLine.Type + "类型，GUID为" + polyLine.Polyline.Guid;
                    break;
                case IRenderPolygonPickResult polygon:
                    msg = "拾取到" + polygon.Type + "类型，GUID为" + polygon.Polygon.Guid;
                    break;
                default:
                    CreateObjectOnRenderControl(ObjType, position);
                    break;
            }
            RaiseCallbackMsg(msg);
        }

        private void CreateObjectOnRenderControl(CreateObjType type, IPoint point)
        {
            switch (type)
            {
                case CreateObjType.CreateLabel:
                    CreateLabel(point);
                    break;
                case CreateObjType.CreateRenderModelPoint:
                    CreateRenderModelPoint(point);
                    break;
                case CreateObjType.CreateRenderPoint:
                    CreateRenderPoint(point);
                    break;
                case CreateObjType.CreateRenderPolyline:
                    CreateRenderPolyline(point);
                    break;
                case CreateObjType.CreateRenderPolygon:
                    CreateRenderPolygon(point);
                    break;
                case CreateObjType.CreateFixedBillboard:
                    break;
                default:
                    break;
            }
        }

        #region 创建地图对象
        private ILabel label = null;
        private ITextSymbol textSymbol = null;
        private TextAttribute textAttribute = null;
        private void CreateLabel(IPoint point)
        {
            // 创建文本
            if (label == null)
                label = _axRenderControl.ObjectManager.CreateLabel();

            label.Text = $"X = {point.X}, Y = {point.Y}, Z = {point.Z}";
            label.Position = point;

            if (textAttribute == null)
                textAttribute = new TextAttribute();

            textAttribute.TextColor = 0xffffff00;
            textAttribute.TextSize = 15;
            textAttribute.Underline = true;
            textAttribute.Font = "楷体";

            if (textSymbol == null)
                textSymbol = new TextSymbol();
            textSymbol.TextAttribute = textAttribute;
            textSymbol.VerticalOffset = 10;
            textSymbol.DrawLine = true;
            textSymbol.MarginColor = 0x8800ffff;

            label.TextSymbol = textSymbol;
            _axRenderControl.Camera.FlyToObject(label.Guid, i3dActionCode.i3dActionFlyTo);
        }


        private IModelPoint fde_modelpoint = null;
        private IRenderModelPoint rmodelpoint = null;
        private void CreateRenderModelPoint(IPoint point)
        {
            // 创建模型
            if (gfactory == null)
                gfactory = new GeometryFactory();

            //string tmpOSGPath = Path.Combine(rootPath, @"data\OSG\JGRW006w.OSG");
            string tmpOSGPath = Path.GetFullPath(@"../../../data/Apartment/Apartment.osg");
            //string tmpOSGPath = Environment.CurrentDirectory + @"\Apartment\Apartment.osg";
            fde_modelpoint = gfactory.CreateGeometry(i3dGeometryType.i3dGeometryModelPoint, i3dVertexAttribute.i3dVertexAttributeZ) as IModelPoint;
            fde_modelpoint.SpatialCRS = crs;
            fde_modelpoint.SetCoords(point.X, point.Y, point.Z, 0, 0);
            fde_modelpoint.ModelName = tmpOSGPath;

            rmodelpoint = _axRenderControl.ObjectManager.CreateRenderModelPoint(fde_modelpoint, null);
            rmodelpoint.MaxVisibleDistance = double.MaxValue;
            rmodelpoint.MinVisiblePixels = 0;
            //IEulerAngle angle = new EulerAngle();
            //angle.Set(0, -20, 0);

            //_axRenderControl.Camera.LookAt2(point, 100, angle);
        }


        private IPoint fde_point = null;
        private ISimplePointSymbol pointSymbol = null;
        private IRenderPoint rpoint = null;
        private void CreateRenderPoint(IPoint point)
        {
            if (gfactory == null)
                gfactory = new GeometryFactory();

            fde_point = (IPoint)gfactory.CreateGeometry(i3dGeometryType.i3dGeometryPoint, i3dVertexAttribute.i3dVertexAttributeZ);
            fde_point.SpatialCRS = crs;
            fde_point.SetCoords(point.X, point.Y, point.Z, 0, 0);

            pointSymbol = new SimplePointSymbolClass
            {
                FillColor = 0xff0000ff,
                Size = 10
            };
            rpoint = _axRenderControl.ObjectManager.CreateRenderPoint(fde_point, pointSymbol);

            _axRenderControl.Camera.FlyToObject(rpoint.Guid, i3dActionCode.i3dActionFlyTo);
        }


        private IPolyline fde_polyline = null;
        private IRenderPolyline rpolyline = null;
        //private CurveSymbolClass lineSymbol = null;
        private void CreateRenderPolyline(IPoint point)
        {
            if (gfactory == null)
                gfactory = new GeometryFactory();

            fde_polyline = (IPolyline)gfactory.CreateGeometry(i3dGeometryType.i3dGeometryPolyline, i3dVertexAttribute.i3dVertexAttributeZ);
            fde_polyline.SpatialCRS = crs;

            fde_point = (IPoint)gfactory.CreateGeometry(i3dGeometryType.i3dGeometryPoint, i3dVertexAttribute.i3dVertexAttributeZ);
            fde_point.SpatialCRS = crs;

            fde_point.SetCoords(point.X, point.Y, point.Z, 0, 0);
            fde_polyline.AppendPoint(fde_point);

            fde_point.SetCoords(point.X + 20, point.Y, point.Z, 0, 0);
            fde_polyline.AppendPoint(fde_point);

            fde_point.SetCoords(point.X + 20, point.Y + 20, point.Z, 0, 0);
            fde_polyline.AppendPoint(fde_point);

            fde_point.SetCoords(point.X + 20, point.Y + 20, point.Z + 20, 0, 0);
            fde_polyline.AppendPoint(fde_point);

            lineSymbol = new CurveSymbolClass
            {
                Color = 0xffff00ff  // 紫红色
            };

            rpolyline = _axRenderControl.ObjectManager.CreateRenderPolyline(fde_polyline, lineSymbol);

            _axRenderControl.Camera.FlyToObject(rpolyline.Guid, i3dActionCode.i3dActionFlyTo);
        }


        private IPolygon fde_polygon = null;
        private IRenderPolygon rpolygon = null;
        private ISurfaceSymbol surfaceSymbol = null;
        private void CreateRenderPolygon(IPoint point)
        {
            if (gfactory == null)
                gfactory = new GeometryFactory();

            fde_polygon = (IPolygon)gfactory.CreateGeometry(i3dGeometryType.i3dGeometryPolygon, i3dVertexAttribute.i3dVertexAttributeZ);
            fde_polygon.SpatialCRS = crs;

            fde_point = (IPoint)gfactory.CreateGeometry(i3dGeometryType.i3dGeometryPoint, i3dVertexAttribute.i3dVertexAttributeZ);

            fde_point.SetCoords(point.X, point.Y, point.Z, 0, 0);
            fde_polygon.ExteriorRing.AppendPoint(fde_point);

            fde_point.SetCoords(point.X + 10, point.Y, point.Z, 0, 0);
            fde_polygon.ExteriorRing.AppendPoint(fde_point);

            fde_point.SetCoords(point.X + 10, point.Y + 10, point.Z, 0, 0);
            fde_polygon.ExteriorRing.AppendPoint(fde_point);

            fde_point.SetCoords(point.X, point.Y + 10, point.Z, 0, 0);
            fde_polygon.ExteriorRing.AppendPoint(fde_point);

            surfaceSymbol = new SurfaceSymbolClass();
            surfaceSymbol.Color = 0xFF0000FF;  // 蓝色

            rpolygon = _axRenderControl.ObjectManager.CreateRenderPolygon(fde_polygon, surfaceSymbol);

            _axRenderControl.Camera.FlyToObject(rpolygon.Guid, i3dActionCode.i3dActionFlyTo);
        }
        #endregion

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
            // 美都3dm初始位置
            double tilt = flag ? -90 : -33.85;
            SetCameraValues(121.30204548415803, 31.18139334303109, 331.7257668711245, heading: 317.2000527037274, tilt: tilt, isLookAt: false);
        }

        public void InitlizedGKDCameraPosition()
        {
            // 国科大3dm初始位置
            SetCameraValues(116.24467033505763, 39.9067384367435, 3.2176464665681124, 8.8422702994738636, 0, 0, 2000, false);

            // 国科大广告牌位置
            //SetCameraValues(116.245185436104, 39.9071683998616, 4.65836078487337, 100.630121481486, -12.3758727789334, 0, 2000, false);
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
                {
                    rmp.MaxVisibleDistance = 50000;
                    _axRenderControl.Camera.FlyToObject(rmp.Guid, i3dActionCode.i3dActionFlyTo);
                }

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
            if (selectPoint == null)
                return;

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

        // 显示视频投射
        ITerrainVideo video = null;
        IPoint videoPoint = null;
        IEulerAngle videoAngle = null;
        public string ShowVideoProjection(double x, double y, double z, double heading,
            double tilt, double roll, double farClip, double aspectRadio, double fieldOfView, double vPos)
        {
            if (video == null)
            {
                //var vUrl = Path.GetFullPath(@"../../../data/JiaDing.mp4");
                var vUrl = Path.GetFullPath(@"../../../data/Persion.mp4");
                videoAngle = new EulerAngle();

                videoPoint = gfactory.CreatePoint(i3dVertexAttribute.i3dVertexAttributeZ);
                SetIPoint(videoPoint, x, y, z);
                video = _axRenderControl.ObjectManager.CreateTerrainVideo(videoPoint);

                video.PlayVideoOnStartup = true;
                video.ShowProjectionLines = true;
                video.ShowProjector = true;
                video.VideoFileName = vUrl;
            }
            else
            {
                SetIPoint(videoPoint, x, y, z);
                video.Position = videoPoint;
            }

            video.FarClip = farClip;
            videoAngle.Set(heading, tilt, roll);
            video.Angle = videoAngle;

            video.AspectRatio = aspectRadio;
            video.VideoPosition = vPos;
            video.FieldOfView = fieldOfView;

            var a = video.AspectRatio;
            var b = video.VideoPosition;
            var c = video.FieldOfView;

            string res = $"AspectRatio = {a}, FieldOfView = {c}, VideoPosition = {b}";
            return res;
        }

        /// <summary>
        /// 获取当前相机位置
        /// </summary>
        /// <returns></returns>
        public Position GetCameraPosition()
        {
            _axRenderControl.Camera.GetCamera(out IVector3 vector3, out IEulerAngle angle);
            Position position = new Position() { X = vector3.X, Y = vector3.Y, Z = vector3.Z, Heading = angle.Heading, Tilt = angle.Tilt, Roll = angle.Roll };
            return position;
        }

        /// <summary>
        /// 生成gif动画
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void CreateRenderGif(Gif gif)
        {
            var point = gfactory.CreatePoint(i3dVertexAttribute.i3dVertexAttributeZ);
            SetIPoint(point, gif.X, gif.Y, gif.Z);
            IVector3 widthvec = new Vector3() { X = gif.WidthVecX, Y = gif.WidthVecY, Z = gif.WidthVecZ };
            IVector3 heightvec = new Vector3() { X = gif.HeightVecX, Y = gif.HeightVecY, Z = gif.HeightVecZ };
            CurveSymbol boundary = new CurveSymbol()
            {
                Color = 0x00FFFFFF
            };
            ISurfaceSymbol symbol = new SurfaceSymbol
            {
                Color = 0xDDFFFFFF,
                BoundarySymbol = boundary,
                ImageName = Path.GetFullPath($@"../../../data/GIF/{gif.GifName}"),
            };
            var render = _axRenderControl.ObjectManager.CreateRenderTextureQuad(point, widthvec, heightvec, symbol);
            render.MaxVisibleDistance = 500;
            render.VisibleMask = i3dViewportMask.i3dViewAllNormalView;
            render.ForceCullMode = true;
            render.CullMode = i3dCullFaceMode.i3dCullNone; // 关闭背面裁剪
        }
        #region 轨迹相关

        private IRenderPolyline renderPolyline;
        public IRenderPolyline RenderPolyline
        {
            get => renderPolyline;
            set => renderPolyline = value;
        }

        ICurveSymbol lineSymbol = null;
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

        IMotionPath motionPath = null;
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
        Guid currentCarGuid;
        double z = 1.8;
        IVector3 carVec= new Vector3();
        public void LoadCarModel(string name)
        {
            if (currentCarGuid != null)
                _axRenderControl.ObjectManager.DeleteObject(currentCarGuid);

            //加载模型
            if (string.IsNullOrEmpty(name))
                name = "CleanUpVehicle.osg";

            string path = Path.GetFullPath($@"../../../data/CarMDB/{name}");
            IGeometryFactory geoFac = new GeometryFactory();//创建几何工厂
            IModelPoint modelpoint = geoFac.CreateGeometry(i3dGeometryType.i3dGeometryModelPoint, i3dVertexAttribute.i3dVertexAttributeZ) as IModelPoint;// 构造ModelPoint
            modelpoint.ModelName = path;   //将模型绑定到ModelPoint上

            //modelpoint.SelfScale(0.1,0.1,0.1);
            modelpoint.SpatialCRS = crs;
            
            if (name == "WetRubbishVehicle_s.osg")
                z = 0;

            if (selectPoint != null)
                carVec.Set(selectPoint.X, selectPoint.Y, z);
            else
            {
                carVec.Set(121.29864020314436, 31.18334045401004, z);
                //pos1.Set(483728.30192234676, 3451679.6690112567, z);
            }

            modelpoint.Position = carVec;//模型起始位置

            if (name == "hddk07car020.osg")
                modelpoint.Matrix33 = new float[9] { -0.6038073f, -0.7971303f, 0, 0.7971303f, -0.6038073f, 0, 0, 0, 1 };

            carModelPoint = _axRenderControl.ObjectManager.CreateRenderModelPoint(modelpoint, null);//显示模型
            currentCarGuid = carModelPoint.Guid;
            _axRenderControl.Camera.FlyToObject(currentCarGuid, i3dActionCode.i3dActionFollowAbove);
        }

        /// <summary>
        /// 设置车牌号码和车牌位置
        /// </summary>
        public void AttachPlate(string carName,string carNum)
        {
            string path = Path.GetFullPath($@"../../../data/CarMDB/{carName}");
            IGeometryFactory geoFac = new GeometryFactory();//创建几何工厂
            IModelPoint modelpoint = geoFac.CreateGeometry(i3dGeometryType.i3dGeometryModelPoint, i3dVertexAttribute.i3dVertexAttributeZ) as IModelPoint;// 构造ModelPoint
            modelpoint.ModelName = path;   //将模型绑定到ModelPoint上
            modelpoint.SpatialCRS = crs;
            IVector3 pos1 = new Vector3();
            pos1.Set(121.29864020314436, 31.18334045401004, z);
            modelpoint.Position = pos1;

            IRenderCar rcar = _axRenderControl.ObjectManager.CreateRenderCar(modelpoint, null);
            rcar.CarNumber = carNum;
            rcar.CarPlateStyle = i3dCarPlateStyle.i3dLargeCarPlate;
            var frontPlate = SetCarFlontPlateABCPointVector();
            var backPlate = SetCarBackPlateABCPointVector();
            rcar.AttachPlate(frontPlate.Item1, frontPlate.Item2, frontPlate.Item3, backPlate.Item1, backPlate.Item2, backPlate.Item3);
            _axRenderControl.Camera.FlyToObject(rcar.Guid, i3dActionCode.i3dActionFollowCockpit);
        }

        private (IVector3,IVector3,IVector3) SetCarFlontPlateABCPointVector()
        {
            IVector3 frontPlateA= new Vector3();
            frontPlateA.Set(0.5, 1, -0.2);

            IVector3 frontPlateB = new Vector3();
            frontPlateB.Set(0.5, 1, 0.3);

            IVector3 frontPlateC = new Vector3();
            frontPlateC.Set(-0.5, 1, -0.2);

            return (frontPlateA, frontPlateB, frontPlateC);
        }

        private (IVector3, IVector3, IVector3) SetCarBackPlateABCPointVector()
        {
            IVector3 backPlateA = new Vector3();
            backPlateA.Set(-0.5, -8, -0.2);

            IVector3 backPlateB = new Vector3();
            backPlateB.Set(-0.5, -8, 0.3);

            IVector3 backPlateC = new Vector3();
            backPlateC.Set(0.5, -8, -0.2);

            return (backPlateA, backPlateB, backPlateC);
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

        #region Shp文件相关
        public (int, string) UpLoadShapFile(string shpPath)
        {
            int count = 0;
            if (!File.Exists(shpPath) || ds == null)
                return (count, $"不存在{shpPath}文件！");
            var di = GetConnection(shpPath);
            if (di == null)
                return (count, $"执行方法{nameof(GetConnection)}错误！");

            string fileName = Path.GetFileNameWithoutExtension(shpPath);
            ILayerInfo layer = di.LayersInfo[0];
            IFieldInfo geoField = layer.FieldInfos.Get(layer.FieldInfos.IndexOf("Geometry"));
            if (geoField == null || geoField.GeometryDef == null)
                return (count, $"执行方法{nameof(UpLoadShapFile)}错误！");

            try
            {
                var fcList = ImportFC(di, ds, dsName, fileName);
                return fcList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        string geoField = "ColumnsVectorModel";
        public void PullupBlock(string fcName, string fieldName)
        {
            if (HasFeatureClass(ds, dsName, fcName, out IFeatureClass fc))
            {
                IFieldInfo fi = new FieldInfoClass()
                {
                    Name = geoField,
                    FieldType = i3dFieldType.i3dFieldGeometry,
                    Length = 200,
                    RegisteredRenderIndex = true,
                    GeometryDef = new GeometryDefClass()
                    {
                        HasZ = true,
                        GeometryColumnType = i3dGeometryColumnType.i3dGeometryColumnModelPoint
                    }
                };


                // 先删除一下
                _axRenderControl.ObjectManager.DeleteObject(Fls[fcName].Guid);

                AddFieldToFeatureClass(fc, geoField, fi);

                _axRenderControl.PauseRendering(false);
                fc.SetRenderIndexEnabled(geoField, false);
                fc.SetRenderIndexEnabled("Geometry", true);

                ds.StartEditing();
                FillFieldValueToFeatureClass(fc, fieldName);
                ds.StopEditing(true);

                _axRenderControl.ResumeRendering();
                //_axRenderControl.FeatureManager.RefreshFeatureClass(fc);

                // 地图重绘
                DrawMapByFc(fc, fc.AliasName, geoField);
            }
        }

        public void DeleteGeometry(string fcName)
        {
            if(HasFeatureClass(ds, dsName, fcName, out IFeatureClass fc))
            {
                fc.DeleteField("Geometry");
            }
        }

        public void CreateTerrainLayers(string path)
        {
            if (File.Exists(path))
            {
                bool b = _axRenderControl.Terrain.RegisterTerrain(path, "");
            }
        }

        /// <summary>
        /// 绑定刷新listView
        /// </summary>
        /// <param name="listView"></param>
        public void RebindingListView(ListView listView)
        {
            if (fls != null && listView != null)
            {
                if (listView.Items.Count > 0)
                    listView.Items.Clear();
                foreach (var item in fls.Keys)
                {
                    listView.Items.Add(new ListNode(item, fls[item]) { Checked = true });
                }
            }
        }

        public void LookAtEnvelope(ListNode item)
        {
            if (item != null && item.layer != null)
                _axRenderControl.Camera.LookAtEnvelope(item.layer.Envelope);
        }

        private void FillFieldValueToFeatureClass(IFeatureClass fc, string floorName)
        {
            var cursor = fc.Update(new QueryFilterClass());
            var res = fc.FeatureDataSet as IResourceManager;
            IRowBuffer row;
            var floorNumIndex = fc.GetFields().IndexOf(floorName);
            if (floorNumIndex < 0)
                return;

            int posModelPoint = fc.GetFields().IndexOf(geoField);
            
            while ((row = cursor.NextRow()) != null)
            {
                try
                {
                    int nPose = fc.GetFields().IndexOf("Geometry");
                    var geo = (IGeometry)row.GetValue(nPose);
                    if (geo == null)
                        continue;

                    var type = geo.GeometryType;
                    if (type == i3dGeometryType.i3dGeometryPolygon)
                    {
                        var polygon = geo as IPolygon;
                        var floorNum = int.Parse(row.GetValue(floorNumIndex).ToString() ?? "0");
                        var _geoConvert = new GeometryConvertorClass();
                        _geoConvert.ExtrudePolygonToModel(polygon, floorNum, 3, 0, i3dRoofType.i3dRoofFlat,
                            string.Empty, string.Empty, out IModelPoint modelPoint, out IModel model);

                        UpdateModelColor(model);
                        //modelPoint.Z = 100;
                        var z = modelPoint.Z;
                        if (modelPoint != null)
                            row.SetValue(posModelPoint, modelPoint);

                        cursor.UpdateRow(row);
                        
                        res.AddModel(modelPoint.ModelName, model, null);
                        System.Diagnostics.Debug.WriteLine($"更新位置{modelPoint.ModelName}成功！\r\n");
                    }
                }
                catch (COMException ex)
                {
                    throw ex;
                }
            }
            if (cursor != null)
            {
                Marshal.ReleaseComObject(cursor);
            }
        }


        private void UpdateModelColor(IModel model)
        {
            if (model != null)
            {
                for (int i = 0; i < model.GroupCount; i++)
                {
                    var gdg = model.GetGroup(i);
                    if(gdg!=null && gdg.PrimitiveCount > 0)
                    {
                        for (int j = 0; j < gdg.PrimitiveCount; j++)
                        {
                            var gdp = gdg.GetPrimitive(j);
                            gdp.Material.DiffuseColor = modifyColor;
                            gdp.Material.EnableLight = true;
                        }
                    }
                }
            }
        }


        private IDataInterop GetConnection(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            PropertySet ps = new PropertySet();
            DataInteropFactory factory = new DataInteropFactory();
            IDataInterop di = null;
            string exten = Path.GetExtension(filePath).ToLower();
            if (exten == ".shp")
            {
                ps.SetProperty("FILENAME", filePath);
                di = factory.CreateDataInterop(i3dDataConnectionType.i3dOgrConnectionShp, ps);
            }
            return di;
        }

        private (int,string) ImportFC(IDataInterop di, IDataSource ds,string dsName, string fileName)
        {
            int nCount = 0;
            string msg = "成功！";
            try
            {
                //string dsWkt = crs.AsWKT();
                ILayerInfoCollection layerInfos = di.LayersInfo;
                for (int i = 0; i < layerInfos.Count; i++)
                {
                    var layer = layerInfos[i];
                    //// 检查空间参考是否一致
                    //if (!CompareCRS(layer.CrsWKT, dsWkt))
                    //{
                    //    msg = "导入的shp文件空间参考与3dm不一致！";
                    //    break;
                    //}

                    string fcName = $"UP_{DateTime.Now.Millisecond}_{fileName}";
                    var fc = CreateFeatureClass(ds, dsName, fcName, layer.FieldInfos);
                    if (fc != null)
                    {
                        fc.AliasName = layer.Name;
                        // 开始导入
                        fc.LockType = i3dLockType.i3dLockExclusiveSchema;
                        int n = di.ImportLayer(fc, "Geometry", null);
                        fc.LockType = i3dLockType.i3dLockSharedSchema;

                        // 添加渲染索引
                        //BuildRenderIndex(fc, "Geometry", 500);
                        nCount += n;

                        // 地图重绘
                        DrawMapByFc(fc, fcName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return (nCount, msg);
        }

        /// <summary>
        /// 添加指定名称字段
        /// </summary>
        /// <param name="fc">要素类</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldType">字段类型</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="nullable">是否可空</param>
        /// <param name="registeredRenderIndex">是否注册渲染索引</param>
        private void AddFieldToFetureClass(IFeatureClass fc, string fieldName, IFieldInfo field)
        {
            if (fc == null)
                return;

            // 如果存在则先删除
            if (fc.GetFields().IndexOf(fieldName) != -1)
                fc.DeleteField(fieldName);

            fc.AddField(field);
        }

        /// <summary>
        /// 添加指定名称字段
        /// </summary>
        /// <param name="fc">要素类</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fi">要添加的字段</param>
        private void AddFieldToFeatureClass(IFeatureClass fc, string fieldName, IFieldInfo fi)
        {
            if (fc == null)
                return;

            try
            {
                // 如果存在则不添加
                if (fc.GetFields().IndexOf(fieldName) != -1)
                    return;

                fc.LockType = i3dLockType.i3dLockExclusiveSchema;
                fc.AddField(fi);
                fc.LockType = i3dLockType.i3dLockSharedSchema;

                //BuildSpatialIndexIfNotHas(fc, fieldName);
                //BuildRenderIndexIfNotHas(fc, fieldName);

            }
            catch (COMException ex)
            {
                throw ex;
            }
        }

        private void BuildSpatialIndexIfNotHas(IFeatureClass fc, string fieldName)
        {
            var gridCollection = fc.GetSpatialIndexInfos();
            if (gridCollection != null && gridCollection.Count > 0)
            {
                bool hasGrid = false;
                for (int i = 0; i < gridCollection.Count; i++)
                {
                    var index = gridCollection[i];
                    if (index.IndexType == i3dIndexType.i3dIndexGrid)
                    {
                        if ((index as IGridIndexInfo).GeoColumnName == fieldName)
                        {
                            hasGrid = true;
                            break;
                        }
                    }
                }

                // 如果空间列没有空间索引，则创建
                if (!hasGrid)
                {
                    BuildSpatialIndex(fc, fieldName);
                }
            }
            else
                BuildSpatialIndex(fc, fieldName);
        }

        private void BuildSpatialIndex(IFeatureClass fc, string fieldName)
        {
            if (fc == null)
                return;

            fc.AddSpatialIndex(new GridIndexInfo
            {
                L1 = 500,
                L2 = 2000,
                L3 = 10000,
                GeoColumnName = fieldName
            });
        }


        private void BuildRenderIndexIfNotHas(IFeatureClass fc, string fieldName)
        {
            //判断footPrint是否建立渲染索引，没有则创建
            var renderCollection = fc.GetRenderIndexInfos();
            if (renderCollection != null && renderCollection.Count > 0)
            {
                bool hasRender = false;
                for (int i = 0; i < renderCollection.Count; i++)
                {
                    IIndexInfo index = renderCollection[i];
                    if (index.IndexType == i3dIndexType.i3dIndexRender)
                    {
                        if ((index as IRenderIndexInfo).GeoColumnName == fieldName)
                        {
                            hasRender = true;
                            break;
                        }
                    }
                }
                if (!hasRender)
                {
                    BuildRenderIndex(fc, fieldName, 500);
                }
            }
            else
            {
                BuildRenderIndex(fc, fieldName, 500);
            }
        }

        /// <summary>
        /// 添加渲染索引
        /// </summary>
        /// <param name="fc"></param>
        /// <param name="geoColumnName"></param>
        /// <param name="l1"></param>
        private void BuildRenderIndex(IFeatureClass fc, string geoColumnName, double l1)
        {
            if (fc == null)
                return;
            fc.AddRenderIndex(new RenderIndexInfo()
            {
                GeoColumnName = geoColumnName,
                L1 = l1
            });
        }

        /// <summary>
        /// 创建要素类
        /// </summary>
        /// <param name="ds">数据源</param>
        /// <param name="dsName">数据集名称</param>
        /// <param name="fcName">要素名称</param>
        /// <param name="fields">字段集</param>
        /// <returns></returns>
        private IFeatureClass CreateFeatureClass(IDataSource ds, string dsName, string fcName, IFieldInfoCollection fields)
        {
            IFeatureDataSet dataSet = null;
            try
            {
                if (ds == null || !HasDataSet(ds, dsName, out dataSet) || dataSet == null)
                    return null;
                IFeatureClass fc = dataSet.CreateFeatureClass(fcName, fields);
                return fc;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (dataSet != null)
                {
                    Marshal.ReleaseComObject(dataSet);
                }
            }
        }

        /// <summary>
        /// 数据源是否存在指定名称的数据集
        /// </summary>
        /// <param name="ds">数据源</param>
        /// <param name="dsName">数据集名称</param>
        /// <param name="dataSet">数据集</param>
        /// <returns></returns>
        private bool HasDataSet(IDataSource ds, string dsName, out IFeatureDataSet dataSet)
        {
            dataSet = null;
            if (ds == null)
                return false;
            try
            {
                string[] arrDsNames = (string[])ds.GetFeatureDatasetNames();
                if (arrDsNames == null || Array.IndexOf(arrDsNames, dsName) == -1)
                    return false;
                dataSet = ds.OpenFeatureDataset(dsName);
                return dataSet != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 数据源中是否有指定要素类
        /// </summary>
        /// <param name="ds">数据源</param>
        /// <param name="dsName">数据集名称</param>
        /// <param name="fcname">要素名称</param>
        /// <param name="fc">要素类</param>
        /// <returns></returns>
        private bool HasFeatureClass(IDataSource ds, string dsName, string fcname, out IFeatureClass fc)
        {
            fc = null;
            IFeatureDataSet dataSet = null;
            if (ds == null)
                return false;
            try
            {
                if (!HasDataSet(ds, dsName, out dataSet) || dataSet == null)
                    return false;
                string[] arrFcName = (string[])dataSet.GetNamesByType(i3dDataSetType.i3dDataSetFeatureClassTable);
                if (arrFcName == null || Array.IndexOf(arrFcName, fcname) == -1)
                    return false;
                fc = dataSet.OpenFeatureClass(fcname);
                return fc != null;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (dataSet != null)
                {
                    Marshal.ReleaseComObject(dataSet);
                }
            }
        }

        /// <summary>
        /// 比较两个坐标系是否一致
        /// </summary>
        /// <param name="wkt1">坐标系1</param>
        /// <param name="wkt2">坐标系2</param>
        /// <returns></returns>
        private bool CompareCRS(string wkt1, string wkt2)
        {
            try
            {
                ICRSFactory cRSFactory = new CRSFactoryClass();
                var gcrs1= cRSFactory.CreateFromWKT(wkt1);
                var gcrs2 = cRSFactory.CreateFromWKT(wkt2);
                return gcrs1.IsSame(gcrs2);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GetGeometryColumn(IFeatureClass fc)
        {
            if (fc == null) return string.Empty;

            var fields = fc.GetFields();
            int num = fields.Count;
            IFieldInfo field;
            for (int i = 0; i < num; i++)
            {
                field = fields.Get(i);
                if (field.FieldType == i3dFieldType.i3dFieldGeometry)
                    return field.Name;
            }
            return "Geometry";
        }
        #endregion
    }

    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Heading { get; set; }
        public double Tilt { get; set; }
        public double Roll { get; set; }
    }

    public class Gif
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public string GifName { get; set; }
        public double WidthVecX { get; set; }
        public double WidthVecY { get; set; }
        public double WidthVecZ { get; set; }
        public double HeightVecX { get; set; }
        public double HeightVecY { get; set; }
        public double HeightVecZ { get; set; }
    }

    /// <summary>
    /// 创建类型
    /// </summary>
    public enum CreateObjType
    {
        [Description("创建文本")]
        CreateLabel,

        [Description("创建模型")]
        CreateRenderModelPoint,

        [Description("创建点")]
        CreateRenderPoint,

        [Description("创建线")]
        CreateRenderPolyline,

        [Description("创建多边形")]
        CreateRenderPolygon,

        [Description("创建POI")]
        CreateRenderPOI,

        [Description("创建固定广告牌")]
        CreateFixedBillboard
    }

    public class ListNode : ListViewItem
    {
        public string name;
        public IFeatureLayer layer;
        public ListNode(string n, IFeatureLayer fl)
        {
            name = n;
            layer = fl;
            Text = n;
        }
    }
}
