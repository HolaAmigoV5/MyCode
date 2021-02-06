using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace WbyToDo.Models
{
    public class MenuModel : ViewModelBase
    {
        public string IconFont { get; set; }
        public string Title { get; set; }
        public string BackColor { get; set; }
        public bool Display { get; set; } = true;

        private ObservableCollection<TaskInfo> taskInfos = new ObservableCollection<TaskInfo>();
        public ObservableCollection<TaskInfo> TaskInfos
        {
            get { return taskInfos; }
            set { taskInfos = value; RaisePropertyChanged(); }
        }

    }

    public class TaskInfo
    {
        public string Content { get; set; }
    }
}
