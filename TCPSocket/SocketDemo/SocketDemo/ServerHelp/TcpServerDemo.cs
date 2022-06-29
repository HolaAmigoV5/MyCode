using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using MySql.Data.MySqlClient;
using SocketDemo.Common;
using SocketDemo.Modes;
using System;

namespace SocketDemo.ServerHelp
{
    public static class TcpServerDemo
    {
        public static TcpServer tcpServer = new TcpServer();
        public static MySQLHelp.MySQLHelp mySqlHelp = new MySQLHelp.MySQLHelp();
        public static int num = 0;
        public static int clientNum = 0;
        static TcpServerDemo() 
        {
            tcpServer.OnStart += TcpServer_OnStart;
        }
        /// <summary>
        /// 启动
        /// </summary>
        public static void Start() 
        {
            Console.WriteLine(string.Format("【{0}】正在启动服务……", DateTime.Now.ToString()));
            tcpServer.Start();
        }
        /// <summary>
        /// 获取端口
        /// </summary>
        /// <returns></returns>
        public static int GetPort()
        {
            return tcpServer.Port;
        }
        /// <summary>
        /// 关闭
        /// </summary>
        public static void Stop()
        {
            Console.WriteLine(string.Format("【{0}】正在关闭服务……", DateTime.Now.ToString()));
            tcpServer.Stop();
        }
        public static void TcpServer_OnStart(IChannel obj)
        {
            var handler = new TcpChannelHandler();
            handler.OnDataReceived0 += Handler_OnDataReceived0;
            handler.OnChannelAactived += Handler_OnChannelAactived;
            handler.OnChannelInactived += Handler_OnChannelInactived;
#if !DEBUG
            
#else
            //防止粘包断包
            obj.Pipeline.AddLast(new LineBasedFrameDecoder(1024));
#endif
            obj.Pipeline.AddLast(handler);
        }
        /// <summary>
        /// 断开连接后事件
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void Handler_OnChannelInactived(IChannelHandlerContext context)
        {
            var key = tcpServer.GetBindKeyByChannel(context.Channel);
            if (key != null)
            {
                tcpServer.DelClient(key);
            }
            else 
            {
                Console.WriteLine(string.Format("【{0}】未查找到该客户端【{1}】。", DateTime.Now.ToString(), new TCPIPPort(context.Channel).IpPort));
            }
        }
        /// <summary>
        /// 连接后事件
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void Handler_OnChannelAactived(IChannelHandlerContext context)
        {
            if (context.Channel != null) 
            {
                tcpServer.AddClient(context.Channel);
            }
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void Handler_OnDataReceived0(IByteBuffer data, IChannelHandlerContext context)
        {
            if (data.HasArray)
            {
                //没有数据就不进行处理
                if (data.ReadableBytes <= 0)
                    return;
                //有数据才解析
                string content = "";
                byte[] bytes = new byte[data.ReadableBytes];
                for (int i = 0; i < data.ReadableBytes; i++)
                {
                    bytes[i] = (data.Array)[i];
                    content = content + ((data.Array)[i]).ToString("X2") + " ";
                }
                Console.WriteLine(string.Format("{0}【{1}】服务端接收客户端【{2}】的消息：{3}", Environment.NewLine, DateTime.Now.ToString(), new TCPIPPort(context.Channel).IpPort, content));
                CommonMethods.AnalysisMessage(bytes, content);
            }
        }
    }
}
