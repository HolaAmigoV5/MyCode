using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RPCServer
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
                VirtualHost = "wby"
            };

            using IConnection conn = factory.CreateConnection();
            using IModel channel = conn.CreateModel();

            channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false,
                autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            Console.WriteLine("[*] Waiting for message.");

            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.Span);
                int n = int.Parse(message);
                Console.WriteLine($"Receive request of Fib ({n})");

                int result = Fib(n);

                //从请求的参数中获取请求的唯一标识，在消息回传时同样绑定
                var properties = ea.BasicProperties;
                var replyProperties = channel.CreateBasicProperties();
                replyProperties.CorrelationId = properties.CorrelationId;

                //将远程调用结果发送到客户端监听的队列上
                channel.BasicPublish(exchange: "", routingKey: properties.ReplyTo, basicProperties: replyProperties,
                    body: Encoding.UTF8.GetBytes(result.ToString()));

                //手动发回消息确认
                channel.BasicAck(ea.DeliveryTag, false);
                Console.WriteLine($"Return result:Fib({n})={result}");
            };

            channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);
            Console.ReadLine();
        }

        private static int Fib(int n)
        {
            if (n < 2)
                return n;

            var dp = new int[n + 1];
            dp[0] = 1; dp[1] = 1;
            for(int i=2; i <= n; i++)
            {
                dp[i] = dp[i - 1] + dp[i - 2];
            }
            return dp[n];
        }
    }
}
