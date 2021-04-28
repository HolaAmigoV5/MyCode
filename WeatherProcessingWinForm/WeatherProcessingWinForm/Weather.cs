using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherProcessingWinForm
{
    public class Weather
    {
        public int Serial { get; set; }
        public string DeviceId { get; set; }
        public string Time { get; set; }
        public string AddressID { get; set; }
        public string Lighting { get; set; }
        public string AirPressure { get; set; }
        public string WindDirection { get; set; }
        public string WindSpeed { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }

    }
}
