using System;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;

namespace ConsoleSerializable
{
    class Program
    {
        static void Main(string[] args)
        {
            IFormatter formatter = new SoapFormatter();
            string connString = "Data Source=.; Initial Catalog=DB; User ID=sa; Password=123";
            Product item = new Product(connString) { Name = "Lumia 920" };
            Stream fs = File.OpenWrite(@"D:\product2.xml");
            formatter.Serialize(fs, item);
            fs.Dispose();

            fs = File.OpenRead(@"D:\product2.xml");
            Product newItem = (Product)formatter.Deserialize(fs);
            Console.WriteLine(newItem);
            Console.ReadLine();
        }
    }

    [Serializable]
    public class Product : ObjectBase, ISerializable
    {
        private string connString;
        [NonSerialized]
        public SqlConnection conn;
        public Product(string connString)
        {
            this.connString = connString;
        }
        protected Product(SerializationInfo info, StreamingContext context)
        {
            string encrypted = info.GetString("encrypted");
            this.connString = encrypted.Replace("*", "a").Replace("+", "s").Replace("$", "i");
            this.Name = info.GetString("name");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            string encrypted = this.connString.Replace("a", "*").Replace("s", "+").Replace("i", "$");
            info.AddValue("encrypted", encrypted);
            info.AddValue("name", base.Name);
        }

        public override string ToString()
        {
            return string.Format($"Con:【{this.connString}】, Name:【{this.Name}】");
        }
    }

    public class ObjectBase { public string Name; }
}
