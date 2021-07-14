using System;

namespace CommonLibrary
{
    public class GeometryInformation
    {
        public string GeometryColumnType { get; set; }
        public bool HasSpatialIndex { get; set; }
        public bool HasM { get; set; }
        public bool HasZ { get; set; }
        public double MaxM { get; set; }
        public double MinM { get; set; }
        public int AvgNumPoints { get; set; }
        public EnvelopeInformation Envolope { get; set; }
    }
}
