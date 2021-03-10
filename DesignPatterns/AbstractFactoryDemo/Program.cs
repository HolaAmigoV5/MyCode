using System;

namespace AbstractFactoryDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //南昌工厂制作南昌鸭脖和鸭架
            AbstractFactory ncFactory = new NanChangFactory();
            YaBo ncYaBo = ncFactory.CreateYaBo();
            YaJia ncYaJia = ncFactory.CreateYaJia();
            ncYaBo.Print();
            ncYaJia.Print();


            //上海工厂制作上海鸭脖和鸭架
            AbstractFactory shFactory = new ShangHaiFactory();
            YaBo shYaBo = shFactory.CreateYaBo();
            YaJia shYaJia = shFactory.CreateYaJia();
            shYaBo.Print();
            shYaJia.Print();
        }
    }

    /// <summary>
    /// 抽象工厂类
    /// </summary>
    public abstract class AbstractFactory
    {
        public abstract YaBo CreateYaBo();
        public abstract YaJia CreateYaJia();
    }

    public class NanChangFactory : AbstractFactory
    {
        public override YaBo CreateYaBo()
        {
            return new NanChangYaBo();
        }

        public override YaJia CreateYaJia()
        {
            return new NanChangYaJia();
        }
    }

    public class ShangHaiFactory : AbstractFactory
    {
        public override YaBo CreateYaBo()
        {
            return new ShangHaiYaBo();
        }

        public override YaJia CreateYaJia()
        {
            return new ShangHaiYaJia();
        }
    }

    /// <summary>
    /// 鸭脖抽象类
    /// </summary>
    public abstract class YaBo
    {
        public abstract void Print();
    }

    public class NanChangYaBo : YaBo
    {
        public override void Print()
        {
            Console.WriteLine("南昌的鸭脖");
        }
    }

    public class NanChangYaJia : YaJia
    {
        public override void Print()
        {
            Console.WriteLine("南昌的鸭架");
        }
    }


    /// <summary>
    /// 鸭架抽象类
    /// </summary>
    public abstract class YaJia
    {
        public abstract void Print();
    }

    public class ShangHaiYaBo : YaBo
    {
        public override void Print()
        {
            Console.WriteLine("上海鸭脖");
        }
    }

    public class ShangHaiYaJia : YaJia
    {
        public override void Print()
        {
            Console.WriteLine("上海鸭架");
        }
    }
}
