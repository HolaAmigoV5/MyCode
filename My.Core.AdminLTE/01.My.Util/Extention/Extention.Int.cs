using System;
using System.Collections.Generic;
using System.Text;

namespace My.Util
{
    /// <summary>
    /// 描述：Int扩展
    /// 作者：wby 2019/10/22 16:46:46
    /// </summary>
    public static partial class Extention
    {
        /// <summary>
        /// int转ASCII字符
        /// </summary>
        /// <param name="asciiCode">ASCII码</param>
        /// <returns></returns>
        public static string ToAsciiStr(this int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                ASCIIEncoding aSCII = new ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = aSCII.GetString(byteArray);
                return strCharacter;
            }
            else
                throw new ArgumentException("ASCII Code is not valid.");
        }

        /// <summary>
        /// jsGetTime转为DateTime
        /// </summary>
        /// <param name="jsGetTime">js中Date.getTime()</param>
        /// <returns></returns>
        public static DateTime ToDateTime_From_JsGetTime(this long jsGetTime)
        {
            DateTime dtStart = new DateTime(1970, 1, 1).ToLocalTime();

            //时间格式为13位后面补加4个"0"，如果时间格式为10位则后面补加7个"0"
            long lTime = long.Parse(jsGetTime + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);//得到转换后的时间
            return dtResult;
        }
    }
}
