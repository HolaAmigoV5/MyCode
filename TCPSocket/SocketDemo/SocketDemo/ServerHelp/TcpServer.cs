using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using SocketDemo.Modes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SocketDemo.ServerHelp
{
    public class TcpServer
    {
        /// <summary>
        /// 处理客户端连接请求（主工作线程组，设置为1个线程）
        /// </summary>
        IEventLoopGroup connectGroup;
        /// <summary>
        /// 处理与各个客户端连接的IO操作（工作线程组，默认为内核数*2的线程组）
        /// </summary>
        IEventLoopGroup workerGroup;
        /// <summary>
        /// 创建一个Netty服务
        /// </summary>
        ServerBootstrap bootstrap;
        /// <summary>
        /// 绑定端口
        /// </summary>
        IChannel boundChannel;
        /// <summary>
        /// 客户端连接
        /// </summary>
        ConcurrentDictionary<string, IChannel> clientList = new ConcurrentDictionary<string, IChannel>();
#if !DEBUG
        /// <summary>
        /// 默认端口
        /// </summary>
        public int Port { get; set; } = 8380;
#else
        /// <summary>
        /// 默认端口
        /// </summary>
        public int Port { get; set; } = 7450;
#endif
        /// <summary>
        /// 定义一个启动事件
        /// </summary>
        public event Action<IChannel> OnStart;
        /// <summary>
        /// 添加客户端连接
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="bindTo"></param>
        public void AddClient(IChannel channel) 
        {
            TCPIPPort iPEndPoint =new TCPIPPort(channel);
            if (clientList.ContainsKey(iPEndPoint.IpPort))
            {
                return;
            }
            clientList.TryAdd(iPEndPoint.IpPort, channel);
            Console.WriteLine(string.Format("{0}【{1}】客户端【{2}】已连接。", Environment.NewLine, DateTime.Now.ToString(), iPEndPoint.IpPort));
        }
        /// <summary>
        /// 删除客户端连接
        /// </summary>
        /// <param name="bindTo"></param>
        public void DelClient(string key)
        {
            //关闭客户端连接（暂时还不清楚是否能用）
            clientList[key].CloseAsync();
            clientList.Remove(key, out _);
            Console.WriteLine(string.Format("{0}【{1}】客户端【{2}】已下线。", Environment.NewLine, DateTime.Now.ToString(), key));
            Console.WriteLine("当前接收到【" + TcpServerDemo.num + "】条数据");
            Console.WriteLine("当前共接收到【" + TcpServerDemo.clientNum + "】个客户端连接");
        }
        /// <summary>
        /// 查询客户端是否在线
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string GetBindKeyByChannel(IChannel channel)
        {
            foreach (var item in clientList)
            {
                if (item.Value == channel)
                {
                    return item.Key;
                }
            }
            return null;
        }
        /// <summary>
        /// 像指定客户端发送消息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public SendMsg Send(string key, byte[] msg) 
        {
            //新建一个消息对象
            var sMsg = new SendMsg();
            //默认发送成功
            sMsg.IsSuccess = true;
            //选择客户端
            var client = clientList[key];
            if (client == null) 
            {
                sMsg.IsSuccess = false;
                sMsg.ErrMsg = string.Format("{0}【{1}】该设备【{2}】未连接。", Environment.NewLine, DateTime.Now.ToString(), key);
                return sMsg;
            }
            try
            {
                client?.WriteAndFlushAsync(Unpooled.CopiedBuffer(msg));
            }
            catch (Exception ex) 
            {
                sMsg.IsSuccess = false;
                sMsg.ErrMsg = string.Format("{0}【{1}】向客户端【{2}】发送消息失败：{3}", Environment.NewLine, DateTime.Now.ToString(), key, ex.ToString());
                return sMsg;
            }
            return sMsg;
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        public async void Start()
        {
            bootstrap = new ServerBootstrap();
            connectGroup = new MultithreadEventLoopGroup(1);
            workerGroup = new MultithreadEventLoopGroup();
            bootstrap.Group(connectGroup, workerGroup)  //设置主工作和工作线程组
                      .Channel<TcpServerSocketChannel>()    //设置通道模式为TCPSocket
                      .Option(ChannelOption.SoKeepalive, true)  //TCP实时监控服务端和客户端之间的连接
                      .ChildOption(ChannelOption.SoKeepalive, true)
                      //工作线程连接器 是设置了一个管道，服务端主线程所有接收到的信息都会通过这个管道一层层往下传输
                      //同时所有出栈的消息 也要这个管道的所有处理器进行一步步处理
                      .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                      {
                          OnStart?.Invoke(channel);
                      }));
            //启动服务
            boundChannel = await bootstrap.BindAsync(Port);
            Console.WriteLine(string.Format("【{0}】服务已经在【{1}】端口启动。", DateTime.Now.ToString(), Port));
        }
        /// <summary>
        /// 关闭服务
        /// </summary>
        public async void Stop()
        {
            //清除客户端列表
            clientList.Clear();
            //关闭服务
            if (boundChannel != null)
            {
                await boundChannel.CloseAsync();
            }
            if (connectGroup != null)
            {
                await connectGroup.ShutdownGracefullyAsync();
            }
            if (workerGroup != null)
            {
                await workerGroup.ShutdownGracefullyAsync();
            }
            if (bootstrap != null && !bootstrap.Group().IsShutdown)
            {
                await bootstrap.Group().ShutdownGracefullyAsync();
            }
            Console.WriteLine(string.Format("【{0}已关闭服务】。", DateTime.Now.ToString()));
        }
    }
}
