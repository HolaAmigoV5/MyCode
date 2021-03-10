using System;

namespace FactoryMethodDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            //初始化做菜的两个工厂
            CreatorFactory spCreator = new ShreddedPorkWithPotatoesFactory();
            Food sp = spCreator.CreateFood();
            sp.Print();


            CreatorFactory tseCreator = new TomatoScrambledEggsFactory();
            Food tsf = tseCreator.CreateFood();
            tsf.Print();

            Console.Read();
        }
    }

    public abstract class Food
    {
        public abstract void Print();
    }

    public class TomatoScrambledEggs : Food
    {
        public override void Print()
        {
            Console.WriteLine("西红柿炒鸡蛋");
        }
    }

    public class ShreddedPorkWithPotatoes : Food
    {
        public override void Print()
        {
            Console.WriteLine("土豆肉丝");
        }
    }

    public abstract class CreatorFactory
    {
        public abstract Food CreateFood();
    }

    public class TomatoScrambledEggsFactory : CreatorFactory
    {
        public override Food CreateFood()
        {
            return new TomatoScrambledEggs();
        }
    }

    public class ShreddedPorkWithPotatoesFactory : CreatorFactory
    {
        public override Food CreateFood()
        {
            return new ShreddedPorkWithPotatoes();
        }
    }
}
