using Axi3dRenderEngine;
using i3dCommon;
using i3dFdeCore;
using i3dFdeGeometry;
using i3dMath;
using i3dRenderEngine;
using i3dResource;
using System;
using System.IO;

namespace CommonMapLib
{
    public class MapOperation
    {
        private AxRenderControl _axRenderControl;
        private ConnectionInfo _ci;
        private IPoint selectPoint;
        private GeometryFactory gfactory;
        private ISpatialCRS crs;

        private readonly string rootPath = Path.GetFullPath(@"../../../data/Model/");
        private const string tmpFDBPath = @"D:\GitHub\MyCode\SkyvisonPracticeDemo\data\3dm\JD.3DM";
        private readonly string tmpSkyboxPath = @"C:\Program Files\LC\SkySceneryX64\skybox\";   //天空盒图片位置（SkyScenery安装位置）

        private bool _CTRL = false;

        /// <summary>
        /// 标记拾取时是否支持“ctrl”键用于多选
        /// </summary>
        public bool CTRL
        {
            get { return _CTRL; }
            set { _CTRL = value; }
        }

        public void InitializationMapControl(AxRenderControl axRenderControl)
        {
            _axRenderControl = axRenderControl;

            // 初始化RenderControl控件
            InitializeRenderControl();

            // 设置天空盒
            SetSkyBox(SkyBoxType.BTXY);

            // 设置连接信息
            SetConnectionInfo();

            // 连接3DM并创建图层
            Open3DMAndCreateFeatureLayer();

            // 设置选择模式
            SetSelectModeAndRegisterEvent();

            SetI3dObjectType();

        }

        private void SetI3dObjectType()
        {
            crs = new CRSFactory().CreateFromWKT(_axRenderControl.GetCurrentCrsWKT()) as ISpatialCRS;
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

        private void InitializeRenderControl()
        {
            PropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", i3dRenderSystem.i3dRenderOpenGL);

            //初始化三维窗口。isPlanarTerrain（true:平面地形，false：地球形）, params（配置参数）
            _axRenderControl.Initialize(true, ps);

            _axRenderControl.Camera.FlyTime = 3;
        }

        private void SetConnectionInfo()
        {
            _ci = new ConnectionInfo
            {
                ConnectionType = i3dConnectionType.i3dConnectionFireBird2x,
                Database = tmpFDBPath
            };
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
                rmp.MaxVisibleDistance = 50000;
                _axRenderControl.Camera.FlyToObject(rmp.Guid, i3dActionCode.i3dActionFlyTo);

                pResFactory = null;
                objManager =null;
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
    }
}
