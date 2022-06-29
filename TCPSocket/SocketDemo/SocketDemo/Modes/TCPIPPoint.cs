using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocketDemo.Modes
{
    public class TCPIPPort
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// IP地址:端口
        /// </summary>
        public string IpPort { get; set; }

        public TCPIPPort(IChannel channel)
        {
            IPEndPoint iPEndPoint = (IPEndPoint)channel.RemoteAddress;
            IP = iPEndPoint.Address.ToString();
            Port = iPEndPoint.Port;
            IpPort = IP + ":" + Port;
        }
    }
}
