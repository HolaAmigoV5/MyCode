using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeGrid.Models
{
    public class Student : UIView
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private string? name;
        public string? Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private SexType sex;
        public SexType Sex
        {
            get { return sex; }
            set { SetProperty(ref sex, value); }
        }

        private int age;
        public int Age
        {
            get { return age; }
            set { SetProperty(ref age, value); }
        }

        public ObservableCollection<Score>? Details { get; set; }
    }
}
