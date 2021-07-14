using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WbyJiaXing
{
    /// <summary>
    /// 数据源配置 模型
    /// </summary>
    [XmlRoot("DataSourceConfig", IsNullable = false)]
    public class DataSourceConfig
    {
        [XmlElement("CurrentPlanGroup", IsNullable = false)]
        public CurrentPlanGroup CurrentPlanGroup { get; set; }

        [XmlElement("TerrainLayerGroup", IsNullable = false)]
        public TerrainLayerGroup TerrainLayerGroup { get; set; }

        [XmlElement("TileLayerGroup", IsNullable = false)]
        public TileLayerGroup TileLayerGroup { get; set; }

        [XmlElement("TwoDimensionGroup", IsNullable = false)]
        public TwoDimensionGroup TwoDimensionGroup { get; set; }

    }

    public class TwoDimensionGroup
    {
        [XmlElement("TwoDimensionLayerLib", IsNullable = false)]
        public List<CommonCurrentPlanLib> TwoDimensionLayerLib { get; set; }
    }
    public class CurrentPlanGroup
    {
        [XmlElement("CurrentPlanLib", IsNullable = false)]
        public List<CommonCurrentPlanLib> CurrentPlanLib { get; set; }
    }
    public class TerrainLayerGroup
    {
        [XmlElement("TerrainLayerLib", IsNullable = false)]
        public List<CommonCurrentPlanLib> TerrainLayerLib { get; set; }
    }

    public class TileLayerGroup
    {
        [XmlElement("TileLayerLib", IsNullable = false)]
        public List<CommonCurrentPlanLib> TileLayerLib { get; set; }
    }
    public class CommonCurrentPlanLib
    {
        [XmlAttribute(nameof(Id))]
        public string Id { get; set; }

        [XmlAttribute(nameof(Type))]
        public string Type { get; set; }

        [XmlAttribute(nameof(EnumType))]
        public string EnumType { get; set; }

        [XmlAttribute(nameof(Server))]
        public string Server { get; set; }

        [XmlAttribute(nameof(Database))]
        public string Database { get; set; }

        [XmlAttribute(nameof(Port))]
        public string Port { get; set; }

        [XmlAttribute(nameof(Password))]
        public string Password { get; set; }

        [XmlAttribute(nameof(Instance))]
        public string Instance { get; set; }

        [XmlAttribute(nameof(User))]
        public string User { get; set; }

    }
}
