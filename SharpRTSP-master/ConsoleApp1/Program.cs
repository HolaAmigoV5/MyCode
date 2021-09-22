using System;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static Rtsp.RtspListener rtsp_client = null;
        static void Main(string[] args)
        {
            Rtsp.RtspUtils.RegisterUri();

            var rtsp_socket = new Rtsp.RtspTcpTransport("47.110.152.146", 10657);

            // Connect a RTSP Listener to the RTSP Socket (or other Stream) to send RTSP messages and listen for RTSP replies
            rtsp_client = new Rtsp.RtspListener(rtsp_socket);

            rtsp_client.AutoReconnect = false;

            rtsp_client.Start(); // start listening for messages from the server (messages fire the MessageReceived event)


            Rtsp.Messages.RtspRequest msg = new Rtsp.Messages.RtspRequestGetParameter();
            
            msg.RtspUri = new Uri("rtsp://47.110.152.146:10657/JT808://47.110.152.146:25010:1:1:20815821905?pushuuid=1ea56741af6d24596&recvuuid=b982a84ca25d1f864&mode=0");

            while (true)
            {
                rtsp_client.SendMessage(msg);
                Thread.Sleep(1000);
            }
            

            Console.WriteLine("Hello World!");
        }
    }
}
