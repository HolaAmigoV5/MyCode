using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using WbyToDo.Models;

namespace WbyToDo.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            menuModels = new ObservableCollection<MenuModel>() {
                new MenuModel(){ IconFont="\xe635", Title="我的一天", BackColor="#218868", Display=false},
                new MenuModel(){ IconFont="\xe6b6", Title="重要", BackColor="#EE3B3B"},
                new MenuModel(){ IconFont="\xe6e1", Title="已计划日程", BackColor="#5d6b99"},
                new MenuModel(){ IconFont="\xe614", Title="已分配给我", BackColor="#ad6227"},
                new MenuModel(){ IconFont="\xe755", Title="任务", BackColor="#D7BDE2"}
            };

            menuModel = menuModels[0];
            SelectedCommand = new RelayCommand<MenuModel>(t => Select(t));
            SelectedTaskCommand = new RelayCommand<TaskInfo>(t => SelectedTask(t));
        }

        private ObservableCollection<MenuModel> menuModels;

        public ObservableCollection<MenuModel> MenuModels
        {
            get { return menuModels; }
            set { menuModels = value; RaisePropertyChanged(); }
        }

        private MenuModel menuModel;

        public MenuModel MenuModel
        {
            get { return menuModel; }
            set { menuModel = value; RaisePropertyChanged(); }
        }

        private TaskInfo taskInfo;

        public TaskInfo TaskInfo
        {
            get { return taskInfo; }
            set { taskInfo = value;  RaisePropertyChanged();}
        }


        public RelayCommand<MenuModel> SelectedCommand { get; set; }

        public RelayCommand<TaskInfo> SelectedTaskCommand { get; set; }

        private void Select(MenuModel model)
        {
            MenuModel = model;
        }

        private void SelectedTask(TaskInfo task)
        {
            TaskInfo = task;
            Messenger.Default.Send(task, "Expand");
        }

        public void AddTaskInfo(string content)
        {
            MenuModel.TaskInfos.Add(new TaskInfo() { Content = content });
        }
    }
}