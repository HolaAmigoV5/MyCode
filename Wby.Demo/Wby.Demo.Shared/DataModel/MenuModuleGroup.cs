using System.Collections.ObjectModel;

namespace Wby.Demo.Shared.DataModel
{
    public class MenuModuleGroup
    {
        public string MenuCode { get; set; }

        public string MenuName { get; set; }

        public ObservableCollection<MenuModule> Modules { get; set; }
    }

    public class MenuModule
    {
        public string Name { get; set; }

        public int Value { get; set; }
    }
}
