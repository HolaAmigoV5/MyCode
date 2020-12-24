using System;
using System.IO;
using System.Threading.Tasks;

namespace My.Util
{
    /// <summary>
    /// 描述：日志帮助类
    /// 作者：wby 2019/10/25 9:46:59
    /// </summary>
    public static class LogHelper
    {
        /// <summary>
        /// 写入日志到本地TXT文件
        /// </summary>
        /// <param name="log">日志内容</param>
        public static void WriteLog_LocalTxt(string log)
        {
            Task.Run(() =>
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    DateTime.Now.ToCstTime().ToString("yyyy-MM-dd HH:mm:ss"), "_log.txt");
                string logContent = $"{DateTime.Now.ToCstTime().ToString("yyyy-MM-dd HH:mm:ss")}:{log}\r\n";
                File.AppendAllText(filePath, logContent);
            });
        }
    }
}
