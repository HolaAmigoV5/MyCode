using System;
using System.Collections.Generic;

namespace DrawPointToolForMeiDu
{
    public class RoadPoints
    {
        public string type { get; set; } = "FeatureCollection";
        public List<Feature> features { get; set; }
    }

    public class Feature
    {
        public Guid Id { get; set; }
        public string type { get; set; } = "Feature";
        public Property properties { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Property
    {
        public string Gid { get; set; }
        public string Class_Id { get; set; }
        public double Source { get; set; }
        public double Target { get; set; }
        public bool Oneway { get; set; }
        public float Priority { get; set; } = 1;
        public float MaxForwardSpeed { get; set; } = 120;
        public float MaxBackwardSpeed { get; set; } = 120;
    }

    public class Geometry
    {
        public string type { get; set; } = "LineString";
        public List<double[]> coordinates { get; set; }
    }
}
