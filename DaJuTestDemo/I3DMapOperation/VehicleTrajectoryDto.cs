using System;
using System.Collections.Generic;
using System.Text;

namespace I3DMapOperation
{
    public class VehicleTrajectoryDto
    {
        public string VehicleNo { get; set; }
        public string PlateNo { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double MeanVelocity { get; set; }
        public double MaxVelocity { get; set; }
        public List<Trajectory> List { get; set; }
    }
}
