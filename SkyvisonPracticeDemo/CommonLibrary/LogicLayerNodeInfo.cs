using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class LogicLayerNodeInfo
    {
        public int layerId;
        public string layerName;

        public LogicLayerNodeInfo(int layerId, string layerName)
        {
            this.layerId = layerId;
            this.layerName = layerName;
        }
    }
}
