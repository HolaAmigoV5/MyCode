using Axi3dRenderEngine;
using CommonLibrary;
using i3dCommon;
using i3dFdeCore;
using i3dMath;
using i3dRenderEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace InteractMode
{
    public partial class Form1 : Form
    {
        private ISkyBox skybox = null;
        private readonly string tmpSkyboxPath = @"C:\Program Files\LunCeTX\SkySceneryX64\skybox\";   //天空盒图片位置（SkyScenery安装位置）
        private Hashtable fcMap = null;             //IFeatureClass, List<string> 存储dataset里featureclass及对应的空间列名
        EulerAngle angle = new EulerAngle();
        AxRenderControl axRenderControl;

        public Form1()
        {
            InitializeComponent();

            // 初始化RenderControl控件
            InitializeRenderControl();

            // 设置默认天空盒
            SetDefaultSkyBox();

            //加载数据
            LoadData();
        }

        // 初始化RenderControl控件
        private void InitializeRenderControl()
        {
            axRenderControl = new AxRenderControl();
            axRenderControl.BeginInit();
            axRenderControl.Dock = DockStyle.Fill;
            tableLayoutPanel1.Controls.Add(axRenderControl);
            axRenderControl.EndInit();

            PropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", i3dRenderSystem.i3dRenderOpenGL);

            //初始化三维窗口。isPlanarTerrain（true:平面地形，false：地球形）, params（配置参数）
            axRenderControl.Initialize(true, ps);
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
        private void LoadData(bool localData = true)
        {
            ConnectionInfo ci = new ConnectionInfo();
            string rootPath = Path.GetFullPath(@"..//..//..");

            if (localData)
            {
                // 加载本地数据
                ci.ConnectionType = i3dConnectionType.i3dConnectionFireBird2x;

                string tmpFDBPath = Path.Combine(rootPath, "data\\3dm\\JD.3DM");
                ci.Database = tmpFDBPath;
            }
            else
            {
                // 加载服务数据
                ci.ConnectionType = i3dConnectionType.i3dConnectionCms7Http;
                ci.Server = "192.168.2.90";
                ci.Port = 8040;
                ci.Database = "point";
            }

            FeatureLayerVisualize(ci, true, "point");

            // 注册地形
            string tmpTedPath = Path.Combine(rootPath, "data\\2021_04_07\\Hebei_aster_gdem_30m.tif");
            axRenderControl.Terrain.RegisterTerrain(tmpTedPath, "");
        }

        private void FeatureLayerVisualize(ConnectionInfo ci, bool needfly, string sourceName)
        {
            try
            {
                IDataSourceFactory dsFactory = new DataSourceFactory();
                IDataSource ds = dsFactory.OpenDataSource(ci);
                string[] setnames = (string[])ds.GetFeatureDatasetNames();
                if (setnames.Length == 0)
                    return;
                IFeatureDataSet dataset = ds.OpenFeatureDataset(setnames[0]);
                string[] fcnames = (string[])dataset.GetNamesByType(i3dDataSetType.i3dDataSetFeatureClassTable);
                if (fcnames.Length == 0)
                    return;
                fcMap = new Hashtable(fcnames.Length);
                foreach (string name in fcnames)
                {
                    IFeatureClass fc = dataset.OpenFeatureClass(name);
                    // 找到空间列字段
                    List<string> geoNames = new List<string>();

                    IFieldInfoCollection fieldinfos = fc.GetFields();
                    for (int i = 0; i < fieldinfos.Count; i++)
                    {
                        IFieldInfo fieldinfo = fieldinfos.Get(i);
                        if (null == fieldinfo)
                            continue;
                        IGeometryDef geometryDef = fieldinfo.GeometryDef;
                        if (null == geometryDef)
                            continue;
                        geoNames.Add(fieldinfo.Name);
                    }
                    fcMap.Add(fc, geoNames);
                }
            }
            catch (COMException ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return;
            }

            // CreateFeautureLayer
            bool hasfly = false;
            foreach (FeatureClass fc in fcMap.Keys)
            {
                List<string> geoNames = (List<string>)fcMap[fc];
                foreach (string geoName in geoNames)
                {
                    if (!geoName.Equals("Geometry"))
                        continue;

                    IFeatureLayer featureLayer = this.axRenderControl.ObjectManager.CreateFeatureLayer(
                    fc, geoName, null, null);

                    if (!hasfly)
                    {
                        IFieldInfoCollection fieldinfos = fc.GetFields();
                        IFieldInfo fieldinfo = fieldinfos.Get(fieldinfos.IndexOf(geoName));
                        IGeometryDef geometryDef = fieldinfo.GeometryDef;
                        IEnvelope env = geometryDef.Envelope;
                        if (env == null || (env.MaxX == 0.0 && env.MaxY == 0.0 && env.MaxZ == 0.0 &&
                            env.MinX == 0.0 && env.MinY == 0.0 && env.MinZ == 0.0))
                            continue;
                        EulerAngle angle = new EulerAngle();
                        angle.Set(0, -20, 0);
                        this.axRenderControl.Camera.LookAt(env.Center, 1000, angle);
                    }
                    hasfly = true;
                }
            }
        }

        private void SetSkyBox(SkyBoxType sky)
        {
            string skyVal = ((int)sky).ToString();
            if (skyVal == "0" || skyVal == "4")
                skyVal = "0" + skyVal;

            // 获取天空盒
            skybox = this.axRenderControl.ObjectManager.GetSkyBox(0);
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBack, Path.Combine(tmpSkyboxPath, skyVal + "_BK.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBottom, Path.Combine(tmpSkyboxPath, skyVal + "_DN.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageFront, Path.Combine(tmpSkyboxPath, skyVal + "_FR.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageLeft, Path.Combine(tmpSkyboxPath, skyVal + "_LF.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageRight, Path.Combine(tmpSkyboxPath, skyVal + "_RT.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageTop, Path.Combine(tmpSkyboxPath, skyVal + "_UP.jpg"));
        }

        #region 交互方式
        private void toolStripNormal_Click(object sender, EventArgs e)
        {
            axRenderControl.InteractMode = i3dInteractMode.i3dInteractNormal;

            Text = "当前处于漫游模式";
        }

        private void toolStripSelect_Click(object sender, EventArgs e)
        {
            this.axRenderControl.InteractMode = i3dInteractMode.i3dInteractSelect;
            this.axRenderControl.MouseSelectObjectMask = i3dMouseSelectObjectMask.i3dSelectAll;
            this.axRenderControl.MouseSelectMode = i3dMouseSelectMode.i3dMouseSelectClick;
            this.axRenderControl.RcMouseClickSelect -= axRenderControl1_RcMouseClickSelect;
            this.axRenderControl.RcMouseClickSelect += axRenderControl1_RcMouseClickSelect;

            this.Text = "当前处于选择模式";
        }

        void axRenderControl1_RcMouseClickSelect(object sender, Axi3dRenderEngine._IRenderControlEvents_RcMouseClickSelectEvent e)
        {
            MessageBox.Show(string.Format("拾取到{0}类型的物体", e.pickResult.Type.ToString()));
        }

        private void toolStripCoordinate_Click(object sender, EventArgs e)
        {
            axRenderControl.InteractMode = i3dInteractMode.i3dInteractMeasurement;
            axRenderControl.MeasurementMode = i3dMeasurementMode.i3dMeasureCoordinate;

            Text = "当前处于拾取坐标模式";
        }

        private void toolStripAerialDistance_Click(object sender, EventArgs e)
        {
            axRenderControl.InteractMode = i3dInteractMode.i3dInteractMeasurement;
            axRenderControl.MeasurementMode = i3dMeasurementMode.i3dMeasureAerialDistance;

            Text = "当前处于直线测距模式";
        }

        private void toolStripHorizontalDistance_Click(object sender, EventArgs e)
        {
            axRenderControl.InteractMode = i3dInteractMode.i3dInteractMeasurement;
            axRenderControl.MeasurementMode = i3dMeasurementMode.i3dMeasureHorizontalDistance;

            Text = "当前处于水平测距模式";
        }

        private void toolStripVerticalDistance_Click(object sender, EventArgs e)
        {
            axRenderControl.InteractMode = i3dInteractMode.i3dInteractMeasurement;
            axRenderControl.MeasurementMode = i3dMeasurementMode.i3dMeasureVerticalDistance;

            Text = "当前处于垂直测距模式";
        }

        private void toolStripGroundDistance_Click(object sender, EventArgs e)
        {
            axRenderControl.InteractMode = i3dInteractMode.i3dInteractMeasurement;
            axRenderControl.MeasurementMode = i3dMeasurementMode.i3dMeasureGroundDistance;

            Text = "当前处于地表距离测量模式";
        }

        private void toolStripArea_Click(object sender, EventArgs e)
        {
            axRenderControl.InteractMode = i3dInteractMode.i3dInteractMeasurement;
            axRenderControl.MeasurementMode = i3dMeasurementMode.i3dMeasureArea;

            Text = "当前处于投影面积测量模式";
        }

        private void toolStripGroundArea_Click(object sender, EventArgs e)
        {
            axRenderControl.InteractMode = i3dInteractMode.i3dInteractMeasurement;
            axRenderControl.MeasurementMode = i3dMeasurementMode.i3dMeasureGroundArea;

            Text = "当前处于地表面积测量模式";
        }

        private void toolStripGroupSightLine_Click(object sender, EventArgs e)
        {
            axRenderControl.InteractMode = i3dInteractMode.i3dInteractMeasurement;
            axRenderControl.MeasurementMode = i3dMeasurementMode.i3dMeasureGroupSightLine;

            Text = "当前处于地形通视分析测量模式";
        }

        private void toolStripWalk_Click(object sender, EventArgs e)
        {
            axRenderControl.InteractMode = i3dInteractMode.i3dInteractWalk;
            Text = "当前处于步行模式";
            axRenderControl.Focus();  //三维控件取得焦点，以便步行模式键盘有效
        }

        private void toolStripDisable_Click(object sender, EventArgs e)
        {
            axRenderControl.InteractMode = i3dInteractMode.i3dInteractDisable;
            Text = "当前处于禁止交互模式";
        }

        private void toolStrip2DMap_Click(object sender, EventArgs e)
        {
            axRenderControl.InteractMode = i3dInteractMode.i3dInteract2DMap;
            axRenderControl.Terrain.FlyTo();
            Text = "当前处于二维地图模式";
        } 
        #endregion
    }
}
