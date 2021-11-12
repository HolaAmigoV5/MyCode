using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapWithContent
{
    public class MainWindowViewModel : DependencyObject
    {
        private DelegateCommand _btnCom;
        public DelegateCommand BtnCom =>
            _btnCom ?? (_btnCom = new DelegateCommand(ExecuteCommandName));

        void ExecuteCommandName()
        {
            MessageBox.Show("AAA");
            Name = "BBB";
        }



        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata("ABC"));


    }
}
