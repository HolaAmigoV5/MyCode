using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TreeGrid.Models;

namespace TreeGrid
{
    public class MainViewModel : ObservableObject
    {
        private ObservableCollection<Student> students = new();
        public ObservableCollection<Student> Students
        {
            get { return students; }
            set { SetProperty(ref students, value); }
        }

        public MainViewModel()
        {
            for (int i = 0; i < 10; i++)
            {
                var student = new Student
                {
                    Id = i + 1,
                    Name = "Tom" + i.ToString(),
                    Sex = i % 2 == 0 ? SexType.女 : SexType.男,
                    Age = i + 25,

                    Details = new ObservableCollection<Score>()
                };


                for (int j = 0; j < 5; j++)
                {
                    var score = new Score
                    {
                        Id = j + 1,
                        No = "0805" + i.ToString("0000"),
                        Tel = "1871050" + j.ToString("0000"),
                        Email = "5844" + j.ToString("0000") + "qq.com",
                        ChinaScore = j + 55,
                        MathScore = j + 77,
                        EnglishScore = j + 99
                    };
                    student.Details.Add(score);
                }
                students.Add(student);
            }
        }

        public void DetailExpanded(int index)
        {
            if (index >= 0)
            {
                students[index].IsVisibility = Visibility.Visible;
            }
        }

        public void DetailCollapsed(int index)
        {
            if (index >= 0)
            {
                students[index].IsVisibility = Visibility.Collapsed;
            }
        }
    }
}
