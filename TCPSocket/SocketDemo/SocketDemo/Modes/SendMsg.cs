using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketDemo.Modes
{
    public class SendMsg
    {
        /// <summary>
        /// 是否发送成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 发送不成功，消息提示
        /// </summary>
        public string? ErrMsg { get; set; }
    }
}
