using Axi3dRenderEngine;
using i3dCommon;
using i3dFdeCore;
using i3dMath;
using i3dRenderEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;

namespace MapWithContent
{
    public class MapOperation
    {

        public void InitializationMapControl(WindowsFormsHost host)
        {
            // 初始化RenderControl控件
            InitializeRenderControl(host);

            // 获取连接信息
            GetConnectionInfo();

            // 连接3DM并创建图层
            Open3DMAndCreateFeatureLayer();

            // 设置摄像机初始位置
            InitlizedCameraPosition();
        }

        #region 初始化
        private AxRenderControl _axRenderControl;
        private ConnectionInfo _ci;
        string currentDir = Environment.CurrentDirectory;


        private void InitializeRenderControl(WindowsFormsHost host)
        {
            try
            {
                _axRenderControl = new AxRenderControl();
                _axRenderControl.BeginInit();
                host.Child = _axRenderControl;
                _axRenderControl.EndInit();

                var ps = new PropertySet();
                ps.SetProperty("RenderSystem", i3dRenderSystem.i3dRenderOpenGL);

                //初始化三维窗口。isPlanarTerrain（true:平面地形，false：地球形）, params（配置参数）
                _axRenderControl.Initialize(false, ps);
                _axRenderControl.Camera.FlyTime = 3;

                //_axRenderControl
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// 获取连接信息
        /// </summary>
        private void GetConnectionInfo()
        {
            _ci = new ConnectionInfo
            {
                ConnectionType = i3dConnectionType.i3dConnectionFireBird2x,
                Database = Path.Combine(currentDir, "3dmData\\XJJD.3DM")
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
                    return;

                foreach (string name in setnames)
                {
                    var dataSet = ds.OpenFeatureDataset(name);
                    var fcnames = (string[])dataSet.GetNamesByType(i3dDataSetType.i3dDataSetFeatureClassTable);
                    if (fcnames == null || fcnames.Length == 0)
                        continue;

                    foreach (string fcname in fcnames)
                    {
                        ReadFieldAndDrawMap(dataSet, fcname);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                //LoggerHelper.Logger.Error(ex, $"执行{nameof(Open3DMAndCreateFeatureLayer)}错误");
            }
        }

        private void ReadFieldAndDrawMap(IFeatureDataSet dataSet, string fcname)
        {
            var fc = dataSet.OpenFeatureClass(fcname);
            IFeatureLayer featureLayer = _axRenderControl.ObjectManager.CreateFeatureLayer(fc, "Geometry", null, null);
            featureLayer.MaxVisibleDistance = 500000000;
        }


        public void InitlizedCameraPosition(bool flag = false)
        {
            double tilt = flag ? -90 : -33.85;
            SetCameraValues(121.30204548415803, 31.18139334303109, 331.7257668711245, heading: 317.2000527037274, tilt: tilt, isLookAt: false);

            //StopVehicleTrajectory();
        }

        public void SetCameraValues(double x, double y, double z, double heading = 0, double tilt = -60, double roll = 0, double distance = 2000, bool isLookAt = true)
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
        #endregion
    }
}
