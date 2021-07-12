using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace WpfTestWithPrism.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private ObservableCollection<string> playBackTime;
        public ObservableCollection<string> PlayBackTime
        {
            get { return playBackTime; }
            set { SetProperty(ref playBackTime, value); }
        }

        private ObservableCollection<int> playBackSpeed;
        public ObservableCollection<int> PlayBackSpeed
        {
            get { return playBackSpeed; }
            set { SetProperty(ref playBackSpeed, value); }
        }

        private void GeneratePlayBackTime()
        {
            PlayBackTime = new ObservableCollection<string>();
            var now = DateTime.Now.Date.AddDays(1);
            for (int i = 0; i < 180; i++)
            {
                now = now.AddDays(-1);
                PlayBackTime.Add(now.ToShortDateString());
            }
        }

        private void GeneratePlayBackSpeed()
        {
            PlayBackSpeed = new ObservableCollection<int>();
            for (int i = 1; i < 33; i++)
            {
                PlayBackSpeed.Add(i);
            }
        }


        private DelegateCommand<string> playBackTimeDoubleClickCommand;
        public DelegateCommand<string> PlayBackTimeDoubleClickCommand =>
            playBackTimeDoubleClickCommand ?? (playBackTimeDoubleClickCommand = new DelegateCommand<string>(ExecutePlayBackTimeDoubleClickCommand));

        void ExecutePlayBackTimeDoubleClickCommand(string parameter)
        {
            System.Windows.MessageBox.Show(parameter);
        }

        private DelegateCommand<object> playBackSpeedDoubleClickCommand;
        public DelegateCommand<object> PlayBackSpeedDoubleClickCommand =>
            playBackSpeedDoubleClickCommand ?? (playBackSpeedDoubleClickCommand = new DelegateCommand<object>(ExecutePlayBackSpeedDoubleClickCommand));

        void ExecutePlayBackSpeedDoubleClickCommand(object parameter)
        {
            System.Windows.MessageBox.Show(parameter.ToString());
        }

        public MainWindowViewModel()
        {
            GeneratePlayBackTime();
            GeneratePlayBackSpeed();
        }
    }
}
