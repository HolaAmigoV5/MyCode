using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using i3dRenderEngine;
using i3dFdeCore;
using i3dMath;
using i3dCommon;
using i3dFdeGeometry;
using System.ComponentModel;

namespace HelloWorld
{
    public partial class Form1 : Form
    {
        #region 属性和变量
        private Hashtable fcMap = null;     //IFeatureClass, List<string> 存储dataset里featureclass及对应的空间列名
        private IEnvelope env;              //加载数据时，初始化的矩形范围
        private ISpatialCRS datasetCRS = null;
        private IGeometryFactory geoFactory = null;

        private readonly string tmpSkyboxPath = @"C:\Program Files\LunCeTX\SkySceneryX64\skybox\";   //天空盒图片位置
        private ISkyBox skybox = null;
        #endregion
        public Form1()
        {
            InitializeComponent();

            // 初始化RenderControl控件
            var ps = new PropertySet();
            ps.SetProperty("abc", i3dRenderSystem.i3dRenderOpenGL);

            //初始化三维窗口。isPlanarTerrain（true:平面地形，false：地球形）, params（配置参数）
            this.axRenderControl1.Initialize(true, ps);
            this.axRenderControl1.Camera.FlyTime = 1;

            // 注册相机“返回”和“前进”事件
            this.axRenderControl1.RcCameraUndoRedoStatusChanged += new EventHandler(axRenderControl1_RcCameraUndoRedoStatusChanged);

            // 设置天空盒
            SetDefaultSkyBox();

            // 加载FDB场景
            LoadLocalData();
            CreateFeautureLayer();

            // 设置其他属性
            this.btnPause.Enabled = false;
            this.btnStop.Enabled = false;
            this.toolStripComboBoxWeather.SelectedIndex = 0;
            this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
            this.helpProvider1.SetHelpString(this.axRenderControl1, "");
            this.helpProvider1.HelpNamespace = "HelloWorld.html";
        }

        /// <summary>
        /// 设置默认天空盒
        /// </summary>
        private void SetDefaultSkyBox()
        {
            // 获取天空盒
            skybox = this.axRenderControl1.ObjectManager.GetSkyBox(0);
            SetSkyBox(SkyBoxType.JSCX);
        }

        private void LoadLocalData()
        {
            try
            {
                ConnectionInfo ci = new ConnectionInfo
                {
                    ConnectionType = i3dConnectionType.i3dConnectionFireBird2x
                };
                string rootPath = Path.GetFullPath(@"..//..//..");
                string tmpFDBPath = Path.Combine(rootPath, "data\\3dm\\1.3DM");

                ci.Database = tmpFDBPath;
                IDataSourceFactory dsFactory = new DataSourceFactory();
                IDataSource ds = dsFactory.OpenDataSource(ci);
                string[] setnames = (string[])ds.GetFeatureDatasetNames();
                if (setnames.Length == 0)
                    return;
                IFeatureDataSet dataset = ds.OpenFeatureDataset(setnames[0]);
                datasetCRS = dataset.SpatialReference;
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
        }

        private void CreateFeautureLayer()
        {
            bool hasfly = false;
            foreach (FeatureClass fc in fcMap.Keys)
            {
                List<string> geoNames = (List<string>)fcMap[fc];
                foreach (string geoName in geoNames)
                {
                    if (!geoName.Equals("Geometry"))
                        continue;

                    IFeatureLayer featureLayer = this.axRenderControl1.ObjectManager.CreateFeatureLayer(
                    fc, geoName, null, null);

                    if (!hasfly)
                    {
                        IFieldInfoCollection fieldinfos = fc.GetFields();
                        IFieldInfo fieldinfo = fieldinfos.Get(fieldinfos.IndexOf(geoName));
                        IGeometryDef geometryDef = fieldinfo.GeometryDef;
                        env = geometryDef.Envelope;
                        if (env == null || (env.MaxX == 0.0 && env.MaxY == 0.0 && env.MaxZ == 0.0 &&
                            env.MinX == 0.0 && env.MinY == 0.0 && env.MinZ == 0.0))
                            continue;
                        EulerAngle angle = new EulerAngle();
                        angle.Set(0, -20, 0);
                        if (geoFactory == null)
                            geoFactory = new GeometryFactory();
                        IPoint pos = geoFactory.CreatePoint(i3dVertexAttribute.i3dVertexAttributeZ);
                        pos.SpatialCRS = datasetCRS;
                        pos.Position = env.Center;
                        this.axRenderControl1.Camera.LookAt2(pos, 1000, angle);
                    }
                    hasfly = true;
                }
            }
        }

        void axRenderControl1_RcCameraUndoRedoStatusChanged(object sender, EventArgs e)
        {
            this.上一视图ToolStripMenuItem.Enabled = axRenderControl1.Camera.CanUndo;
            this.下一视图ToolStripMenuItem.Enabled = axRenderControl1.Camera.CanRedo;
        }

        // 更换天空盒
        private void skyboxListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.skyboxListView.SelectedItems.Count > 0)
            {
                var skyName = skyboxListView.SelectedItems[0].Text;
                var skyType = GetSkyBoxTypeByDescription(skyName);
                SetSkyBox(skyType);
            }
        }

        // 设置天气
        private void toolStripComboBoxWeather_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (toolStripComboBoxWeather.Text)
            {
                case "晴天":
                    skybox.Weather = i3dWeatherType.i3dWeatherSunShine;
                    break;
                case "小雨":
                    skybox.Weather = i3dWeatherType.i3dWeatherLightRain;
                    break;
                case "中雨":
                    skybox.Weather = i3dWeatherType.i3dWeatherModerateRain;
                    break;
                case "大雨":
                    skybox.Weather = i3dWeatherType.i3dWeatherHeavyRain;
                    break;
                case "小雪":
                    skybox.Weather = i3dWeatherType.i3dWeatherLightSnow;
                    break;
                case "中雪":
                    skybox.Weather = i3dWeatherType.i3dWeatherModerateSnow;
                    break;
                case "大雪":
                    skybox.Weather = i3dWeatherType.i3dWeatherHeavySnow;
                    break;
            }
        }

        // 全屏
        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.axRenderControl1.FullScreen = !this.axRenderControl1.FullScreen;
        }

        // 截图
        private void captureScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.PNG)|*.PNG|Image Files(*.JPG)|*.JPG";
            dlg.DefaultExt = ".bmp";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                bool higquality = false;
                bool b = this.axRenderControl1.ExportManager.ExportImage(dlg.FileName, 1024, 1024, higquality);
                if (!b)
                {
                    MessageBox.Show("Capture Screen Failed, Please check it.");
                }
            }
        }

        // 开启关闭雾效果菜单项
        private void toolStripFog_Click(object sender, EventArgs e)
        {
            bool fogCheck = (sender as ToolStripMenuItem).Checked;
            if (!fogCheck)
            {
                skybox.FogStartDistance = 0;
                skybox.FogEndDistance = 500;
                skybox.FogMode = i3dFogMode.i3dFogLinear;
                toolStripFog.Text = "关闭雾效";
            }
            else
            {
                skybox.FogMode = i3dFogMode.i3dFogNone;
                toolStripFog.Text = "开启雾效";
            }
            (sender as ToolStripMenuItem).Checked = !fogCheck;
        }

        private void 上一视图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.axRenderControl1.Camera.Undo();
        }

        private void 下一视图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.axRenderControl1.Camera.Redo();
        }

        #region Methods
        private void SetSkyBox(SkyBoxType sky)
        {
            string skyVal = ((int)sky).ToString();
            if (skyVal == "0" || skyVal == "4")
                skyVal = "0" + skyVal;

            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBack, Path.Combine(tmpSkyboxPath, skyVal + "_BK.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBottom, Path.Combine(tmpSkyboxPath, skyVal + "_DN.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageFront, Path.Combine(tmpSkyboxPath, skyVal + "_FR.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageLeft, Path.Combine(tmpSkyboxPath, skyVal + "_LF.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageRight, Path.Combine(tmpSkyboxPath, skyVal + "_RT.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageTop, Path.Combine(tmpSkyboxPath, skyVal + "_UP.jpg"));
        }

        /// <summary>
        /// 根据描述信息获取天空盒枚举
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        private SkyBoxType GetSkyBoxTypeByDescription(string description)
        {
            if (list.Count == 0)
                GetEnumList(typeof(SkyBoxType));
            int val = 1;
            foreach (var item in list)
            {
                if (item.Item1 == description)
                {
                    val = item.Item3;
                    break;
                }
            }
            return (SkyBoxType)val;
        }

        // 获取所有枚举描述信息和对应的枚举值
        List<(string, string, int)> list = new List<(string, string, int)>();
        public List<(string, string, int)> GetEnumList(Type enumType)
        {
            //var arr2 = typeof(Operation).GetFields();
            var arr2 = enumType.GetFields();
            for (int i = 1; i < arr2.Length; i++)
            {
                var item = arr2[i];
                DescriptionAttribute[] dd = item.GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[];
                string des = dd[0].Description;
                var name = item.Name;
                //int num = (int)Enum.Parse(typeof(Operation), name);
                int num = (int)Enum.Parse(enumType, name);
                list.Add((des, name, num));
            }
            return list;
        }
        #endregion
    }

    /// <summary>
    /// 天空盒类型枚举
    /// </summary>
    public enum SkyBoxType
    {
        [Description("无")]
        WU = 0,

        [Description("金色晨曦")]
        JSCX = 1,

        [Description("光暗之手")]
        GYZS=2,

        [Description("天马行空")]
        TMXK =4,

        [Description("飘絮人间")]
        PXRJ =7,

        [Description("七彩紫罗")]
        QCZL =9,

        [Description("云中之触")]
        YZZC =10,

        [Description("鲲鹏万里")]
        KPWL =11,

        [Description("血色苍穹")]
        XSCQ =12,

        [Description("白云旋天")]
        BTXY =13,

        [Description("长空破日")]
        CKPR =22,

        [Description("霞光掩影")]
        XGYY =44,

        [Description("混沌沧海")]
        HDCH =99,

        [Description("梦境之末")]
        MJZM =100,

        [Description("玄浑宇宙")]
        XHYZ =120,

        [Description("月神之眼")]
        YSZY =130
    }
}
