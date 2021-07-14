using Axi3dRenderEngine;
using i3dCommon;
using i3dFdeCore;
using i3dFdeGeometry;
using i3dMath;
using i3dRenderEngine;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

namespace CommonLibrary
{
    public class AxRenderControlOperation
    {
        #region Members and ctors
        private AxRenderControl _axRenderControl = null;
        private ISkyBox skybox = null;
        private readonly string tmpSkyboxPath = @"C:\Program Files\LunCeTX\SkySceneryX64\skybox\";   //天空盒图片位置（SkyScenery安装位置）

        private Dictionary<IFeatureDataSet, List<IFeatureClass>> featureDataSetMapping;
        private Dictionary<IFeatureClass, List<string>> featureClassMapping;
        private Dictionary<IFieldInfo, IGeometryDef> fieldInfoMapping;
        private List<IFeatureClass> featureClasses;
        private List<IFieldInfoCollection> fieldInfoCollections;

        private IDataSourceFactory dsFactory = null;
        ConnectionInfo ci = null;

        private readonly string rootPath = Path.GetFullPath(@"..//..//..");
        private const string tmpFDBPath = @"D:\GitHub\MyCode\SkyvisonPracticeDemo\data\3dm\1.3DM";
        //private i3dObjectType TYPE;

        private ISpatialCRS crs = null;

        public AxRenderControlOperation(AxRenderControl axRenderControl)
        {
            _axRenderControl = axRenderControl;
        } 
        #endregion

        #region Propertys

        private bool _CTRL = false;

        /// <summary>
        /// 标记拾取时是否支持“ctrl”键用于多选
        /// </summary>
        public bool CTRL
        {
            get { return _CTRL; }
            set { _CTRL = value; }
        }

        public event Action<string> CallbackMsg;

        public virtual void RaiseCallbackMsg(string msg)
        {
            CallbackMsg?.Invoke(msg);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool InitializationAxRenderControl(string i3dmPath = tmpFDBPath)
        {
            // 初始化RenderControl控件
            InitializeRenderControl();

            // 设置默认天空盒
            SetDefaultSkyBox();

            // 加载数据
            LoadData(i3dmPath);

            // 图层创建
            FeatureLayerVisualize();
            CreateFeautureLayer();

            return true;
        }

        public void SetI3dObjectType()
        {
            crs = new CRSFactory().CreateFromWKT(_axRenderControl.GetCurrentCrsWKT()) as ISpatialCRS;

            //if (crs.CrsType == i3dCoordinateReferenceSystemType.i3dCrsGeographic)
            //    TYPE = i3dObjectType.i3dObjectTerrain;
            //else if (crs.CrsType == i3dCoordinateReferenceSystemType.i3dCrsProject || crs.CrsType == i3dCoordinateReferenceSystemType.i3dCrsUnknown)
            //    TYPE = i3dObjectType.i3dObjectReferencePlane;
        }

        /// <summary>
        /// 选取模式
        /// </summary>
        public void SetSelectMode(string modeName)
        {
            switch (modeName)
            {
                case "漫游不拾取":
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

        public void SetRenderParamColor(uint color)
        {
            _axRenderControl.SetRenderParam(i3dRenderControlParameters.i3dRenderParamLight0Ambient, color);
        }

        /// <summary>
        /// 取消高亮
        /// </summary>
        public void UnhighlightAll()
        {
            _axRenderControl.FeatureManager.UnhighlightAll();
        }

        /// <summary>
        /// 绑定图层名称到树目录
        /// </summary>
        public MyTreeNode BindDataToCatalogTree()
        {
            MyTreeNode myTreeNode = GetTreeNodes();
            AppendFeatureDataSetToTreeNode(myTreeNode);
            return myTreeNode;
        }

        /// <summary>
        /// 获取树节点
        /// </summary>
        /// <returns></returns>
        public MyTreeNode GetTreeNodes()
        {
            if (ci == null) return null;
            MyTreeNode sourceNode;
            if (ci.ConnectionType == i3dConnectionType.i3dConnectionMySql5x)
                sourceNode = new MyTreeNode(ci.Database + "@" + ci.Server, ci);
            else
                sourceNode = new MyTreeNode(ci.Database, ci);
            return sourceNode;
        }

        /// <summary>
        /// 添加 FeatureDataSet 到树节点
        /// </summary>
        /// <param name="sourceNode"></param>
        public void AppendFeatureDataSetToTreeNode(MyTreeNode sourceNode)
        {
            foreach (IFeatureDataSet dataSet in featureDataSetMapping.Keys)
            {
                TreeNode setNode = new TreeNode(dataSet.Name, 1, 1);
                sourceNode.Nodes.Add(setNode);

                AppendFeatureClassToTreeNode(dataSet, setNode);
            }
        }

        /// <summary>
        /// 添加 FeatureClass 到树节点
        /// </summary>
        /// <param name="featureDataSet"></param>
        /// <param name="node"></param>
        public void AppendFeatureClassToTreeNode(IFeatureDataSet featureDataSet , TreeNode node)
        {
            string[] fcnames = (string[])featureDataSet.GetNamesByType(i3dDataSetType.i3dDataSetFeatureClassTable);
            if (fcnames == null || fcnames.Length == 0) return;

            foreach (string fcname in fcnames)
            {
                TreeNode fcNode = new TreeNode(fcname, 2, 2);
                node.Nodes.Add(fcNode);

                IFeatureClass fc = featureDataSet.OpenFeatureClass(fcname);
                AppendFieldInfoToTreeNode(fc, fcNode);
            }
        }

        /// <summary>
        /// 添加 FieldInfo 到树节点
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="node"></param>
        public void AppendFieldInfoToTreeNode(IFeatureClass featureClass, TreeNode node)
        {
            // 获取属性字段
            IFieldInfoCollection fieldinfos = featureClass.GetFields();
            for (int i = 0; i < fieldinfos.Count; i++)
            {
                IFieldInfo fieldinfo = fieldinfos.Get(i);
                if (fieldinfo == null || fieldinfo.Length == 0)
                    return;

                TreeNode fieldinfoNode = new TreeNode(fieldinfo.Name);
                node.Nodes.Add(fieldinfoNode);
            }
        }

        /// <summary>
        /// 通过连接获取DataSource
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        private IDataSource GetDataSource(IConnectionInfo ci)
        {
            if (dsFactory == null)
                dsFactory = new DataSourceFactory();
            IDataSource ds = dsFactory.OpenDataSource(ci);
            return ds;
        }

        /// <summary>
        /// 连接数据库并获取数据库名
        /// </summary>
        /// <param name="connectionType"></param>
        /// <param name="dcInfor"></param>
        /// <returns></returns>
        public string[] ConnectAndGetDatabaseNames(string connectionType, DabaseConnectionInfo dcInfor)
        {
            try
            {
                switch (connectionType)
                {
                    case "i3dConnectionMySql5x":
                        ci.ConnectionType = i3dConnectionType.i3dConnectionMySql5x;
                        break;
                    case "i3dConnectionFireBird2x":
                        ci.ConnectionType = i3dConnectionType.i3dConnectionFireBird2x;
                        break;
                    case "i3dConnectionSQLite3":
                        ci.ConnectionType = i3dConnectionType.i3dConnectionSQLite3;
                        break;
                    default:
                        break;
                }
                ci.Server = dcInfor.Server;
                ci.Port = dcInfor.Port;
                ci.UserName = dcInfor.UserName;
                ci.Password = dcInfor.PassWord;

                if (TryOpenDatabase(ci))
                {
                    string[] dataBaseNames = (string[])dsFactory.GetDataBaseNames(ci, true);
                    Marshal.ReleaseComObject(ci);
                    return dataBaseNames;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FieldInformation GetFieldInfoByName(string name)
        {
            foreach (IFieldInfoCollection fieldinfos in fieldInfoCollections)
            {
                if (fieldinfos != null && fieldinfos.Count > 0)
                {
                    for (int i = 0; i < fieldinfos.Count; i++)
                    {
                        IFieldInfo field = fieldinfos.Get(i);
                        if (field.Name == name)
                            return TransferFieldInfo(field);
                    }
                }
            }
            return null;
        }

        public List<string> GetFieldNamesByFeatureName(string name)
        {
            List<string> fieldNames = new List<string>();
            IFeatureClass fc = GetFeatureClassByName(name);
            var fields = fc.GetFields();
            for (int i = 0; i < fields.Count; i++)
            {
                fieldNames.Add(fields.Get(i)?.Name);
            }
            return fieldNames;
        }

        public DataTable BuildDataTableByFeatureName(string name, string whereClause = "")
        {
            // add column to DataTable
            IFeatureClass fc = GetFeatureClassByName(name);
            DataTable dt = new DataTable();
            AddColumnToDataTable(dt, fc);

            // Add data to DataTable
            AddDataToDataTable(dt, fc, whereClause);
            return dt;
        }
        #endregion

        #region Private Methods

        #region 初始化
        /// <summary>
        ///  初始化RenderControl控件
        /// </summary>
        private void InitializeRenderControl()
        {
            PropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", i3dRenderSystem.i3dRenderOpenGL);

            //初始化三维窗口。isPlanarTerrain（true:平面地形，false：地球形）, params（配置参数）
            _axRenderControl.Initialize(true, ps);

            _axRenderControl.Camera.FlyTime = 1;
        }

        /// <summary>
        /// 设置默认天空盒
        /// </summary>
        private void SetDefaultSkyBox()
        {
            SetSkyBox(SkyBoxType.JSCX);
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="localData"></param>
        public void LoadData(string i3dmFilePath, bool localData = true)
        {
            ci = new ConnectionInfo();
            if (localData)
            {
                // 加载本地数据
                ci.ConnectionType = i3dConnectionType.i3dConnectionFireBird2x;
                ci.Database = i3dmFilePath;
            }
            else
            {
                // 加载服务数据
                ci.ConnectionType = i3dConnectionType.i3dConnectionCms7Http;
                ci.Server = "192.168.2.90";
                ci.Port = 8040;
                ci.Database = "point";
            }
        }

        /// <summary>
        /// 注册地形
        /// </summary>
        public void RegisterTerrain()
        {
            string tmpTedPath = Path.Combine(rootPath, @"data\2021_04_07\Hebei_aster_gdem_30m.tif");
            _axRenderControl.Terrain.RegisterTerrain(tmpTedPath, "");
        }

        /// <summary>
        /// 注册控件拾取事件
        /// </summary>
        public void RegisterRcSelectEvent()
        {
            _axRenderControl.RcMouseClickSelect += axRenderControl1_RcMouseClickSelect;
            _axRenderControl.RcMouseDragSelect += axRenderControl1_RcMouseDragSelect;
            _axRenderControl.MouseSelectObjectMask = i3dMouseSelectObjectMask.i3dSelectFeatureLayer;
        }

        #region 视频相关
        /// <summary>
        /// 注册视频事件
        /// </summary>
        public void RegisterRcCameraAndVideo()
        {
            // 注册播放事件
            _axRenderControl.RcCameraTourWaypointChanged += _axRenderControl_RcCameraTourWaypointChanged;
            _axRenderControl.RcCameraFlyFinished += _axRenderControl_RcCameraFlyFinished;

            // 注册输出视频事件
            _axRenderControl.RcVideoExportBegin += _axRenderControl_RcVideoExportBegin;
            _axRenderControl.RcVideoExporting += _axRenderControl_RcVideoExporting;
            _axRenderControl.RcVideoExportEnd += _axRenderControl_RcVideoExportEnd;
        }
        // 播放过程中
        private void _axRenderControl_RcCameraTourWaypointChanged(object sender, _IRenderControlEvents_RcCameraTourWaypointChangedEvent e)
        {
            throw new NotImplementedException();
        }

        // 播放结束
        private void _axRenderControl_RcCameraFlyFinished(object sender, _IRenderControlEvents_RcCameraFlyFinishedEvent e)
        {
            throw new NotImplementedException();
        }

        // 开始输出视频
        private void _axRenderControl_RcVideoExportBegin(object sender, _IRenderControlEvents_RcVideoExportBeginEvent e)
        {
            throw new NotImplementedException();
        }

        // 输出视频过程中
        private void _axRenderControl_RcVideoExporting(object sender, _IRenderControlEvents_RcVideoExportingEvent e)
        {
            throw new NotImplementedException();
        }

        // 输出视频结束
        private void _axRenderControl_RcVideoExportEnd(object sender, _IRenderControlEvents_RcVideoExportEndEvent e)
        {
            throw new NotImplementedException();
        } 
        #endregion

        private void FeatureLayerVisualize()
        {
            FeatureDataSetMapToFeatureClass();
            FeatureClassMapToFieldInfor();
            FieldinfoMapToGeometryDef();
        }

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

        public void SetSkyBox(SkyBoxType sky)
        {
            string skyVal = ((int)sky).ToString();
            if (skyVal == "0" || skyVal == "4")
                skyVal = "0" + skyVal;

            // 获取天空盒
            skybox = _axRenderControl.ObjectManager.GetSkyBox(0);
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBack, Path.Combine(tmpSkyboxPath, skyVal + "_BK.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBottom, Path.Combine(tmpSkyboxPath, skyVal + "_DN.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageFront, Path.Combine(tmpSkyboxPath, skyVal + "_FR.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageLeft, Path.Combine(tmpSkyboxPath, skyVal + "_LF.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageRight, Path.Combine(tmpSkyboxPath, skyVal + "_RT.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageTop, Path.Combine(tmpSkyboxPath, skyVal + "_UP.jpg"));
        }

        #endregion

        #region RenderControl事件
        private void axRenderControl1_RcMouseDragSelect(object sender, _IRenderControlEvents_RcMouseDragSelectEvent e)
        {
            FeatureLayerPicks(e.pickResults);
        }

        private void axRenderControl1_RcMouseClickSelect(object sender, _IRenderControlEvents_RcMouseClickSelectEvent e)
        {
            FeatureLayerPick(e.pickResult, e.intersectPoint, e.eventSender);
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

        private void HighlightFeature(IPickResult pr)
        {
            if (pr.Type == i3dObjectType.i3dObjectFeatureLayer)
            {
                IFeatureLayerPickResult flpr = pr as IFeatureLayerPickResult;
                int fid = flpr.FeatureId;
                IFeatureLayer fl = flpr.FeatureLayer;
                foreach (IFeatureClass fc in featureClasses)
                {
                    if (fc.Guid.Equals(fl.FeatureClassId))
                    {
                        _axRenderControl.FeatureManager.HighlightFeature(fc, fid, 0xffff0000);
                    }
                }
            }
        }

        private void FeatureLayerPicks(IPickResultCollection pickResults)
        {
            UnhighlightFeature(pickResults);

            if (pickResults != null && pickResults.Count > 0)
            {
                for (int i = 0; i < pickResults.Count; i++)
                {
                    IPickResult pr = pickResults[i];
                    HighlightFeature(pr);
                }
            }
        }

        private void FeatureLayerPick(IPickResult pickResult, IPoint position, i3dMouseSelectMode selectedModel)
        {
            UnhighlightFeature(pickResult);

            if (selectedModel == i3dMouseSelectMode.i3dMouseSelectClick)
                FeaturePickOrCreate(pickResult, position);
        }

        /// <summary>
        /// 创建类型
        /// </summary>
        public CreateObjType ObjType { get; set; } = CreateObjType.CreateLabel;
        private void FeaturePickOrCreate(IPickResult pr, IPoint position)
        {
            if (pr == null)
                return;

            string msg = string.Empty;
            HighlightFeature(pr);
            switch (pr)
            {
                case ILabelPickResult label:
                    msg = "拾取到" + label.Type + "类型，内容为" + label.Label.Text;
                    break;
                case IRenderModelPointPickResult model:
                    msg = "拾取到" + model.Type + "类型，模型名称为" + model.ModelPoint.ModelName;
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
        public void CreateObjectOnRenderControl(CreateObjType type, IPoint point)
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

            label.Text = "我创建了文本——TestLabel";
            label.Position = point;

            if (textAttribute == null)
                textAttribute = new TextAttribute();

            textAttribute.TextColor = 0xffffff00;
            textAttribute.TextSize = 20;
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


        private IGeometryFactory gfactory = null;
        private IModelPoint fde_modelpoint = null;
        private IRenderModelPoint rmodelpoint = null;
        private void CreateRenderModelPoint(IPoint point)
        {
            // 创建模型
            if (gfactory == null)
                gfactory = new GeometryFactory();

            string tmpOSGPath = Environment.CurrentDirectory + @"\TrashCar\GLJCHE01.osg";
            //string tmpOSGPath = Environment.CurrentDirectory + @"\Apartment\Apartment.osg";
            fde_modelpoint = gfactory.CreateGeometry(i3dGeometryType.i3dGeometryModelPoint, i3dVertexAttribute.i3dVertexAttributeZ) as IModelPoint;
            fde_modelpoint.SpatialCRS = crs;
            fde_modelpoint.SetCoords(point.X, point.Y, point.Z, 0, 0);
            fde_modelpoint.ModelName = tmpOSGPath;

            rmodelpoint = _axRenderControl.ObjectManager.CreateRenderModelPoint(fde_modelpoint, null);
            rmodelpoint.MaxVisibleDistance = double.MaxValue;
            rmodelpoint.MinVisiblePixels = 0;
            IEulerAngle angle = new EulerAngle();
            angle.Set(0, -20, 0);

            _axRenderControl.Camera.LookAt2(point, 100, angle);
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

            pointSymbol = new SimplePointSymbolClass();
            pointSymbol.FillColor = 0xff0000ff;
            pointSymbol.Size = 10;
            rpoint = _axRenderControl.ObjectManager.CreateRenderPoint(fde_point, pointSymbol);

            _axRenderControl.Camera.FlyToObject(rpoint.Guid, i3dActionCode.i3dActionFlyTo);
        }


        private IPolyline fde_polyline = null;
        private IRenderPolyline rpolyline = null;
        private CurveSymbolClass lineSymbol = null;
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

            lineSymbol = new CurveSymbolClass();
            lineSymbol.Color = 0xffff00ff;  // 紫红色

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

        #endregion

        #region BuildDataTableByFeatureName
        private void AddColumnToDataTable(DataTable dt, IFeatureClass fc)
        {
            DataColumn dc;
            if (fc != null)
            {
                IFieldInfoCollection fields = fc.GetFields();
                for (int i = 0; i < fields.Count; i++)
                {
                    IFieldInfo fInfo = fields.Get(i);
                    if (fInfo.FieldType == i3dFieldType.i3dFieldGeometry || fInfo.FieldType == i3dFieldType.i3dFieldBlob || fInfo.Name.ToUpper() == "GROUPID")
                        continue;
                    dc = new DataColumn
                    {
                        ColumnName = fInfo.Name
                    };
                    SetDataColumnType(fInfo.FieldType, dc);
                    dt.Columns.Add(dc);
                }

                // GroupId
                dc = new DataColumn("GroupId")
                {
                    DataType = typeof(int)
                };
                dt.Columns.Add(dc);

                // GroupName
                dc = new DataColumn("GroupName")
                {
                    DataType = typeof(string)
                };
                dt.Columns.Add(dc);
            }
        }

        private void AddDataToDataTable(DataTable dt, IFeatureClass fc, string whereClause)
        {
            IQueryFilter filter = new QueryFilter();
            IFdeCursor cursor;
            try
            {
                filter.PostfixClause = "order by oid desc";
                filter.WhereClause = whereClause;
                cursor = fc.Search(filter, true);
                if (cursor != null)
                {
                    dt.BeginLoadData();
                    IRowBuffer fdeRow;
                    DataRow dr;
                    while ((fdeRow = cursor.NextRow()) != null)
                    {
                        dr = dt.NewRow();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            string colName = dt.Columns[i].ColumnName;
                            int nPos = fdeRow.FieldIndex(colName);
                            if (nPos == -1 || fdeRow.IsNull(nPos))
                                continue;
                            dr[i] = fdeRow.GetValue(nPos);
                        }
                        dt.Rows.Add(dr);
                    }
                    dt.EndLoadData();
                }

                // 通过解析逻辑树获取GroupId对应的GroupName
                GroupId2LayerName(dt, fc.FeatureDataSet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //解析GroupId对应的GroupName
        private void GroupId2LayerName(DataTable dt, IFeatureDataSet dataset)
        {
            List<LogicLayerNodeInfo> infoList = GetLogicLayerInfos(dataset);
            foreach (DataRow dr in dt.Rows)
            {
                string strGrpId = dr["GroupId"].ToString();
                int grpId;
                if (int.TryParse(strGrpId, out grpId))
                {
                    string layerName = FindGroupName(grpId, infoList);
                    dr["GroupName"] = layerName;
                }
            }
        }

        private string FindGroupName(int id, List<LogicLayerNodeInfo> infoList)
        {
            if (infoList == null)
                return string.Empty;
            foreach (LogicLayerNodeInfo logicLayer in infoList)
            {
                if (logicLayer.layerId == id)
                {
                    return logicLayer.layerName;
                }
            }

            return string.Empty;
        }

        // 解析逻辑树xml获取GroupId及其对应的GroupName
        private List<LogicLayerNodeInfo> GetLogicLayerInfos(IFeatureDataSet dataset)
        {
            byte[] bb = GetLogicTreeContent(dataset);
            if (bb == null)
                return null;
            List<LogicLayerNodeInfo> layerList = new List<LogicLayerNodeInfo>();
            using (MemoryStream ms = new MemoryStream(bb))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ms);
                XmlNode rootNode = doc.DocumentElement;
                if (rootNode != null && rootNode.HasChildNodes)
                {
                    TravelXML(rootNode.ChildNodes[0], layerList);
                }
            }
            return layerList;
        }

        // 读数据库获取逻辑树xml内容
        private byte[] GetLogicTreeContent(IFeatureDataSet dataset)
        {
            byte[] strContent = null;
            try
            {
                IQueryDef qd = dataset.DataSource.CreateQueryDef();
                qd.AddSubField("content");

                qd.Tables = new string[] { "cm_logictree", "cm_group" };
                qd.WhereClause = string.Format("cm_group.groupuid = cm_logictree.groupid " + " and cm_group.DataSet = '{0}'", dataset.Name);
                IFdeCursor cursor = qd.Execute(false);

                IRowBuffer row = null;
                if ((row = cursor.NextRow()) != null)
                {
                    //content
                    int nPose = row.FieldIndex("content");
                    if (nPose != -1 || row.IsNull(nPose))
                    {
                        IBinaryBuffer bb = row.GetValue(nPose) as IBinaryBuffer;
                        strContent = (byte[])bb.AsByteArray();
                    }
                }
            }
            catch (COMException ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return null;
            }

            return strContent;
        }

        private void TravelXML(XmlNode pNode, List<LogicLayerNodeInfo> layerList)
        {
            if (pNode == null || layerList == null || pNode.Attributes == null)
                return;

            string layerName = pNode.Attributes["Name"].Value;
            int grpId = int.Parse(pNode.Attributes["ID"].Value);
            LogicLayerNodeInfo info = new LogicLayerNodeInfo(grpId, layerName);
            layerList.Add(info);

            if (pNode.HasChildNodes)
            {
                foreach (XmlNode node in pNode.ChildNodes)
                {
                    TravelXML(node, layerList);
                }
            }
        }

        private void SetDataColumnType(i3dFieldType type, DataColumn dc)
        {
            switch (type)
            {
                case i3dFieldType.i3dFieldInt16:
                    dc.DataType = typeof(short);
                    break;
                case i3dFieldType.i3dFieldInt32:
                case i3dFieldType.i3dFieldFID:
                    dc.DataType = typeof(int);
                    break;
                case i3dFieldType.i3dFieldInt64:
                    dc.DataType = typeof(long);
                    break;
                case i3dFieldType.i3dFieldFloat:
                    dc.DataType = typeof(float);
                    break;
                case i3dFieldType.i3dFieldDouble:
                    dc.DataType = typeof(double);
                    break;
                case i3dFieldType.i3dFieldString:
                case i3dFieldType.i3dFieldUUID:
                    dc.DataType = typeof(string);
                    break;
                case i3dFieldType.i3dFieldDate:
                    dc.DataType = typeof(DateTime);
                    break;
                case i3dFieldType.i3dFieldGeometry:
                    dc.DataType = typeof(object);
                    break;
                //case i3dFieldType.i3dFieldANSIString:
                //    break;
                //case i3dFieldType.i3dFieldBlob:
                //    break;
                //case i3dFieldType.i3dFieldXML:
                //    break;
                //case i3dFieldType.i3dFieldUnknown:
                //    break;
                //case i3dFieldType.i3dFieldInt8:
                //    break;
                default:
                    dc.DataType = typeof(string);
                    break;
            }
        }
        #endregion

        private FieldInformation TransferFieldInfo(IFieldInfo field)
        {
            FieldInformation fieldInfo = new FieldInformation
            {
                Name = field.Name,
                FieldType = field.FieldType.GetType().ToString(),
                IsSystemField = field.IsSystemField,
                Alias = field.Alias,
                DefaultValue = field.DefaultValue,
                Editable = field.Editable,
                Length = field.Length,
                Nullable = field.Nullable,
                RegisteredRenderIndex = field.RegisteredRenderIndex,
                Precision = field.Precision,
                Scale = field.Scale,
                DomainFixed = field.DomainFixed
            };

            var geometryDef = field.GeometryDef;
            GeometryInformation geometryInfo;
            if (geometryDef != null)
            {
                EnvelopeInformation envelopInfo;
                geometryInfo = new GeometryInformation
                {
                    GeometryColumnType = geometryDef.GeometryColumnType.ToString(),
                    HasSpatialIndex = geometryDef.HasSpatialIndex,
                    HasM = geometryDef.HasM,
                    HasZ = geometryDef.HasZ,
                    MaxM = geometryDef.MaxM,
                    MinM = geometryDef.MinM,
                    AvgNumPoints = geometryDef.AvgNumPoints
                };
                if (geometryDef.Envelope != null)
                {
                    envelopInfo = new EnvelopeInformation()
                    {
                        Width = geometryDef.Envelope.Width,
                        Height = geometryDef.Envelope.Height,
                        Depth = geometryDef.Envelope.Depth,
                        MinX = geometryDef.Envelope.MinX,
                        MaxX = geometryDef.Envelope.MaxX,
                        MinY = geometryDef.Envelope.MinY,
                        MaxY = geometryDef.Envelope.MaxY,
                        MinZ = geometryDef.Envelope.MinZ,
                        MaxZ = geometryDef.Envelope.MaxZ
                    };
                    geometryInfo.Envolope = envelopInfo;
                }
                fieldInfo.GeometryDef = geometryInfo;
            }
            if (field.Domain != null)
            {
                DomainInformation domainInfo = new DomainInformation
                {
                    Name = field.Domain.Name,
                    Description = field.Domain.Description,
                    DomainType = field.Domain.DomainType.ToString(),
                    FieldType = field.Domain.ToString(),
                    Owner = field.Domain.Owner
                };
                fieldInfo.Domain = domainInfo;
            }
            return fieldInfo;
        }

        private IFeatureClass GetFeatureClassByName(string name)
        {
            foreach (IFeatureClass item in featureClasses)
            {
                if (item.Name == name)
                    return item;
            }
            return null;
        }

        private bool TryOpenDatabase(IConnectionInfo info)
        {
            try
            {
                IDataSource ds = dsFactory.OpenDataSource(info);
                if (ds != null)
                {
                    Marshal.ReleaseComObject(ds);
                    ds = null;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetInteractMode(i3dInteractMode interactMode, i3dMouseSelectMode mouseSelectMode = i3dMouseSelectMode.i3dMouseSelectClick,
            i3dMouseSelectObjectMask mouseSelectObjectMask = i3dMouseSelectObjectMask.i3dSelectAll)
        {
            _axRenderControl.InteractMode = interactMode;
            _axRenderControl.MouseSelectMode = mouseSelectMode;
            _axRenderControl.MouseSelectObjectMask = mouseSelectObjectMask;
        }
        #endregion
    }
}
