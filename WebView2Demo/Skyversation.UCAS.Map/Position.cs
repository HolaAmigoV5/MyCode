using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyversation.UCAS.Map
{
    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Heading { get; set; }
        public double Tilt { get; set; }
        public double Roll { get; set; }
        public double Second { get; set; } = 5;
    }
}
