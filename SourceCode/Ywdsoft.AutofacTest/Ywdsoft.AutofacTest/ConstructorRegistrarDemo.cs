using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Ywdsoft.Core;

namespace Ywdsoft.AutofacTest
{
    public class ConstructorRegistrarDemo : IDependencyRegistrar
    {
        /// <summary>
        /// 
        /// </summary>
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void Register(ContainerBuilder builder, List<Type> listType)
        {
            builder.RegisterType(typeof(TestDemo)).AsSelf()
               .OnRegistered(e => Console.WriteLine("OnRegistered在注册的时候调用!"))
               .OnPreparing(e => Console.WriteLine("OnPreparing在准备创建的时候调用!"))
               .OnActivating(e => Console.WriteLine("OnActivating在创建之前调用!"))
               .OnActivated(e => Console.WriteLine("OnActivated创建之后调用!"))
               .OnRelease(e => Console.WriteLine("OnRelease在释放占用的资源之前调用!"));
        }
    }

    public class TestDemo
    {
        private readonly string _name;

        private readonly string _sex;

        private readonly int _age;

        public TestDemo(string name, string sex, int age)
        {
            _name = name;
            _age = age;
            _sex = sex;
        }
        public string Sex
        {
            get
            {
                return _sex;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int Age
        {
            get
            {
                return _age;
            }
        }
    }
}
