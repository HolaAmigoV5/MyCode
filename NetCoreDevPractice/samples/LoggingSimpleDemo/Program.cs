using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace LoggingSimpleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json", false, true);
            var config = configBuilder.Build();

            IServiceCollection serviceCollection = new ServiceCollection();
            //工厂模式将配置对象注册到容器管理
            serviceCollection.AddSingleton<IConfiguration>(p => config);

            serviceCollection.AddLogging(builder => {
                builder.AddConfiguration(config.GetSection("Logging"));
                builder.AddConsole();
            });

            serviceCollection.AddTransient<OrderService>();

            IServiceProvider service = serviceCollection.BuildServiceProvider();
            var order = service.GetService<OrderService>();
            order.Show();

            //ILoggerFactory loggerFactory = service.GetService<ILoggerFactory>();

            //ILogger alogger = loggerFactory.CreateLogger("alogger");//创建日志记录器
            //alogger.LogDebug(2001, "aiya");
            //alogger.LogInformation("hello");
            //var ex = new Exception("出错了");
            //alogger.LogError(ex, "出错了");

            Console.ReadKey();
        }
    }
}
