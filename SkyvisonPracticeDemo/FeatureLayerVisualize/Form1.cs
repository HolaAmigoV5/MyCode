using CommonLibrary;
using i3dCommon;
using i3dFdeCore;
using i3dMath;
using i3dRenderEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FeatureLayerVisualize
{
    public partial class Form1 : Form
    {
        private ISkyBox skybox = null;
        private readonly string tmpSkyboxPath = @"C:\Program Files\LunCeTX\SkySceneryX64\skybox\";   //天空盒图片位置（SkyScenery安装位置）
        private Hashtable fcMap = null;             //IFeatureClass, List<string> 存储dataset里featureclass及对应的空间列名
        private Hashtable layerEnvelopeMap = null;  //IFeatureLayer, IEnvelope 存储所有加载的featurelayer及其对应的envelope
        EulerAngle angle = new EulerAngle();

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
            PropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", i3dRenderSystem.i3dRenderOpenGL);

            //初始化三维窗口。isPlanarTerrain（true:平面地形，false：地球形）, params（配置参数）
            this.axRenderControl1.Initialize(true, ps);
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
        private void LoadData(bool localData=true)
        {
            ConnectionInfo ci = new ConnectionInfo();
            string rootPath = Path.GetFullPath(@"..//..//..");

            if (localData)
            {
                // 加载本地数据
                ci.ConnectionType = i3dConnectionType.i3dConnectionFireBird2x;

                string tmpFDBPath = Path.Combine(rootPath, "data\\3dm\\1.3DM");
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
            bool hasfly = !needfly;
            layerEnvelopeMap = new Hashtable();
            foreach (FeatureClass fc in fcMap.Keys)
            {
                List<string> geoNames = (List<string>)fcMap[fc];
                foreach (string geoName in geoNames)
                {
                    IFeatureLayer featureLayer = this.axRenderControl1.ObjectManager.CreateFeatureLayer(
                    fc, geoName, null, null);

                    // 添加节点到界面控件上
                    MyListNode item = new MyListNode(string.Format("{0}_{1}_{2}", sourceName, fc.Name, featureLayer.MaxVisibleDistance.ToString()), featureLayer)
                    {
                        Checked = true
                    };
                    listView1.Items.Add(item);

                    IFieldInfoCollection fieldinfos = fc.GetFields();
                    IFieldInfo fieldinfo = fieldinfos.Get(fieldinfos.IndexOf(geoName));
                    IGeometryDef geometryDef = fieldinfo.GeometryDef;
                    IEnvelope env = geometryDef.Envelope;
                    layerEnvelopeMap.Add(featureLayer, env);
                    if (env == null || (env.MaxX == 0.0 && env.MaxY == 0.0 && env.MaxZ == 0.0 &&
                        env.MinX == 0.0 && env.MinY == 0.0 && env.MinZ == 0.0))
                        continue;

                    // 相机飞入
                    if (!hasfly)
                    {
                        angle.Set(0, -20, 0);
                        this.axRenderControl1.Camera.LookAt(env.Center, 1000, angle);
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
            skybox = this.axRenderControl1.ObjectManager.GetSkyBox(0);
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBack, Path.Combine(tmpSkyboxPath, skyVal + "_BK.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBottom, Path.Combine(tmpSkyboxPath, skyVal + "_DN.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageFront, Path.Combine(tmpSkyboxPath, skyVal + "_FR.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageLeft, Path.Combine(tmpSkyboxPath, skyVal + "_LF.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageRight, Path.Combine(tmpSkyboxPath, skyVal + "_RT.jpg"));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageTop, Path.Combine(tmpSkyboxPath, skyVal + "_UP.jpg"));
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0) return;
            MyListNode item = (MyListNode)this.listView1.SelectedItems[0];
            item.Checked = true;

            //this.axRenderControl1.Camera.LookAtEnvelope(item.layer.Envelope);

            IEnvelope env = (IEnvelope)layerEnvelopeMap[item.layer];
            if (env == null || (env.MaxX == 0.0 && env.MaxY == 0.0 && env.MaxZ == 0.0 &&
                env.MinX == 0.0 && env.MinY == 0.0 && env.MinZ == 0.0))
                return;
            this.axRenderControl1.Camera.LookAt(env.Center, 1000, angle);
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            MyListNode item = (MyListNode)e.Item;
            if (e.Item.Checked)
                item.layer.VisibleMask = i3dViewportMask.i3dViewAllNormalView;
            else
                item.layer.VisibleMask = i3dViewportMask.i3dViewNone;
        }
    }
}
