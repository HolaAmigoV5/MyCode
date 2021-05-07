using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RPCClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var fatory = new ConnectionFactory()
            {
                HostName = "192.168.11.135",
                Port = 5672,
                UserName = "admin",
                Password = "p.123456",
                VirtualHost = "wby"
            };

            using IConnection conn = fatory.CreateConnection();
            using IModel channel = conn.CreateModel();

            //申明唯一guid用来标识此次发送的远程调用请求
            var correlationId = Guid.NewGuid().ToString();

            //申明需要监听的回调队列
            var replyQueue = channel.QueueDeclare().QueueName;

            var properties = channel.CreateBasicProperties();
            properties.ReplyTo = replyQueue;    // 指定回调队列
            properties.CorrelationId = correlationId;   //指定消息唯一标识

            string number = args.Length > 0 ? args[0] : "30";
            var body = Encoding.UTF8.GetBytes(number);

            //发布消息
            channel.BasicPublish(exchange: "", routingKey: "rpc_queue", basicProperties: properties, body: body);

            Console.WriteLine($" [*] Request fib ({number})");

            // 创建消费者用于消息回调
            var callbackConsumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: replyQueue, autoAck: true, consumer: callbackConsumer);
            callbackConsumer.Received += (model, ea) =>
            {
                //仅当消息回调的ID与发送的ID一致时，说明远程调用结果正确返回。
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var responseMsg = $"Get Response: {Encoding.UTF8.GetString(ea.Body.Span)}";
                    Console.WriteLine($"[x]: {responseMsg}");
                }
            };

            Console.ReadLine();
        }
    }
}
