using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace LoggingSimpleDemo
{
    public class OrderService
    {
        ILogger<OrderService> _logger;
        public OrderService(ILogger<OrderService> logger)
        {
            _logger = logger;
        }

        public void Show()
        {
            _logger.LogInformation("Show time：{time}", DateTime.Now); //推荐这种写法，字符串拼接时机不同
            _logger.LogInformation($"Show time：{DateTime.Now}");//这里先拼接，然后传给LogInformation方法
        }
    }
}
