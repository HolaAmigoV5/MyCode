using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ConfigurationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 键值对模式配置
            //IConfigurationBuilder builder = new ConfigurationBuilder();
            //builder.AddInMemoryCollection(new Dictionary<string, string> {
            //    { "key1","value1" },
            //    { "key2","value2" },
            //    { "section1:key4","value4" },
            //    { "section2:key5","value5" },
            //    { "section2:key6","value6" },
            //    { "section2:section3:key7","value7" }
            //});


            //IConfigurationRoot configurationRoot = builder.Build(); //配置对象构建出来，返回配置的根对象
            //Console.WriteLine(configurationRoot["key1"]);
            //Console.WriteLine(configurationRoot["key2"]);

            //IConfigurationSection section = configurationRoot.GetSection("section1");
            //Console.WriteLine($"key4:{section["key4"]}");
            //Console.WriteLine($"key5:{section["key5"]}");

            //IConfigurationSection section2 = configurationRoot.GetSection("section2");
            //Console.WriteLine($"key5_v2:{section2["key5"]}");
            //var section3 = section2.GetSection("section3");
            //Console.WriteLine($"key7:{section3["key7"]}"); 
            #endregion

            #region 命令行模式配置
            //var builder = new ConfigurationBuilder();
            ////builder.AddCommandLine(args);

            ////命令替换模式。这里常见的应用是：用短写的"-k1"命令代替，全称命令"CommandLineKey1"。
            //var mapper = new Dictionary<string, string> { {"-k1", "CommandLineKey1" } }; 
            //builder.AddCommandLine(args, mapper);

            //var configurationRoot = builder.Build();
            //Console.WriteLine($"CommandLineKey1：{configurationRoot["CommandLineKey1"]}");
            #endregion

            #region 环境变量配置
            //var builder = new ConfigurationBuilder();
            ////builder.AddEnvironmentVariables();

            ////var configurationRoot = builder.Build();
            ////Console.WriteLine($"key1:{configurationRoot["key1"]}");

            ////分层键
            ////var section = configurationRoot.GetSection("SECTION1");
            ////Console.WriteLine($"key3:{section["KEY3"]}");

            //////多层键
            ////var section2 = configurationRoot.GetSection("SECTION1:SECTION2");
            ////Console.WriteLine($"KEY4:{section2["KEY4"]}");


            ////前缀过滤：注入指定前缀的变量，如下只注入前缀为"XIAO_"的环境变量
            //builder.AddEnvironmentVariables("XIAO_");
            //var configurationRoot = builder.Build();
            //Console.WriteLine($"KEY1:{configurationRoot["KEY1"]}");
            //Console.WriteLine($"KEY2:{configurationRoot["KEY2"]}");
            #endregion

            #region 文件配置
            var builder = new ConfigurationBuilder();
            //optional参数：true，配置不存在时不报错；false，配置不存在时报错。
            //reloadOnChange参数：true，配置文件修改就重载，立马生效；false，修改不重载
            builder.AddJsonFile("appsettings.Json",true,true);
            //builder.AddIniFile("appsettings.ini",true,true);

            //var configurationRoot = builder.Build();

            ////监视变更后重新输出，只能监视一次
            //IChangeToken token = configurationRoot.GetReloadToken();
            //token.RegisterChangeCallback(state =>
            //{
            //    Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
            //    Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
            //}, configurationRoot);
            //Console.ReadKey();

            ////文件的监听，只要一有变更，立马捕获
            //ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () => {
            //    Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
            //    Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
            //});
            //Console.WriteLine("开始了");

            //Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
            //Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
            //Console.WriteLine($"Key3:{configurationRoot["Key3"]}");
            //Console.ReadKey();


            //Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
            //Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
            //Console.WriteLine($"Key3:{configurationRoot["Key3"]}");
            //Console.ReadKey();
            #endregion

            #region 使用强类型对象承载配置数据
            //var config = new Config() { Key1 = "config key1", Key5 = false };
            //configurationRoot.Bind(config);

            //分组后，分别绑定
            ////configurationRoot.GetSection("OrderService").Bind(config, 
            ////    binderOptions => { binderOptions.BindNonPublicProperties = true; }); //让私有属性生效


            //Console.WriteLine($"Key1:{config.Key1}");
            //Console.WriteLine($"Key5:{config.Key5}");
            //Console.WriteLine($"Key6:{config.Key6}"); 
            #endregion

            #region 自定义配置数据源
            //扩展ConfigurationBuilder实现，包装我的配置源对象，封装
            builder.AddMyConfiguration();

            //builder.Add(new MyConfigurationSource());

            var configRoot = builder.Build();
            ChangeToken.OnChange(() => configRoot.GetReloadToken(), () => {
                Console.WriteLine($"lastTime:{configRoot["lastTime"]}");
            });
            //Console.WriteLine($"lastTime:{configRoot["lastTime"]}");
            Console.WriteLine("开始了。");
            #endregion

            Console.ReadLine();
        }

        class Config
        {
            public string Key1 { get; set; }
            public bool Key5 { get; set; }
            public int Key6 { get; private set; } = 100;
        }
    }
}
