using System;

namespace Coldairarrow.Util
{
    public static partial class Extention
    {
        /// <summary>
        /// int转ASCII字符
        /// </summary>
        /// <param name="asciiCode"></param>
        /// <returns></returns>
        public static string ToAsciiStr(this int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }
        }

        /// <summary>
        /// jsGetTime转为DateTime
        /// </summary>
        /// <param name="jsGetTime">js中Date.getTime()</param>
        /// <returns></returns>
        public static DateTime ToDateTime_From_JsGetTime(this long jsGetTime)
        {
            DateTime dtStart = new DateTime(1970, 1, 1).ToLocalTime();
            long lTime = long.Parse(jsGetTime + "0000");  //说明下，时间格式为13位后面补加4个"0"，如果时间格式为10位则后面补加7个"0",至于为什么我也不太清楚，也是仿照人家写的代码转换的
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow); //得到转换后的时间

            return dtResult;
        }
    }
}
