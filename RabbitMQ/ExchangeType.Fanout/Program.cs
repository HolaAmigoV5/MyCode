using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ExchangeType.Receive
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

            using IConnection conn = factory.CreateConnection();
            using IModel channel = conn.CreateModel();

            // 声明exchange。
            // direct：明确的路由规则，单播模式，消费端绑定的队列名称必须和消息发布时指定的路由名称一致
            // topic：模式匹配的路由规则，支持通配符(符号#和符号*。其中*匹配一个单词， #则表示匹配0个或多个单词，单词之间用.分割)
            // fanout：消息广播，将消息分发到exchange上绑定的所有队列上
            channel.ExchangeDeclare(exchange: exchangeType, type: exchangeType);

            // 声明随机队列名称
            var queuename = channel.QueueDeclare().QueueName;

            // 绑定队列到指定exchange, direct
            //channel.QueueBind(queue: queuename, exchange: exchangeType, routingKey: "green");

            // 绑定队列到指定exchange, topic，使用通配符
            channel.QueueBind(queue: queuename, exchange: exchangeType , routingKey: "#.*.fast");

            // 绑定队列到指定exchange, fanout，无 routingKey
            //channel.QueueBind(queue: queuename, exchange: exchangeType, routingKey: "");

            Console.WriteLine("[*] Waitting for {0} logs.", exchangeType);

            //声明consumer
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.Span;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("[x] Received {0}:{1}", ea.RoutingKey, message);
            };

            channel.BasicConsume(queue: queuename, autoAck: true, consumer: consumer);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
