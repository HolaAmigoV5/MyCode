using System;

namespace I3DMapOperation
{
    public class Trajectory
    {
        public string Location { get; set; }
        public double LongitudeWgs84 { get; set; }
        public double LatitudeWgs84 { get; set; }
        public double Altitude { get; set; }
        public DateTime GPSTime { get; set; }
        public double Velocity { get; set; }
        public double Mileage { get; set; }
    }
}
