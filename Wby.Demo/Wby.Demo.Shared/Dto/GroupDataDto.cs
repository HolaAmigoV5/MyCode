using System.Collections.Generic;
using System.Collections.ObjectModel;
using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.DataModel;

namespace Wby.Demo.Shared.Dto
{
    public class GroupDataDto : ViewModelBase
    {
        public Group Group { get; set; }
        public List<GroupFunc> GroupFuncs { get; set; }

        private ObservableCollection<GroupUserDto> groupUsers;
        public ObservableCollection<GroupUserDto> GroupUsers
        {
            get { return groupUsers; }
            set { SetProperty(ref groupUsers, value); }
        }

        public GroupDataDto()
        {
            Group = new Group();
            GroupFuncs = new List<GroupFunc>();
            GroupUsers = new ObservableCollection<GroupUserDto>();
        }
    }
}
