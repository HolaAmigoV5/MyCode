using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OptionsDemo.Services
{
    public interface IOrderService
    {
        int ShowMaxOrderCount();
    }
    public class OrderService : IOrderService
    {
        //IOptions<OrderServiceOptions> _options;

        //换成IOptionsSnapshot后可以读取修改后的配置
        //IOptionsSnapshot<OrderServiceOptions> _options;

        IOptionsMonitor<OrderServiceOptions> _options;


        //public OrderService(IOptions<OrderServiceOptions> options)
        //{
        //    _options = options;
        //}

        //public OrderService(IOptionsSnapshot<OrderServiceOptions> options)
        //{
        //    _options = options;
        //}

        public OrderService(IOptionsMonitor<OrderServiceOptions> options)
        {
            _options = options;
            _options.OnChange(option =>
            {
                Console.WriteLine($"配置发生了变化：{option.MaxOrderCount}");
            });
        }
        public int ShowMaxOrderCount()
        {
            //IOptionsSnapshot时使用
            //return _options.Value.MaxOrderCount;

            //IOptionsMonitor时使用
            return _options.CurrentValue.MaxOrderCount;
        }
    }

    public class OrderServiceOptions
    {
        [Range(30, 100)]
        public int MaxOrderCount { get; set; } = 100;
    }

    public class OrderServiceValidateOptions : IValidateOptions<OrderServiceOptions>
    {
        public ValidateOptionsResult Validate(string name, OrderServiceOptions options)
        {
            if (options.MaxOrderCount > 100)
            {
                return ValidateOptionsResult.Fail("不能大于100");
            }
            else
                return ValidateOptionsResult.Success;
        }
    }
}
