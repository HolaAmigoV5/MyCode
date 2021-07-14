using Axi3dRenderEngine;
using i3dCommon;
using i3dFdeCore;
using i3dMath;
using i3dRenderEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace WbyJiaXing
{
    public class RenderControlService
    {
        private AxRenderControl _axRenderControl;
        public RenderXmlParser xmlParser { get; set; } = new RenderXmlParser();
        public IList<IFeatureClass> FeatureClassMap { get; } = new List<IFeatureClass>();
        public IList<IFeatureLayer> FeatureLayers { get; } = new List<IFeatureLayer>();
        private List<IFeatureLayer> buildingFeatureLayers = new List<IFeatureLayer>();
        public IList<IFeatureLayer> BuildingFeatureLayers => buildingFeatureLayers;
        public Hashtable FeatureLayerListMap { get; set; }

        private List<IFeatureLayer> craftFeatureLayers = new List<IFeatureLayer>(); //手工模型
        private List<IFeatureLayer> vegetationFeatureLayers = new List<IFeatureLayer>(); //植被
        private List<IFeatureLayer> roadFeatureLayers = new List<IFeatureLayer>(); //道路
        private List<IFeatureLayer> waterFeatureLayers = new List<IFeatureLayer>(); //水系
        private List<IFeatureLayer> floorFeatureLayers = new List<IFeatureLayer>(); //地面
        private List<IFeatureLayer> restsFeatureLayers = new List<IFeatureLayer>(); //其它

        private List<IFeatureLayer> twoDimensionLayers = new List<IFeatureLayer>(); //二维模型
        private List<IFeatureLayer> sbdq = new List<IFeatureLayer>(); //山边地区
        private List<IFeatureLayer> lsfmq = new List<IFeatureLayer>(); //历史风貌区
        private List<IFeatureLayer> stdq = new List<IFeatureLayer>(); //山体地区
        private List<IFeatureLayer> waterSideArea = new List<IFeatureLayer>(); //水边地区
        private List<IFeatureLayer> zydl = new List<IFeatureLayer>(); //重要道路
        private List<IFeatureLayer> zyfzq = new List<IFeatureLayer>(); //重要发展区
        private List<IFeatureLayer> fw = new List<IFeatureLayer>(); //范围
        private List<IFeatureLayer> sgmxfw = new List<IFeatureLayer>(); //手工模型范围
        private List<IFeatureLayer> kg = new List<IFeatureLayer>(); //控规
        private List<I3DTileLayer> tileLayer = new List<I3DTileLayer>(); //创建倾斜摄影

        public bool hasfly { get; set; } = false;

        /// <summary>
        /// 返回RenderControl
        /// </summary>
        public AxRenderControl RenderControl { get { return _axRenderControl; } }
        public RenderControlService()
        {
            _axRenderControl = new AxRenderControl();
        }
        public bool InitlizeRenderControl()
        {
            try
            {
                IPropertySet param = new PropertySet();
                param.SetProperty("RenderSystem", i3dRenderSystem.i3dRenderOpenGL);
                bool result = _axRenderControl.Initialize(true, param);

                //设置天空盒
                SetDefaultSkyBox(GetCheckedSkyBox());
                //InitRenderLayer();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        /// <summary>
        /// 初始化图层
        /// </summary>
        public void InitRenderLayer()
        {
            var xmlHelperService = new XmlHelperService();
            string wsConfig= Environment.CurrentDirectory + "\\Config\\WSConfig.xml";
            var result = xmlHelperService.DeserializeFromXml<DataSourceConfig>(wsConfig, Encoding.UTF8);

            foreach (var currentPlanLib in result.CurrentPlanGroup.CurrentPlanLib)
                Create3DView(currentPlanLib);
            //foreach (var terrainLayerLib in result.TerrainLayerGroup.TerrainLayerLib)
            //    CreateTerrainView(terrainLayerLib);
            //foreach (var tileLayerLib in result.TileLayerGroup.TileLayerLib)
            //    Create3DTileView(tileLayerLib);
            //foreach (var twoDimensionLayerLib in result.TwoDimensionGroup.TwoDimensionLayerLib)
            //    CreateTwoDimensionView(twoDimensionLayerLib);
        }

        /// <summary>
        /// 创建三维图层
        /// </summary>
        private void Create3DView(CommonCurrentPlanLib currentPlanLib)
        {
            IConnectionInfo ci = new ConnectionInfo();
            ci.Database = currentPlanLib.Database;
            ci.ConnectionType = (i3dConnectionType)Convert.ToInt32(currentPlanLib.EnumType);
            ci.Server = currentPlanLib.Server;
            uint.TryParse(currentPlanLib.Port, out var result);
            ci.Port = result;
            FeatureLayerVisualize(ci, false, true);
        }

        private void FeatureLayerVisualize(IConnectionInfo ci, bool isTwoDimension, bool needfly)
        {
            Hashtable fcMap = new Hashtable();
            try
            {
                IDataSourceFactory dsFactory = new DataSourceFactory();
                IDataSource ds = dsFactory.OpenDataSource(ci);
                string[] setnames = (string[])ds.GetFeatureDatasetNames();
                if (setnames.Length == 0)
                    return;

                foreach (string setname in setnames)
                {
                    IFeatureDataSet dataset = ds.OpenFeatureDataset(setname);
                    string[] fcnames = (string[])dataset.GetNamesByType(i3dDataSetType.i3dDataSetFeatureClassTable);
                    if (fcnames.Length == 0)
                        continue;

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
                            if (fieldinfo.FieldType == i3dFieldType.i3dFieldGeometry)
                            {
                                IGeometryDef geometryDef = fieldinfo.GeometryDef;
                                if (null == geometryDef)
                                    continue;
                                geoNames.Add(fieldinfo.Name);
                            }
                        }

                        fcMap.Add(fc, geoNames);
                    }
                }
            }
            catch (COMException ex)
            {
                MessageBox.Show("打开文件时发生错误：" + ex.Message, "错误");
                return;
            }

            IGeometryRender geoRender = null;
            double maxVisibleDistance = 500000000;
            XmlDocument xmldoc = new XmlDocument();
            if (isTwoDimension)
            {
                var styleXmlPath = ci.Database.Replace("shp", "xml");
                if (File.Exists(styleXmlPath))
                {
                    xmldoc.Load(styleXmlPath);
                }

                geoRender = xmlParser.Xml2GeoRender(xmldoc, ref maxVisibleDistance);
            }

            foreach (IFeatureClass fc in fcMap.Keys)
            {
                List<string> geoNames = (List<string>)fcMap[fc];

                foreach (string geoName in geoNames)
                {
                    if (geoName == "Geometry")
                    {
                        FeatureClassMap.Add(fc);
                        IFeatureLayer featureLayer = RenderControl.ObjectManager.CreateFeatureLayer(
                            fc, geoName, null, geoRender);
                        if (featureLayer == null)
                            continue;


                        FeatureLayerListMap.Add(fc.Name, featureLayer);
                        featureLayer.MaxVisibleDistance = maxVisibleDistance; //设置最大可视距离
                        FeatureLayers.Add(featureLayer);
                        if (isTwoDimension)
                        {
                            twoDimensionLayers.Add(featureLayer);
                            GetFeatureLayersByLayerType(fc.Name).Add(featureLayer);
                        }
                        else
                        {
                            craftFeatureLayers.Add(featureLayer);
                            GetFeatureLayersByLayerType(fc.AliasName).Add(featureLayer);
                        }
                        IEnvelope env = featureLayer.Envelope;
                        if (env == null || (env.MaxX == 0.0 && env.MaxY == 0.0 && env.MaxZ == 0.0 &&
                                            env.MinX == 0.0 && env.MinY == 0.0 && env.MinZ == 0.0))
                            continue;

                        if (!hasfly)
                        {
                            //IEulerAngle angle = new EulerAngle();
                            //angle.Set(0, -20, 0);
                            //this.RenderControl.Camera.LookAt(env.Center, 1000, angle);
                        }

                        hasfly = true;
                    }
                }
            }
        }

        private List<IFeatureLayer> GetFeatureLayersByLayerType(string layerType)
        {
            if (layerType.Contains("现状手工模型"))
                return craftFeatureLayers;
            else if (layerType.Contains("建筑"))
                return buildingFeatureLayers;
            else if (layerType.Contains("植被"))
                return vegetationFeatureLayers;
            else if (layerType.Contains("道路"))
                return roadFeatureLayers;
            else if (layerType.Contains("水系"))
                return waterFeatureLayers;
            else if (layerType.Contains("地面"))
                return floorFeatureLayers;
            else if (layerType.Contains("二维"))
                return twoDimensionLayers;
            else if (layerType.Contains("山边地区"))
                return sbdq;
            else if (layerType.Contains("历史风貌区"))
                return lsfmq;
            else if (layerType.Contains("山体地区"))
                return stdq;
            else if (layerType.Contains("水边地区"))
                return waterSideArea;
            else if (layerType.Contains("重要道路"))
                return zydl;
            else if (layerType.Contains("重要发展区"))
                return zyfzq;
            else if (layerType.Contains("范围"))
                return fw;
            else if (layerType.Contains("手工模型范围"))
                return sgmxfw;
            else if (layerType.Contains("控规"))
                return kg;
            else
                return restsFeatureLayers;
        }

        /// <summary>
        /// 设置view0的默认天空盒
        /// </summary>
        public void SetDefaultSkyBox(XmlElement element)
        {
            string skyboxPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "skybox");
            ISkyBox skybox = _axRenderControl.ObjectManager.GetSkyBox(0);
            var imageBack = "22_BK.jpg";
            var imageBottom = "22_DN.jpg";
            var imageFront = "22_FR.jpg";
            var imageLeft = "22_LF.jpg";
            var imageRight = "22_RT.jpg";
            var imageTop = "22_UP.jpg";
            if (element != null)
            {
                imageBack = element.Attributes["BackImage"].Value;
                imageBottom = element.Attributes["BottomImage"].Value;
                imageFront = element.Attributes["FrontImage"].Value;
                imageLeft = element.Attributes["LeftImage"].Value;
                imageRight = element.Attributes["RightImage"].Value;
                imageTop = element.Attributes["TopImage"].Value;
            }

            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBack, Path.Combine(skyboxPath, imageBack));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageBottom, Path.Combine(skyboxPath, imageBottom));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageFront, Path.Combine(skyboxPath, imageFront));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageLeft, Path.Combine(skyboxPath, imageLeft));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageRight, Path.Combine(skyboxPath, imageRight));
            skybox.SetImagePath(i3dSkyboxImageIndex.i3dSkyboxImageTop, Path.Combine(skyboxPath, imageTop));
        }

        /// <summary>
        /// 获取选中的天空盒
        /// </summary>
        /// <returns></returns>
        private XmlElement GetCheckedSkyBox()
        {
            string skyBoxXmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "skybox",
                "SkyBox.zh-CHS.xml");
            XmlDocument xml = new XmlDocument();
            xml.Load(skyBoxXmlPath);
            var element = (XmlElement)xml.SelectSingleNode("/ListView/item[@IsChecked='true']");
            return element;
        }

    }
}
