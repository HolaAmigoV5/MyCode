using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Wby.Demo.Shared.DataInterfaces;

namespace Wby.Demo.Shared.Dto
{
    public class MenuModuleGroupDto
    {
        public string MenuCode { get; set; }
        public string MenuName { get; set; }
        public ObservableCollection<MenuModuleDto> Modules { get; set; }
    }

    public class MenuModuleDto : ViewModelBase
    {
        public string Name { get; set; }

        private int _value;
        public int Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value); }
        }
    }
}
