using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleXml
{
    class Program
    {
        static void Main(string[] args)
        {
            //XmlReaderDemo();
            //XmlWriteDemo();
            //XmlDocumentDemo();
            //UpdateXmlDocument();
            //string filePath = "movieList4.xml";
            //XmlDocument doc = new XmlDocument();
            //doc.Load(filePath);
            //XmlNode root = doc.DocumentElement;
            //ShowNode(root);

            ValidationXSD();
            Console.ReadLine();
        }

        private static void XmlReaderDemo()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.IgnoreWhitespace = true;
            XmlReader reader = XmlReader.Create("movieList.xml", settings);
            while (reader.Read())
            {
                string indent = new string(' ', reader.Depth * 3);
                string line = string.Format("{0}{1} | {2} - {3}",
                    indent, reader.NodeType, reader.Name, reader.Value);
                Console.WriteLine(line);

                line = "";
                if (reader.NodeType == XmlNodeType.Element && reader.HasAttributes)
                {
                    for (int i = 0; i < reader.AttributeCount; i++)
                    {
                        reader.MoveToAttribute(i);
                        line += string.Format($"{indent} [{reader.Name}，{reader.Value}] \r\n");
                    }
                    Console.WriteLine(line);
                }
            }
            reader.Close();
        }

        private static void XmlWriteDemo()
        {
            string filePath = "movieList2.xml";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.OmitXmlDeclaration = false;
            //settings.ConformanceLevel = ConformanceLevel.Auto;

            XmlWriter writer = XmlWriter.Create(filePath, settings);
            writer.WriteStartDocument();//XML声明
            writer.WriteDocType("movieList", null, null, 
                "<!ENTITY Bruce \" 布鲁斯·威利斯\"><!ENTITY Jai \"杰·科特尼\">");
            writer.WriteStartElement("movieList");
            writer.WriteComment("新近上映电影信息");

            //writer.WriteStartElement("movie");
            //writer.WriteAttributeString("id", "1");
            //writer.WriteAttributeString("title", "魔境仙踪");
            //writer.WriteAttributeString("director", "山姆·雷米");
            //writer.WriteAttributeString("release", "2013-3-29");
            //writer.WriteElementString("starring", "詹姆斯·弗兰克/米拉·库尼斯");

            //writer.WriteStartElement("genreList");
            //writer.WriteElementString("genre", "冒险");
            //writer.WriteElementString("genre", "家庭");
            //writer.WriteElementString("genre", "奇幻");
            //writer.WriteEndElement();//genreList
            //writer.WriteEndElement();//movie

            //writer.WriteStartElement("movie");
            //writer.WriteAttributeString("id", "2");
            //writer.WriteAttributeString("title", "虎胆龙威5");
            //writer.WriteAttributeString("director", "约翰·摩尔");
            //writer.WriteAttributeString("release", "2013-3-14");

            ////writer.WriteStartElement("jimmy", "starring", null);
            //writer.WriteStartElement("starring");
            //writer.WriteEntityRef("Bruce");
            //writer.WriteString("/");
            //writer.WriteEntityRef("Jai");
            //writer.WriteEndElement();                // starring
            //writer.WriteStartElement("genreList");

            //writer.WriteElementString("genre", "动作"); 
            //writer.WriteElementString("genre", "犯罪"); 
            //writer.WriteElementString("genre", "惊悚");
            //writer.WriteEndElement();       //genreList
            //writer.WriteEndElement();       //movie

            //writer.WriteEndElement();       //movieList
            //writer.WriteEndDocument(); 
            //writer.Flush();
            //writer.Close();

            string element =
                @"     <movie id=""2"" title=""虎胆龙威5""  director=""约翰·摩尔"" release=""2013-3-14"">
                            <starring>""&Bruce;/&Jai;""</starring>
                            <genreList>
                                <genre>""动作""</genre>
                                <genre>""犯罪""</genre>
                                <genre>""惊悚""</genre>
                            </genreList>
                        </movie>";
            writer.WriteString("\r\n");
            writer.WriteRaw(element);  //写入字符串
            writer.WriteString("\r\n");

            writer.WriteEndElement();       //movieList
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        private static void XmlDocumentDemo()
        {
            string filePath = "movieList.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlElement movie = doc.CreateElement("movie");
            XmlAttribute attr = doc.CreateAttribute("id");
            attr.Value = "3";
            movie.Attributes.Append(attr);

            attr = doc.CreateAttribute("title");
            attr.Value = "霍比特人：意外之旅";
            movie.Attributes.Append(attr);

            attr = doc.CreateAttribute("director");
            attr.Value = "彼得·杰克逊";
            movie.Attributes.Append(attr);

            attr = doc.CreateAttribute("release");
            attr.Value = "2013-2-22";
            movie.Attributes.Append(attr);

            XmlElement genreList = doc.CreateElement("genreList");
            XmlElement genre = doc.CreateElement("genre");
            genre.InnerText = "奇幻";
            genreList.AppendChild(genre);

            genre = doc.CreateElement("genre");
            genre.InnerText = "冒险";
            genreList.AppendChild(genre);

            movie.AppendChild(genreList);

            XmlElement root = doc.DocumentElement;
            root.AppendChild(movie);
            doc.Save("movieList3.xml");
        }

        private static void UpdateXmlDocument()
        {
            string filePath = "movieList3.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlElement root = doc.DocumentElement;
            XmlNode movie = root.SelectSingleNode("movie[@id=2]");
            XmlNode titleAttr = movie.SelectSingleNode("@title");
            titleAttr.Value = "虎胆龙威5(A Good Day to Die Hard)";

            XmlNode starring = movie.SelectSingleNode("starring");
            starring.InnerText = starring.InnerText + "/俞莉雅·斯尼吉尔";

            XmlNode genre = movie.SelectSingleNode("genreList/genre[3]");
            XmlNode genreList = movie.SelectSingleNode("genreList");
            genreList.RemoveChild(genre);

            doc.Save("movieList4.xml");
        }

        private static void ShowNode(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Text)
                Console.Write(node.Value);
            if (node.NodeType == XmlNodeType.Element)
                Console.WriteLine(node.Name);
            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    Console.Write("{0}={1} ", attr.Name, attr.Value);
                }
                Console.WriteLine();
            }

            XmlNodeList childList = node.ChildNodes;
            foreach (XmlNode child in childList)
            {
                ShowNode(child);
            }
        }

        private static void ValidationXSD()
        {
            string filePath = "movieList.xml";
            string xsdPath = "movieList.xsd";

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.Add(null, xsdPath);
            settings.ValidationEventHandler += Settings_ValidationEventHandler;

            XmlReader reader = XmlReader.Create(filePath, settings);
            while (reader.Read()) { }
            Console.WriteLine("Complete!");
        }

        private static void Settings_ValidationEventHandler(object sender, 
            System.Xml.Schema.ValidationEventArgs e)
        {
            Console.WriteLine("Line:{0}, column:{1}, Error:{2}", 
                e.Exception.LineNumber, e.Exception.LinePosition, e.Message); ;
        }
    }
}
