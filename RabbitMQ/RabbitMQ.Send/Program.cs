using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Send
{
    public class Program
    {
        // 发送端
        public static void Main(string[] args)
        {
            //实例化连接工厂
            var factory = new ConnectionFactory()
            {
                HostName = "192.168.11.135",
                Port = 5672,
                UserName = "admin",
                Password = "p.123456",
                VirtualHost = "wby",
                ContinuationTimeout = new TimeSpan(10, 0, 0, 0)
            };
            //建立连接
            using IConnection connection = factory.CreateConnection();
            
            // 创建信道
            using IModel channel = connection.CreateModel();

            // 声明队列（指定durable:true，告知rabbitmq对消息进行持久化）
            channel.QueueDeclare(queue: "hello", durable: false, exclusive: false,
                autoDelete: false, arguments: null);

            // 将消息标记为持久性：将IBasicProperties.SetPersistent设置为true。
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            // 构建byte消息数据包
            string message = args.Length > 0 ? args[0] : "Hello RabbitMQ!";
            var body = Encoding.UTF8.GetBytes(message);

            // 发送数据包
            channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: properties, body: body);
            Console.WriteLine(" [x] Send {0}", message);
            Console.ReadLine();
        }
    }
}
