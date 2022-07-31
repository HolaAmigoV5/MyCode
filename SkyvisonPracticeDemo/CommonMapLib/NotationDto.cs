using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMapLib
{
    public class NotationDto
    {
        public string ID { get; set; }
        public ElementType? ElementType { get; set; }
        public RegionEnum? RegionCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Velocity { get; set; }
        public double? Altitude { get; set; }
        public string GpsTime { get; set; }
        public string ElementProperty { get; set; }
        public string IconPath { get; set; }
        public bool IsOffline { get; set; } = true;
    }

    public class Trajectory
    {
        public string VehicleNo { get; set; }
        public string Location { get; set; }
        public double LongitudeWgs84 { get; set; }
        public double LatitudeWgs84 { get; set; }
        public double Altitude { get; set; }
        public DateTime GPSTime { get; set; }
        public double Velocity { get; set; }
        public double Mileage { get; set; }
        public double Direction { get; set; }
        public bool IsOffline { get; set; }
    }

    /// <summary>
    /// 标注类型
    /// </summary>
    public enum ElementType
    {
        /// <summary>
        /// 默认全部
        /// </summary>
        [Description("")]
        Default = 0,

        /// <summary>
        /// 保洁员
        /// </summary>
        [Description("保洁员")]
        Cleaner = 1,

        /// <summary>
        /// 压缩车
        /// </summary>
        [Description("压缩车")]
        DryRubbishVehicle = 11,

        /// <summary>
        /// 无泄漏式压缩车
        /// </summary>
        [Description("无泄漏式压缩车")]
        WetRubbishVehicle = 12,

        /// <summary>
        /// 餐厨车
        /// </summary>
        [Description("餐厨车")]
        KitchenRubbishVehicle = 13,

        /// <summary>
        /// 炮雾车
        /// </summary>
        [Description("炮雾车")]
        SprayFogVehicle = 15,

        /// <summary>
        /// 道路污染清除车
        /// </summary>
        [Description("道路污染清除车")]
        CleanUpVehicle = 16,

        /// <summary>
        /// 洗扫车
        /// </summary>
        [Description("洗扫车")]
        WashSweepVehicle = 17,

        /// <summary>
        /// 吸粪车
        /// </summary>
        [Description("吸粪车")]
        FecalSuctionVehicle = 18,

        /// <summary>
        /// 路面养护车
        /// </summary>
        [Description("路面养护车")]
        MaintenanceVehicle = 19,

        /// <summary>
        /// 拉臂车
        /// </summary>
        [Description("拉臂车")]
        HookliftVehicle = 20,

        /// <summary>
        /// 巡回保洁车
        /// </summary>
        [Description("巡回保洁车")]
        PatrolCleaningVehicle = 21,

        /// <summary>
        /// 8桶桶装车
        /// </summary>
        [Description("8桶桶装车")]
        BucketVehicle = 22,

        /// <summary>
        /// 自卸车
        /// </summary>
        [Description("自卸车")]
        DumpVehicle = 23,

        /// <summary>
        /// 扫路车
        /// </summary>
        [Description("扫路车")]
        SweepRoadVehicle = 24,

        /// <summary>
        /// 高压清洗车
        /// </summary>
        [Description("高压清洗车")]
        HighPressureFlushingVehicle = 25,

        /// <summary>
        /// 清洗车
        /// </summary>
        [Description("清洗车")]
        FlushingVehicle = 26,

        /// <summary>
        /// 三轮电瓶车-单桶
        /// </summary>
        [Description("三轮电瓶车-单桶")]
        TricycleSingle = 27,

        /// <summary>
        /// 三轮电瓶车-双桶
        /// </summary>
        [Description("三轮电瓶车-双桶")]
        TricycleDouble = 28,

        /// <summary>
        /// 公厕
        /// </summary>
        [Description("公厕")]
        PublicToilet = 31,

        /// <summary>
        /// 清运点
        /// </summary>
        [Description("清运点")]
        TransportPoint = 33,

        /// <summary>
        /// 垃圾箱房
        /// </summary>
        [Description("垃圾箱房")]
        GarbageRoom = 34,
    }

    public enum RegionEnum
    {
        /// <summary>
        /// 默认全部
        /// </summary>
        [Description("全部")]
        Default = 0,

        /// <summary>
        /// 盈浦
        /// </summary>
        [Description("盈浦")]
        YingPu = 1,

        /// <summary>
        /// 夏阳
        /// </summary>
        [Description("夏阳")]
        XiaYang = 2,

        /// <summary>
        /// 徐泾
        /// </summary>
        [Description("徐泾")]
        XuJing = 3
    }
}
