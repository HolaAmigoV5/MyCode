using DaJuTestDemo.Common;
using DaJuTestDemo.Core;
using I3DMapOperation;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace DaJuTestDemo.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Ctor & Properties
        private IMapOperation mapOperation;
        int playBackSpeedTimes = 1;
        private static readonly string basePath = Environment.CurrentDirectory;
        private string savePath = basePath;

        private ObservableCollection<string> trajectoryName;
        public ObservableCollection<string> TrajectoryName
        {
            get { return trajectoryName; }
            set { SetProperty(ref trajectoryName, value); }
        }

        private ObservableCollection<int> playBackSpeed;
        public ObservableCollection<int> PlayBackSpeed
        {
            get { return playBackSpeed; }
            set { SetProperty(ref playBackSpeed, value); }
        }

        public MainWindowViewModel()
        {
            mapOperation = ContainerLocator.Current.Resolve<IMapOperation>();

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            GenerateTrajectoryName();
            GeneratePlayBackSpeed();
        }
        #endregion

        #region Command
        public DelegateCommand getCameraValues { get; set; }
        public DelegateCommand GetCameraValues => getCameraValues ??= new DelegateCommand(GetCameraData);

        private void GetCameraData()
        {
            var str = mapOperation.GetCameraPosition();
            LoggerHelper.Logger.Info(str);
        }

        private DelegateCommand _measureDistance;
        public DelegateCommand MeasureDistance =>
            _measureDistance ?? (_measureDistance = new DelegateCommand(ExecuteMeasureDistance));

        void ExecuteMeasureDistance()
        {
            mapOperation.MeasureDistance();
        }

        private DelegateCommand _normalCommand;
        public DelegateCommand NormalCommand =>
            _normalCommand ?? (_normalCommand = new DelegateCommand(ExecuteNormalCommand));

        void ExecuteNormalCommand()
        {
            mapOperation.Normal();
        }

        private DelegateCommand<string> playTrajectoryDoubleClickCommand;
        public DelegateCommand<string> PlayBackTimeDoubleClickCommand =>
            playTrajectoryDoubleClickCommand ?? (playTrajectoryDoubleClickCommand = new DelegateCommand<string>(ExecutePlayTrajectoryDoubleClickCommand));

        void ExecutePlayTrajectoryDoubleClickCommand(string name)
        {
            // todo: 清除地图上所有轨迹
            mapOperation.ClearAllRenderObj();
            if (string.IsNullOrEmpty(name)) return;
            GetVehicleTraceInfo(name);
        }

        private DelegateCommand<object> playBackSpeedDoubleClickCommand;
        public DelegateCommand<object> PlayBackSpeedDoubleClickCommand =>
            playBackSpeedDoubleClickCommand ?? (playBackSpeedDoubleClickCommand = new DelegateCommand<object>(ExecutePlayBackSpeedDoubleClickCommand));

        void ExecutePlayBackSpeedDoubleClickCommand(object parameter)
        {
            if (parameter != null)
            {
                playBackSpeedTimes = (int)parameter;
                if (res != null && res.Count > 0)
                    mapOperation.RenderVehicleTrajectory(res, playBackSpeedTimes, false);
            }
        }

        private DelegateCommand<string> _playOrStopCommand;
        public DelegateCommand<string> PlayOrStopCommand =>
            _playOrStopCommand ?? (_playOrStopCommand = new DelegateCommand<string>(ExecuteCommand));

        void ExecuteCommand(string flag)
        {
            if (flag == "play")
                mapOperation.PlayOrStopVehicleTrajectory(true);
            else
                mapOperation.PlayOrStopVehicleTrajectory(false);
        }
        #endregion

        #region Methods
        private void GenerateTrajectoryName()
        {
            TrajectoryName = new ObservableCollection<string>() { "沪EA0838", "沪ET2601", "沪FA2377", "轨迹1", "沪LS1214", "沪LS1214-2", "沪LS1210" };

            //for (int i = 0; i < 7; i++)
            //{
            //    TrajectoryName.Add($"轨迹{i + 1}");
            //}
        }

        private void GeneratePlayBackSpeed()
        {
            PlayBackSpeed = new ObservableCollection<int>();
            for (int i = 1; i < 11; i++)
            {
                PlayBackSpeed.Add(i);
            }
        }

        List<Trajectory> res = new List<Trajectory>();
        MapPointHelper helper = null;
        /// <summary>
        /// 轨迹回放
        /// </summary>
        /// <returns></returns>
        private async void GetVehicleTraceInfo(string name)
        {
            try
            {

                #region 读取Excel原始轨迹
                //string excelName = Environment.CurrentDirectory + $"\\data\\{name}.xlsx";
                //if (!File.Exists(excelName))
                //{
                //    MessageBox.Show($"名称为【{name}】轨迹数据不存在!");
                //    return;
                //}

                //ExcelHelper excelHelper = new ExcelHelper(excelName);
                //string[] fields = { "GPSTime", "Location", "Velocity", "LongitudeWgs84", "LatitudeWgs84" };
                //res = await excelHelper.ExcelToListAsync<Trajectory>(fields);

                //var json = JsonConvert.SerializeObject(res);
                //_ = Save(json, name);

                //foreach (var item in res)
                //{
                //    item.LongitudeWgs84 -= 0.00092;
                //    item.LatitudeWgs84 += 0.00001;
                //}
                #endregion

                string originalJson = await ReadJson(Path.Combine(basePath, $"data\\{name}.json"));
                if (string.IsNullOrEmpty(originalJson))
                {
                    MessageBox.Show($"名称为【{name}】轨迹数据不存在!");
                    return;
                }


                //string originalJson = await ReadJson(Path.Combine(basePath, $"data\\轨迹1原始.geojson"));
                //var res = JsonConvert.DeserializeObject<List<Trajectory>>(originalJson);

                var res = JsonConvert.DeserializeObject<BaseResponse<VehicleTrajectoryDto>>(originalJson);

                //显示原始轨迹
                //mapOperation.RenderTrajectory(res);

                if (res == null || res.Data == null || res.Data.List == null)
                    return;

                mapOperation.RenderTrajectory(res.Data.List);

                string newJson = await ReadJson(Path.Combine(basePath, $"data\\RebuildGeoJson\\{name}.geojson"));
                List<Trajectory> newTrajectory = new List<Trajectory>();

                if (!string.IsNullOrEmpty(newJson))
                {
                    newTrajectory = JsonConvert.DeserializeObject<List<Trajectory>>(newJson);
                    //foreach (var item in newTrajectory)
                    //{
                    //    item.LongitudeWgs84 += 0.00092;
                    //    item.LatitudeWgs84 -= 0.00001;
                    //}
                }
                else
                {
                    // 开始纠偏，返回纠偏后轨迹并保存到本地
                    //newTrajectory = await (helper ?? new MapPointHelper(res)).LoadDataAndTransfer();

                    newTrajectory = await (helper ?? new MapPointHelper(res.Data.List)).LoadDataAndTransfer();
                    var json = JsonConvert.SerializeObject(newTrajectory);
                    _ = Save(json, name);
                }

                if (newTrajectory != null && newTrajectory.Count > 0)
                    mapOperation.RenderVehicleTrajectory(newTrajectory, playBackSpeedTimes);
                else
                    MessageBox.Show("该车辆没有轨迹数据！");

                //读取路网图并显示
                //var roadData = await (helper ?? new MapPointHelper()).ReadRoadsAndRender();
                //if (roadData != null)
                //{
                //    mapOperation.RenderTrajectory(roadData, 0xAAFF0000, 0xAA000000);
                //    //mapOperation.RenderVehicleTrajectory(roadData, playBackSpeedTimes);
                //}

                //mapOperation.RenderVehicleTrajectory(res, playBackSpeedTimes);

            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error(ex, "执行GetVehicleTraceInfo错误");
            }
        }

        private async Task<bool> Save(string json, string savaName)
        {
            try
            {
                savePath = Path.Combine(basePath, $"data\\RebuildGeoJson\\{savaName}.geojson");
                await File.WriteAllTextAsync(savePath, json);
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error(ex.Message);
                return false;
            }
        }

        private async Task<string> ReadJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            string res = await File.ReadAllTextAsync(filePath);
            return res;
        }
        #endregion
    }
}
