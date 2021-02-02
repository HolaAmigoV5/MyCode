using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ListBoxBinding
{
    public class MainViewModel
    {
        public ObservableCollection<MenuModel> MenuModels { get; set; }

        public MainViewModel()
        {
            MenuModels = new ObservableCollection<MenuModel>();
            MenuModels.Add(new MenuModel() { IconFont = "\xe608", Title = "Live scores"});
            MenuModels.Add(new MenuModel() { IconFont = "\xe620", Title = "Series"});
            MenuModels.Add(new MenuModel() { IconFont = "\xe622", Title = "Teams"});
            MenuModels.Add(new MenuModel() { IconFont = "\xe603", Title = "Features"});
            MenuModels.Add(new MenuModel() { IconFont = "\xe51c", Title = "Videos"});
            MenuModels.Add(new MenuModel() { IconFont = "\xe790", Title = "Stats"});
            MenuModels.Add(new MenuModel() { IconFont = "\xe672", Title = "World cup 2019"});
        }
    }
}
