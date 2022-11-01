using Axi3dRenderEngine;
using i3dCommon;
using i3dFdeCore;
using i3dFdeGeometry;
using i3dMath;
using i3dRenderEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms.Integration;

namespace Skyversation.UCAS.Map
{
    public partial class MapOperation
    {
        #region Properties & Ctor

        private Guid lineGuid = Guid.Empty;

        private AxRenderControl _axRenderControl;
        private ConnectionInfo _ci;
        private ISpatialCRS _spatialCRS;
        private IGeometryFactory _geoFactory;

        private IVector3 posOffSet;

        private string currentDir = Environment.CurrentDirectory;
        private IVector3 position;
        private IFeatureClass buildingFeatureClass;
        private IFeatureLayer buildingFeatureLayer;
        private List<NotationAndRenderPoint> notationAndRenderPoints;
        private List<NotationAndRenderPoint> locationIconLict;

        private string tmpSkyboxPath;

        public MapOperation()
        {
            notationAndRenderPoints = new List<NotationAndRenderPoint>();
            locationIconLict = new List<NotationAndRenderPoint>();
            _geoFactory = new GeometryFactoryClass();
            posOffSet = new Vector3() { X = 0, Y = 0, Z = 0 };
            position = new Vector3();
            tmpSkyboxPath = Path.GetFullPath(@"./MapData/skybox/");
        }

        #endregion Properties & Ctor

        #region 初始化
        private void BuildRenderControlInWPF(WindowsFormsHost host, bool isPlanarTerrain)
        {
            _axRenderControl = new AxRenderControl();
            _axRenderControl.BeginInit();
            host.Child = _axRenderControl;
            _axRenderControl.EndInit();

            InitializeRenderControl(isPlanarTerrain);
        }

        private void InitializeRenderControl(bool isPlanarTerrain)
        {
            var ps = new PropertySet();
            ps.SetProperty("RenderSystem", i3dRenderSystem.i3dRenderOpenGL);

            //初始化三维窗口。isPlanarTerrain（true:平面地形，false：地球形）, params（配置参数）
            _axRenderControl.Initialize(isPlanarTerrain, ps);
            _axRenderControl.Camera.FlyTime = 3;

            SetSpatialCRS(isPlanarTerrain);

            // 设置天空盒
            //SetSkyBox(SkyBoxType.TMXK);
        }

        private void SetSpatialCRS(bool isPlanarTerrain)
        {
            CRSFactory crsFactory = new();
            _spatialCRS = isPlanarTerrain == false ? crsFactory.CreateWGS84() :
                crsFactory.CreateCRS(i3dCoordinateReferenceSystemType.i3dCrsUnknown) as ISpatialCRS;
        }

        private void Create3dmMapAndSetCameraPosition(string _3dmname)
        {
            // 获取连接信息
            GetConnectionInfo(_3dmname);

            // 连接3DM并创建图层
            Open3DMAndCreateFeatureLayer();


            // 设置摄像机初始位置
            InitlizedCameraPosition();
        }

        private void GetConnectionInfo(string mapname)
        {
            _ci = new ConnectionInfo
            {
                ConnectionType = i3dConnectionType.i3dConnectionFireBird2x,
                Database = Path.GetFullPath($@"./MapData/{mapname}")
            };
        }

        private void Open3DMAndCreateFeatureLayer()
        {
            try
            {
                var dsFactory = new DataSourceFactory();
                var ds = dsFactory.OpenDataSource(_ci);

                string[] setnames = (string[])ds.GetFeatureDatasetNames();
                if (setnames == null || setnames.Length == 0)
                {
                    return;
                }

                foreach (string name in setnames)
                {
                    var dataSet = ds.OpenFeatureDataset(name);
                    var fcnames = (string[])dataSet.GetNamesByType(i3dDataSetType.i3dDataSetFeatureClassTable);
                    if (fcnames == null || fcnames.Length == 0)
                    {
                        continue;
                    }

                    foreach (string fcname in fcnames)
                    {
                        ReadFieldAndDrawMap(dataSet, fcname);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void ReadFieldAndDrawMap(IFeatureDataSet dataSet, string fcname)
        {
            IFeatureClass fc = dataSet.OpenFeatureClass(fcname);
            IFeatureLayer featureLayer = _axRenderControl.ObjectManager.CreateFeatureLayer(fc, "Geometry", null, null);
            if (fc.AliasName == "建筑")
            {
                buildingFeatureClass = fc;
                buildingFeatureLayer = featureLayer;
            }

            featureLayer.MaxVisibleDistance = 500000000;
        }

        #endregion 初始化

        #region IMapOperation

        /// <summary>
        /// 获取当前相机位置
        /// </summary>
        /// <returns></returns>
        public Position GetCameraPosition()
        {
            _axRenderControl.Camera.GetCamera(out IVector3 vector3, out IEulerAngle angle);
            Position position = new() { X = vector3.X, Y = vector3.Y, Z = vector3.Z, Heading = angle.Heading, Tilt = angle.Tilt, Roll = angle.Roll };
            return position;
        }

        public void Dispose()
        {
            Dispose(true);  // 释放非托管和托管资源
            GC.SuppressFinalize(this);  // 通知GC不执行析构函数
        }

        public void InitializationMapControl(WindowsFormsHost host, string _3dmname, bool isPlanarTerrain = false)
        {
            try
            {
                // 初始化RenderControl控件
                BuildRenderControlInWPF(host, isPlanarTerrain);

                SetSelectModeAndRegisterEvent();

                // 创建三维地图并设置相机初始位置
                Create3dmMapAndSetCameraPosition(_3dmname);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 初始化相机位置
        /// </summary>
        /// <param name="pos"></param>
        public void InitlizedCameraPosition(Position pos = null)
        {
            if (pos != null)
                SetCameraValues(pos.X, pos.Y, pos.Z, pos.Heading, pos.Tilt, pos.Roll, 2000, false);
            else
                SetCameraValues(116.24467033505763, 39.9067384367435, 3.2176464665681124, 8.8422702994738636, 0, 0, 2000, false);
        }

        /// <summary>
        /// 保存相机位置到Json文件
        /// </summary>
        /// <param name="pos"></param>
        public void SaveCameraPositionToJson(Position pos)
        {
            try
            {
                var positionJson = Path.Combine(Path.GetFullPath(@"./MapData/"), "position.json");

                if (!File.Exists(positionJson))
                    File.Create(positionJson);

                var str = JsonSerializer.Serialize(pos);
                File.AppendAllText(positionJson, str);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void BuildingVisibility(bool flag)
        {
            if (buildingFeatureLayer != null)
            {
                buildingFeatureLayer.VisibleMask = flag ? i3dViewportMask.i3dViewAllNormalView : i3dViewportMask.i3dViewNone;
            }
        }

        public void SetSkyBox(SkyBoxType sky)
        {
            try
            {
                string skyVal = ((int)sky).ToString();

                // 获取天空盒
                var skybox = _axRenderControl.ObjectManager.GetSkyBox(0);
                skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBack, Path.Combine(tmpSkyboxPath, skyVal + "_BK.jpg"));
                skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBottom, Path.Combine(tmpSkyboxPath, skyVal + "_DN.jpg"));
                skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageFront, Path.Combine(tmpSkyboxPath, skyVal + "_FR.jpg"));
                skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageLeft, Path.Combine(tmpSkyboxPath, skyVal + "_LF.jpg"));
                skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageRight, Path.Combine(tmpSkyboxPath, skyVal + "_RT.jpg"));
                skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageTop, Path.Combine(tmpSkyboxPath, skyVal + "_UP.jpg"));
            }
            catch (FileNotFoundException fe)
            {
                System.Diagnostics.Debug.WriteLine(fe.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void SetSelectModeAndRegisterEvent()
        {
            _axRenderControl.InteractMode = i3dInteractMode.i3dInteractSelect;
            _axRenderControl.MouseSelectObjectMask = i3dMouseSelectObjectMask.i3dSelectRenderGeometry;
            _axRenderControl.MouseSelectMode = i3dMouseSelectMode.i3dMouseSelectClick;
            _axRenderControl.RcMouseClickSelect += axRenderControl1_RcMouseClickSelect;
        }
        #endregion IMapOperation

        #region Private

        private void SetCameraValues(double x, double y, double z, double heading = 25.86, double tilt = -66.65, double roll = 0, double distance = 50, bool isLookAt = true)
        {
            IVector3 position = new Vector3() { X = x, Y = y, Z = z };
            var angle = SetAngle(heading, tilt, roll);
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

        private void axRenderControl1_RcMouseClickSelect(object sender, _IRenderControlEvents_RcMouseClickSelectEvent e)
        {
            try
            {
                if (e.eventSender != i3dMouseSelectMode.i3dMouseSelectClick)
                {
                    return;
                }

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

                        default:
                            break;
                    }

                    if (string.IsNullOrEmpty(id))
                    {
                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        #endregion Private

        #region Dispose

        private bool disposed = false;

        ~MapOperation()
        {
            Dispose(false);  // 释放非托管资源
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // 释放托管代码
                }
                // 释放非托管代码
                _axRenderControl.Dispose();
                _ci = null;
            }
            disposed = true;
        }
        #endregion Dispose
    }
}