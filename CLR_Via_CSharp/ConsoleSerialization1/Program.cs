using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ConsoleSerialization
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            //创建对象图以便把它序列化到流中
            var objectGraph = new List<String> { "Jeff", "Kristin", "Aidan", "Grant" };
            Stream stream = SerializeToMemory(objectGraph);

            stream.Position = 0;
            objectGraph = null;

            //反序列化对象，证明它能工作
            objectGraph = (List<String>)DeserializeFromMemory(stream);
            objectGraph.ForEach(o => Console.WriteLine(o));

            Console.ReadLine();
            //foreach (var item in objectGraph)
            //{
            //    Console.WriteLine(item);
            //}
        }

        //序列化
        private static MemoryStream SerializeToMemory(Object objectGraph)
        {
            //构造流来容纳序列化对象
            MemoryStream stream = new MemoryStream();

            //构造序列化格式化器来执行所有真正的工作
            BinaryFormatter formatter = new BinaryFormatter();

            //告诉格式化器将对象序列化到流中
            formatter.Serialize(stream, objectGraph);
            return stream;
        }

        //反序列化
        private static object DeserializeFromMemory(Stream stream)
        {
            //构造序列化格式化器来做所有真正的工作
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
    }
}
