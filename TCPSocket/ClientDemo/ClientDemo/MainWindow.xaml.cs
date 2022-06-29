using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
#if !DEBUG
        /// <summary>
        /// 服务器IP
        /// </summary>
        private string serverIP = "120.53.236.107";
#else
        /// <summary>
        /// 服务器IP
        /// </summary>
        private string serverIP = "127.0.0.1";
#endif
        /// <summary>
        /// 服务器端口
        /// </summary>
        private int port = 7450;

        Socket socketClient;

        private bool isConnect = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (connectBtn.Content.Equals("连接服务器"))
            {
                StartConnectServer();
            }
            else
            {
                CloseConnectServer();
            }
        }

        private void ShowMsg(string msg) 
        {
            msgTxt.Text = msgTxt.Text + msg + "\n";
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        private void StartConnectServer() 
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                connectBtn.Content = "关闭连接";
                sendTxt.IsEnabled = true;
                isConnect = true;
            }));
            try
            {
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint point = new IPEndPoint(IPAddress.Parse(serverIP), port);
                socketClient.Connect(point);
                this.Dispatcher.Invoke(new Action(() => ShowMsg(string.Format("连接服务器【{0}:{1}】成功！", serverIP, port))));
                if (isConnect)
                {
                    Thread thStart = new Thread(Receive);
                    thStart.IsBackground = true;
                    thStart.Start();
                }
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(new Action(() =>
                ShowMsg(String.Format("连接服务端异常：{0}", ex.ToString()))
                ));
                CloseConnectServer();
            }
        }

        private void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024 * 1024 * 5];
                    int r = socketClient.Receive(buffer);
                    if (r == 0)
                    {
                        break;
                    }
                    string strMsg = Encoding.UTF8.GetString(buffer, 0, r);
                    this.Dispatcher.Invoke(new Action(() => msgTxt.Text = msgTxt.Text + string.Format("【{0}】接收到了服务端【{1}】发送的消息：{2}", DateTime.Now.ToString(), socketClient.RemoteEndPoint.ToString(), strMsg)));
                }
            }
            catch (Exception ex)
            {
                int i = ex.Message.IndexOf("远程主机强迫关闭了一个现有的连接");
                int j = ex.Message.IndexOf("你的主机中的软件中止了一个已建立的连接。");
                if (j == 0 && socketClient != null) 
                {
                    CloseConnectServer();
                }
                else  if (i == 0 && socketClient != null)
                {
                    this.Dispatcher.Invoke(new
                         Action(() =>
                    ShowMsg(String.Format("已断开和主机的连接。"))
                    ));
                    CloseConnectServer();
                }
                else
                {
                    this.Dispatcher.Invoke(new
                         Action(() =>
                    ShowMsg(String.Format("接收服务端消息异常：{0}", ex.ToString()))
                    ));
                }
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        private void CloseConnectServer()
        {
            this.Dispatcher.Invoke(new Action(() =>
           {
               ShowMsg("关闭连接");
               connectBtn.Content = "连接服务器";
               //sendTxt.IsEnabled = false;
               isConnect = false;
           }));
            socketClient.Close();
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                socketClient.Send(strConvertToByte(sendTxt.Text.Trim().Replace(" ", "")));
                this.Dispatcher.Invoke(new
                         Action(() =>
                    ShowMsg(String.Format("【{0}】向服务器【{1}:{2}】发送消息：{3}", DateTime.Now.ToString(), serverIP, port, sendTxt.Text))
                    ));
                sendTxt.Text = "";
            }
            catch (Exception ex) 
            {
                this.Dispatcher.Invoke(new
                        Action(() =>
                   ShowMsg(String.Format("向服务端发送消息异常：{0}", ex.ToString()))
                   ));
            }
        }

        /// <summary>
        /// 字符串转16进制数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private byte[] strConvertToByte(string str) 
        {
            byte[] bytes = str.Length != 0 && str.Length % 2 == 0 ? new byte[(str.Length / 2)] : new byte[(str.Length / 2) + 1];
            for (var i = 0; i < bytes.Length; i++) 
            {
                bytes[i]=Convert.ToByte(str.Substring(i * 2, 2),16);
            }
            //加"\n"回车符
            bytes[bytes.Length - 1] = 10;
            return bytes; 
        }

    }
}
