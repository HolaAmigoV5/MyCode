using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using NLog;
using System;

namespace SocketDemo.ServerHelp
{
    /// <summary>
    /// 接收数据后事件处理
    /// </summary>
    /// <param name="data"></param>
    /// <param name="context"></param>
    public delegate void DataReceivedEventHandler0(IByteBuffer data, IChannelHandlerContext context);
    /// <summary>
    /// 终端连接断开后事件处理
    /// </summary>
    /// <param name="context"></param>
    public delegate void ChannelInactiveEventHandler(IChannelHandlerContext context);
    /// <summary>
    /// 终端连接后事件处理
    /// </summary>
    /// <param name="context"></param>
    public delegate void ChannelActiveEventHandler(IChannelHandlerContext context);
    /// <summary>
    /// 处理TCP发送过来的信息
    /// </summary>
    public class TcpChannelHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        /// <summary>
        /// 日志
        /// </summary>
        Logger log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 收到消息后事件
        /// </summary>
        public event DataReceivedEventHandler0 OnDataReceived0;
        /// <summary>
        /// 连接后事件
        /// </summary>
        public event ChannelActiveEventHandler OnChannelAactived;
        /// <summary>
        /// 连接断开时事件
        /// </summary>
        public event ChannelInactiveEventHandler OnChannelInactived;

        /// <summary>
        /// 处理接收到的消息
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="msg"></param>
        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            //检查是否有支撑数组
            if (msg.HasArray)
            {
                TcpServerDemo.num++;
                OnDataReceived0?.Invoke(msg, ctx);
            }
            else 
            {
                return;
            }
        }
        /// <summary>
        /// 连接后事件
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
            TcpServerDemo.clientNum++;
            OnChannelAactived?.Invoke(context);
        }
        /// <summary>
        /// 连接断开后事件
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            OnChannelInactived?.Invoke(context);
        }
        /// <summary>
        /// 接收消息异常
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            base.ExceptionCaught(context, exception);
            Console.WriteLine(string.Format("【{0}】接收消息异常：{1}", DateTime.Now.ToString(), exception.Message));
            log.Error(exception);
        }
    }
}
