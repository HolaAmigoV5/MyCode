using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TreeGrid
{
    public class UIView : ObservableObject
    {
        private Visibility isVisibility;
        public Visibility IsVisibility
        {
            get { return isVisibility; }
            set { SetProperty(ref isVisibility, value); }
        }

        public UIView()
        {
            isVisibility = Visibility.Collapsed;
        }
    }
}
