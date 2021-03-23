using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Wby.Demo.Shared.Common;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.ViewModel
{
    /// <summary>
    /// 仪表板
    /// </summary>
    public class DashboardViewModel : ObservableObject, IDashboardViewModel
    {
        public string SelectPageTitle { get; } = "仪表板";

        private ObservableCollection<CommandStruct> toolBarCommandList;
        public ObservableCollection<CommandStruct> ToolBarCommandList
        {
            get { return toolBarCommandList; }
            set { SetProperty(ref toolBarCommandList, value); }
        }

        public AsyncRelayCommand<string> ExecuteCommand { get; }
    }
}
