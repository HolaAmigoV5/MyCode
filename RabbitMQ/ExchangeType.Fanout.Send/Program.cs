using RabbitMQ.Client;
using System;
using System.Text;

namespace ExchangeType.Send
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "192.168.11.135",
                Port = 5672,
                UserName = "admin",
                Password = "p.123456",
                VirtualHost="wby"
            };

            //string exchangeType = "direct";
            //string exchangeType = "topic";
            string exchangeType = "fanout";

            using var conn = factory.CreateConnection();
            using var channel = conn.CreateModel();

            var queueName = channel.QueueDeclare().QueueName;

            // 声明exchange。
            // direct：明确的路由规则，单播模式，消费端绑定的队列名称必须和消息发布时指定的路由名称一致
            // topic：模式匹配的路由规则，支持通配符
            // fanout：消息广播，将消息分发到exchange上绑定的所有队列上
            channel.ExchangeDeclare(exchange: exchangeType, type: exchangeType);

            var message = args.Length > 0 ? args[0] : $"Info:Hello World! ExchangeType:{exchangeType}";
            var body = Encoding.UTF8.GetBytes(message);

            // 发布到指定exchange，direct，必须指定routingKey
            //channel.BasicPublish(exchange: exchangeType, routingKey: "green", basicProperties: null, body: body);

            // 发布到指定exchange，topic，必须指定routingKey
            channel.BasicPublish(exchange: exchangeType, routingKey: "first.green.fast", basicProperties: null, body: body);

            // 发布到指定exchange，fanout类型无需指定routingKey
            //channel.BasicPublish(exchange: exchangeType, routingKey: "", basicProperties: null, body: body);
            Console.WriteLine(" [x] Sent {0}", message);

            Console.WriteLine(" Press [Enter] to exit");
            Console.ReadLine();
        }
    }
}
