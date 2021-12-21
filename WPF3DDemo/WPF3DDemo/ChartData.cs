using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF3DDemo
{
    public class ChartData
    {
        /// <summary>
        /// X轴数据
        /// </summary>
        public List<string>? XLabels { get; set; }

        /// <summary>
        /// Y轴数据
        /// </summary>
        public List<string>? ChartValues { get; set; }
    }
}
