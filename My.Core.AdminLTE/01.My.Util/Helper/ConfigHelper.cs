using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace My.Util
{
    /// <summary>
    /// 描述：配置文件帮助类
    /// 作者：wby 2019/9/23 16:22:21
    /// </summary>
    public class ConfigHelper
    {
        private static IConfiguration _config { get; }
        static ConfigHelper()
        {
            IConfiguration config = null;
            try
            {
                config = AutofacHelper.GetScopeService<IConfiguration>();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

            if(config==null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json");
                config = builder.Build();
            }
            _config = config;
        }

        /// <summary>
        /// 从appsettings.json获取key值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            return _config[key];
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="nameOfCon">连接字符串名</param>
        /// <returns></returns>
        public static string GetConnectionString(string nameOfCon)
        {
            return _config.GetConnectionString(nameOfCon);
        }
    }
}
