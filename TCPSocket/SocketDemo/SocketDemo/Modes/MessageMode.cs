using System.Collections.Generic;

namespace SocketDemo.Modes
{
    public static class StationsNum 
    {
        /// <summary>
        /// 气象站小数点
        /// </summary>
        public static readonly int[] WeatherNmu = new int[] { 1, 1, 0, 1, 1, 2, 2, 0, 0, 0, 1 };
        /// <summary>
        /// 燃气站小数点
        /// </summary>
        public static readonly int[] GasNmu = new int[] { 0, 0, 0, 1 };
    }

    public enum ModeType
    {
        其它,
        气象,
        燃气
    }

    /// <summary>
    /// 气象
    /// </summary>
    public enum WeatherEnum
    {
        /// <summary>
        /// 温度
        /// </summary>
        temperature = 1,
        /// <summary>
        /// 湿度
        /// </summary>
        humidity = 1,
        /// <summary>
        /// PM2.5
        /// </summary>
        pm25 = 0,
        /// <summary>
        /// 噪声
        /// </summary>
        noise = 1,
        /// <summary>
        /// 一氧化碳
        /// </summary>
        co = 1,
        /// <summary>
        /// 二氧化硫
        /// </summary>
        so2 = 2,
        /// <summary>
        /// 二氧化氮
        /// </summary>
        no2 = 2,
        /// <summary>
        /// 风向
        /// </summary>
        windDirection = 0,
        /// <summary>
        /// 信号强度
        /// </summary>
        signalIntensity = 0,
        /// <summary>
        /// 错误码
        /// </summary>
        errorCode = 0,
        /// <summary>
        /// 版本号
        /// </summary>
        versionNumber = 1
    }

    /// <summary>
    /// 燃气
    /// </summary>
    public enum GasEnum
    {
        /// <summary>
        /// 可燃气
        /// </summary>
        combustibleGas = 0,
        /// <summary>
        /// 信号强度
        /// </summary>
        signalIntensity = 0,
        /// <summary>
        /// 错误码
        /// </summary>
        errorCode = 0,
        /// <summary>
        /// 版本号
        /// </summary>
        versionNumber = 1
    }

    /// <summary>
    /// 报文
    /// </summary>
    public class MessageMode
    {
        /// <summary>
        /// 帧头（2）
        /// </summary>
        public string FrameHeader;
        /// <summary>
        /// 版本（1）
        /// </summary>
        public string VersionStr;
        /// <summary>
        /// 设备ID（6）
        /// </summary>
        public string DeviceID;
        /// <summary>
        /// 传输Session（4）
        /// </summary>
        public string TransmissionSession;
        /// <summary>
        /// 命令字节（1）
        /// </summary>
        public string CommandByte;
        /// <summary>
        /// 长度（2）
        /// </summary>
        public int LengthStr;
        /// <summary>
        /// 内容（n）
        /// </summary>
        public string ContentStr;
        /// <summary>
        /// 数据信息（原始）
        /// </summary>
        public List<string> ContentList;
        /// <summary>
        /// 数据信息（解析后）
        /// </summary>
        public List<string> ContentListNum;
        /// <summary>
        /// 共几条数据
        /// </summary>
        public int ContentNum;
        /// <summary>
        /// 一条数据的长度
        /// </summary>
        public int ContentLength = 4;
        /// <summary>
        /// 校验位（1）
        /// </summary>
        public string Checksum;
    }
}
