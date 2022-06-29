using SocketDemo.ServerHelp;
using System;
using System.Threading;


namespace SocketDemo
{
    /// <summary>
    /// 主程序
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
#if !DEBUG
            ManualResetEvent blocker = new ManualResetEvent(false);
            StarServer();
            blocker.WaitOne();
#else
            MySQLHelp.MySQLHelp help = new MySQLHelp.MySQLHelp();
            do
            {
                var key = Menu();
                if (key == ConsoleKey.Y)
                {
                    StarServer();
                }
                else if (key == ConsoleKey.N)
                {
                    StopServer();
                }
                //else if (key == ConsoleKey.Q)
                //{
                //    help.StartDatabaseConn();
                //}
                //else if (key == ConsoleKey.S)
                //{
                //    help.StopDatabaseConn();
                //}
                else 
                {
                    break;
                }
            }
            while (true);
#endif
        }

        public static ConsoleKey Menu() 
        {
            Console.WriteLine("请选择操作（Y：开启服务；N：关闭服务）");  //；Q：连接数据库；S：关闭数据库
            var key= Console.ReadKey();
            return key.Key;
        }

        private static void StarServer() 
        {
            Console.WriteLine();
            Console.WriteLine("正在开启服务");
            TcpServerDemo.Start();
        }
        private static void StopServer() 
        {
            Console.WriteLine();
            Console.WriteLine("正在关闭服务");
            TcpServerDemo.Stop();
        }
        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            StopServer();
        }
    }
}