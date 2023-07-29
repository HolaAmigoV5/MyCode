using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeGrid.Models
{
    public class Score : ObservableObject
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private string? no;
        public string? No
        {
            get { return no; }
            set { SetProperty(ref no, value); }
        }

        private string? tel;
        public string? Tel
        {
            get { return tel; }
            set { SetProperty(ref tel, value); }
        }

        private string? email;
        public string? Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        private double chinaScore;
        public double ChinaScore
        {
            get { return chinaScore; }
            set { SetProperty(ref chinaScore, value); }
        }

        private double mathScore;
        public double MathScore
        {
            get { return mathScore; }
            set { SetProperty(ref mathScore, value); }
        }

        private double englishScore;
        public double EnglishScore
        {
            get { return englishScore; }
            set { SetProperty(ref englishScore, value); }
        }
    }
}
