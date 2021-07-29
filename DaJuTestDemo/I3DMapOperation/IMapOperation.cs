using System.Collections.Generic;
using System.Windows.Forms.Integration;

namespace I3DMapOperation
{
    public interface IMapOperation
    {
        /// <summary>
        /// 初始化3dm模型
        /// </summary>
        /// <param name="i3dmPath"></param>
        /// <returns></returns>
        void InitializationAxRenderControl(WindowsFormsHost host);

        /// <summary>
        /// 设置飞行时间
        /// </summary>
        /// <param name="flyTime">时间</param>
        void SetFlyTime(double flyTime);

        /// <summary>
        /// 注册点击事件
        /// </summary>
        void RegisterRcSelectEvent();

        /// <summary>
        /// 取消点击事件
        /// </summary>
        void CancelRcSelectEvent();

        /// <summary>
        /// 选取模式
        /// </summary>
        void SetSelectMode(string modeName);

        /// <summary>
        /// 回放车辆历史轨迹
        /// </summary>
        /// <param name="traceInfo">轨迹数据</param>
        /// <param name="speedTimes">轨迹播放倍数</param>
        /// <param name="traceChanged">轨迹是否改变（默认true）</param>
        void RenderVehicleTrajectory(IList<Trajectory> traceInfo, int speedTimes, bool traceChanged = true);

        void RenderTrajectory(IList<Trajectory> traceInfos);

        /// <summary>
        /// 停止播放车辆历史轨迹
        /// </summary>
        void StopVehicleTrajectory();

        /// <summary>
        /// 停止或继续播放轨迹
        /// </summary>
        /// <param name="play"></param>
        void PlayOrStopVehicleTrajectory(bool play);

        string GetCameraPosition();

        /// <summary>
        /// 初始化相机位置
        /// </summary>
        void InitlizedCameraPosition();

        /// <summary>
        /// 回调事件
        /// </summary>
        //event Action<(string, NotationType)> SelectNotationFinished;

        void SetCameraValues(double x, double y, double z, double heading, double tilt, double roll, double distance, bool isLookAt);

        (double X, double Y, double Z) GetCameraPositionVector();
    }
}
