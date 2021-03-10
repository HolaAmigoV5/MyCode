using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptionsDemo.Services
{
    public static class OrderServiceExtensions
    {
        public static IServiceCollection AddOrderService(this IServiceCollection services, 
            IConfiguration configuration)
        {
            //services.Configure<OrderServiceOptions>(configuration);

            //为数据选项添加验证
            //services.AddOptions<OrderServiceOptions>().Configure(options =>
            //{
            //    configuration.Bind(options);
            //}).Validate(options =>
            //{
            //    return options.MaxOrderCount <= 100;
            //}, "MaxOrderCount不能大于100.");

            //属性注入方式验证
            //services.AddOptions<OrderServiceOptions>().Configure(options =>
            //{
            //    configuration.Bind(options);
            //}).ValidateDataAnnotations();

            //接口注入验证方式
            services.Configure<OrderServiceOptions>(configuration);


            //注入验证接口
            services.AddSingleton<IValidateOptions<OrderServiceOptions>, OrderServiceValidateOptions>();

            //进行动态配置：修改配置信息
            //services.PostConfigure<OrderServiceOptions>(option =>
            //{
            //    option.MaxOrderCount += 100;
            //});
            services.AddTransient<IOrderService, OrderService>();
            return services;
        }
    }
}
