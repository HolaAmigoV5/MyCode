using System.ComponentModel;

namespace CommonLibrary
{
    /// <summary>
    /// 创建类型
    /// </summary>
    public enum CreateObjType
    {
        [Description("创建文本")]
        CreateLabel,

        [Description("创建模型")]
        CreateRenderModelPoint,

        [Description("创建点")]
        CreateRenderPoint,

        [Description("创建线")]
        CreateRenderPolyline,

        [Description("创建多边形")]
        CreateRenderPolygon,

        [Description("创建POI")]
        CreateRenderPOI,

        [Description("创建固定广告牌")]
        CreateFixedBillboard
    }
}
