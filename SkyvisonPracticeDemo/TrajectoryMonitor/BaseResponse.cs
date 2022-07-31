using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrajectoryMonitor
{
    public class BaseResponse
    {
        public string Msg { get; set; }
        public int Code { get; set; }
        public object Data { get; set; }
    }

    public class BaseResponse<T> : BaseResponse
    {
        public new T Data { get; set; }
    }
}
