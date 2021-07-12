using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace BootstrapperShell.ViewModels
{
    public class CustomViewModel : BindableBase
    {
        private string _title = "My CustomViewModel";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public CustomViewModel()
        {

        }
    }
}
