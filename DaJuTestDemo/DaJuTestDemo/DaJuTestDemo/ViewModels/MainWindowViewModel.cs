using DaJuTestDemo.Common;
using DaJuTestDemo.Core;
using I3DMapOperation;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace DaJuTestDemo.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Ctor & Properties
        private IMapOperation mapOperation;
        int playBackSpeedTimes = 1;

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

        private DelegateCommand<string> playTrajectoryDoubleClickCommand;
        public DelegateCommand<string> PlayBackTimeDoubleClickCommand =>
            playTrajectoryDoubleClickCommand ?? (playTrajectoryDoubleClickCommand = new DelegateCommand<string>(ExecutePlayTrajectoryDoubleClickCommand));

        void ExecutePlayTrajectoryDoubleClickCommand(string name)
        {
            mapOperation.StopVehicleTrajectory();
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
            TrajectoryName = new ObservableCollection<string>();
            for (int i = 0; i < 10; i++)
            {
                TrajectoryName.Add($"轨迹{i + 1}");
            }
        }

        private void GeneratePlayBackSpeed()
        {
            PlayBackSpeed = new ObservableCollection<int>();
            for (int i = 1; i < 11; i++)
            {
                PlayBackSpeed.Add(i);
            }
        }

        IList<Trajectory> res;
        MapPointHelper helper; 
        /// <summary>
        /// 轨迹回放
        /// </summary>
        /// <returns></returns>
        private async void GetVehicleTraceInfo(string name)
        {
            try
            {
                if (res == null)
                {
                    string excelName = Environment.CurrentDirectory + $"\\data\\{name}.xlsx";
                    ExcelHelper excelHelper = new ExcelHelper(excelName);
                    string[] fields = { "Time", "Location", "Speed", "Longitude", "Latitude" };
                    res = await excelHelper.ExcelToListAsync<Trajectory>(fields);

                    foreach (var item in res)
                    {
                        item.Longitude -= 0.00092;
                        item.Latitude += 0.00001;
                    }

                    // 显示元素轨迹
                    mapOperation.RenderTrajectory(res);

                    if (helper == null)
                    {
                        //helper = new MapPointHelper(res);
                        //res = helper.LoadDataAndTransfer();
                    }

                    //res = (helper ?? new MapPointHelper()).ReadRoads();
                }

                if (res == null || res.Count == 0)
                    MessageBox.Show("该车辆没有轨迹数据！");
                else
                {
                    //mapOperation.RenderVehicleTrajectory(res, playBackSpeedTimes);
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error(ex, "执行GetVehicleTraceInfo错误");
            }
        }
        #endregion
    }
}
