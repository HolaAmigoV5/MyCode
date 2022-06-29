using MySql.Data.MySqlClient;
using SocketDemo.Modes;
using SocketDemo.ServerHelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SocketDemo.Common
{
    public static class CommonMethods
    {
        /// <summary>
        /// byte[]转16进制
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string Read16Str(byte[] msg)
         {
            string content = "";
            foreach (var tag in msg)
            {
                content = content + tag.ToString("X2");
            }
            return content;
        }

        /// <summary>
        /// byte[]转16进制（有空格）
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string Read16StrSpace(byte[] msg)
        {
            int i = 0;
            string content = "";
            foreach (var tag in msg)
            {
                if ((i != 0) && (i % 3 == 0))
                {
                    content = content + tag.ToString("X2") + " ";
                    i = 0;
                }
                else 
                {
                    content = content + tag.ToString("X2");
                    i++;
                }
            }
            return content;
        }

        /// <summary>
        /// byte[]转10进制
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static short Read10Str(byte[] msg)
        {
            byte[] num = new byte[msg.Length];
            for (int i = msg.Length - 1, j = 0; i >= 0; i--, j++)
            {
                num[j] = msg[i];
            }
            return BitConverter.ToInt16(num, 0);
        }

        /// <summary>
        /// 16进制转有符号10进制
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string ReadTo10Str(string msg)
        {
            return Convert.ToInt16(msg, 16).ToString();
        }

        /// <summary>
        /// 16进制转有符号浮点数(1位小数)
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static double ReadToFloatSignedO(string msg) 
        {
            return ((double)Convert.ToInt16(msg, 16)/10);
        }

        /// <summary>
        /// 16进制转有符号浮点数(2位小数)
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static double ReadToFloatSignedT(string msg)
        {
            return ((double)Convert.ToInt16(msg, 16) / 100);
        }

        /// <summary>
        /// 解析报文
        /// </summary>
        /// <returns></returns>
        public static void AnalysisMessage(byte[] msg, string content)
        {
            MessageMode mm = new MessageMode
            {
                FrameHeader = Read16Str(msg.Skip(0).Take(2).ToArray())
            };

            if (mm.FrameHeader.Equals("FEDC"))
            {
                mm.VersionStr = Read16Str(msg.Skip(2).Take(1).ToArray());
                mm.DeviceID = Read16Str(msg.Skip(3).Take(6).ToArray());
                mm.TransmissionSession = Read16Str(msg.Skip(9).Take(4).ToArray());
                mm.CommandByte = Read16Str(msg.Skip(13).Take(1).ToArray());
                mm.LengthStr = Read10Str(msg.Skip(14).Take(2).ToArray());
                mm.ContentStr = Read16StrSpace(msg.Skip(16).Take((int)mm.LengthStr).ToArray());
                mm.Checksum = Read16Str(msg.Skip(16 + (int)mm.LengthStr).Take(1).ToArray());
                mm.ContentNum = mm.LengthStr / mm.ContentLength;

                Console.WriteLine(string.Format("帧头：{0}{8}版本：{1}{8}设备ID：{2}{8}传输Session：{3}{8}命令字节：{4}{8}长度：{5}{8}内容：{6}{8}校验位：{7}", mm.FrameHeader, mm.VersionStr, mm.DeviceID, mm.TransmissionSession, mm.CommandByte, mm.LengthStr, mm.ContentStr, mm.Checksum, Environment.NewLine));

                string[] strArray = mm.ContentStr.Split(' ');

                //存储原始数据
                mm.ContentList = new List<string>(strArray);
                //存储解析后数据
                mm.ContentListNum = new List<string>();

                ModeType res = TcpServerDemo.mySqlHelp.QueryType(mm.DeviceID);

                Console.WriteLine(string.Format("查询ID类型结果：{0}", res));

                if (res == ModeType.燃气)
                {
                    MySqlParameter[] para =  {
                        new MySqlParameter("@deviceID",MySqlDbType.VarChar,12),
                        new MySqlParameter("@combustibleGas",MySqlDbType.Double),
                        new MySqlParameter("@signalIntensity",MySqlDbType.Double),
                        new MySqlParameter("@errorCode",MySqlDbType.Double),
                        new MySqlParameter("@versionNumber",MySqlDbType.Double),
                        new MySqlParameter("@updateTime",MySqlDbType.DateTime)
                    };
                    para[0].Value = mm.DeviceID;
                    for (int i = 0; i < StationsNum.GasNmu.Length; i++)
                    {
                        switch (StationsNum.GasNmu[i])
                        {
                            case 0:
                                mm.ContentListNum.Add(ReadTo10Str(strArray[i]));
                                break;
                            case 1:
                                mm.ContentListNum.Add(ReadToFloatSignedO(strArray[i]).ToString());
                                break;
                            case 2:
                                mm.ContentListNum.Add(ReadToFloatSignedT(strArray[i]).ToString());
                                break;
                        }
                        Console.WriteLine(string.Format("第{0}条数据：{1}", (i + 1), mm.ContentListNum[i]));
                        para[i + 1].Value = mm.ContentListNum[i];
                    }
                    para[5].Value=DateTime.Now;
                    Console.WriteLine(string.Format("【{0}】接收的燃气站的消息正在存入数据库。", DateTime.Now.ToString()));
                    TcpServerDemo.mySqlHelp.InsertData(para, 2);
                }
                else if (res == ModeType.气象)
                {
                    MySqlParameter[] para = new MySqlParameter[13] {
                        new MySqlParameter("@deviceID",MySqlDbType.VarChar,12),
                        new MySqlParameter("@humidity",MySqlDbType.Double),
                        new MySqlParameter("@temperature",MySqlDbType.Double),
                        new MySqlParameter("@pm25",MySqlDbType.Double),
                        new MySqlParameter("@noise",MySqlDbType.Double),
                        new MySqlParameter("@co",MySqlDbType.Double),
                        new MySqlParameter("@so2",MySqlDbType.Double),
                        new MySqlParameter("@no2",MySqlDbType.Double),
                        new MySqlParameter("@windDirection",MySqlDbType.Double),
                        new MySqlParameter("@signalIntensity",MySqlDbType.Double),
                        new MySqlParameter("@errorCode",MySqlDbType.Double),
                        new MySqlParameter("@versionNumber",MySqlDbType.Double),
                        new MySqlParameter("@updateTime",MySqlDbType.DateTime)
                    };
                    para[0].Value = mm.DeviceID;
                    for(int i=0;i< StationsNum.WeatherNmu.Length;i++)
                    {
                        switch (StationsNum.WeatherNmu[i])
                        {
                            case 0:
                                mm.ContentListNum.Add(ReadTo10Str(strArray[i]));
                                break;
                            case 1:
                                mm.ContentListNum.Add(ReadToFloatSignedO(strArray[i]).ToString());
                                break;
                            case 2:
                                mm.ContentListNum.Add(ReadToFloatSignedT(strArray[i]).ToString());
                                break;
                        }
                        Console.WriteLine(string.Format("第{0}条数据：{1}", (i + 1), mm.ContentListNum[i]));
                        para[i + 1].Value = mm.ContentListNum[i];
                    }
                    para[12].Value=DateTime.Now;
                    Console.WriteLine(string.Format("【{0}】接收的气象站的消息正在存入数据库。", DateTime.Now.ToString()));
                    TcpServerDemo.mySqlHelp.InsertData(para, 1);
                }
                else
                {
                    Console.WriteLine("【{0}】解析失败，未找到该设备ID：{1}，接收的数据是：{2}", DateTime.Now.ToString(), mm.DeviceID, content);
                }
            }
            else
            {
                Console.WriteLine("【{0}】解析失败，接收的数据是：{1}", DateTime.Now.ToString(), content);
            }
        }
    }
}
