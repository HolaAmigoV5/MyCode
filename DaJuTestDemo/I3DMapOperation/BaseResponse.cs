using System;
using System.Collections.Generic;
using System.Text;

namespace I3DMapOperation
{
    /// <summary>
    /// 响应返回（客户端用）
    /// </summary>
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
