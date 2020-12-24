using System;
using System.IO;
using System.Xml.Serialization;

namespace My.Util
{
    /// <summary>
    /// 描述： XML文档操作帮助类
    /// 作者：wby 2019/10/25 15:20:46
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// 序列化为XML字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            Type type = obj.GetType();
            MemoryStream ms = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(type);
            try
            {
                //序列化对象
                xml.Serialize(ms, obj);
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }

            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            string resStr = sr.ReadToEnd();

            sr.Dispose();
            ms.Dispose();

            return resStr;
        }
    }
}
