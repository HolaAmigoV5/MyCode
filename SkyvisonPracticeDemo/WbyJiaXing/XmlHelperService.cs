using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WbyJiaXing
{
    public class XmlHelperService
    {
        public XmlNode IsExistNode(XmlDocument document, string xPath)
        {
            return document.SelectSingleNode(xPath);
        }

        public string IsExistsXmlFile(string path, string xmlName)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var xmlPath = path + xmlName;
            if (!File.Exists(xmlPath))
                File.Create(xmlPath).Close();
            return xmlPath;
        }

        public T DeserializeFromXml<T>(string path, Encoding encoding)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamReader reader = new StreamReader(path, encoding))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public XmlDocument LoadXmlDocument(byte[] buf)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream(buf);
            XmlDocument fcDoc = new XmlDocument();
            fcDoc.Load(stream);
            stream.Close();
            return fcDoc;
        }

        public XmlDocument LoadXmlDocument(string xmlInfo)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(xmlInfo);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer);
            XmlDocument oldXmlDoc = new XmlDocument();
            oldXmlDoc.Load(stream);
            stream.Close();
            return oldXmlDoc;
        }
    }
}
