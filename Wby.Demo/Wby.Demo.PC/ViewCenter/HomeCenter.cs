using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using Wby.Demo.PC.View;
using Wby.Demo.Shared.Common;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// 系统首页
    /// </summary>
    public class HomeCenter : ComponentCenter<HomeView>, IHomeCenter
    {
        public HomeCenter(IHomeViewModel viewModel) : base(viewModel) { }
    }

    /// <summary>
    /// 首页模块
    /// </summary>
    public class HomeViewModel : ObservableObject, IHomeViewModel
    {
        public HomeViewModel()
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "收入",
                    Values = new ChartValues<double> { 5674, 7842, 54648, 28574 ,17973,45000,23000,17829 },
                },
                new LineSeries
                {
                    Title = "支出",
                    Values = new ChartValues<double> { 7346, 15757, 9213, 11435 ,16708,20000,6000,7821,8897 },
                },
                new LineSeries
                {
                    Title = "贷款",
                    Values = new ChartValues<double> { 1200,2341, 13242, 8900, 4351 ,3400,12000,4300,6400 },
                }
            };
            Labels = new[] { "2020-01", "2020-02", "2020-03", "2020-04", "2020-05", "2020-06", "2020-07", "2020-08", "2020-09" };
            YFormatter = value => value.ToString("C");
        }

        public string SelectPageTitle { get; } = "首页";


        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public AsyncRelayCommand<string> ExecuteCommand { get; }

        private ObservableCollection<CommandStruct> toolBarCommandList;
        public ObservableCollection<CommandStruct> ToolBarCommandList
        {
            get { return toolBarCommandList; }
            set { SetProperty(ref toolBarCommandList, value); }
        }
    }
}
