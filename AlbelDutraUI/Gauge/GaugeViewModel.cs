using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Gauge
{
    public class GaugeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private int angle = -85;
        public int Angle
        {
            get { return angle; }
            private set { angle = value; NotifyPropertyChanged("Angle"); }
        }

        private int _value = 0;
        public int Value
        {
            get { return _value; }
            set
            {
                if (value >= 0 && value <= 170)
                {
                    _value = value;
                    Angle = value - 85;
                    NotifyPropertyChanged("Value");
                }
            }
        }
    }
}
