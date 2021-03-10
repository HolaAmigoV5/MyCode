using System;

namespace SimpleFactoryDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            Food f1 = FoodSimpleFactory.CreateFood("西红柿炒蛋");
            f1.CookPrint();

            Food f2 = FoodSimpleFactory.CreateFood("土豆肉丝");
            f2.CookPrint();

            Console.ReadLine();
        }
    }


    /// <summary>
    /// 简单工厂类
    /// </summary>
    public class FoodSimpleFactory
    {
        public static Food CreateFood(string type)
        {
            Food food = null;
            switch (type)
            {
                case "土豆肉丝":
                    food = new TomatoScrambleEggs();
                    break;
                case "西红柿炒蛋":
                    food = new TomatoScrambleEggs();
                    break;
                default:
                    break;
            }
            return food;
        }
    }

    /// <summary>
    /// 菜抽象类
    /// </summary>
    public abstract class Food
    {
        //输出点什么菜
        public abstract void CookPrint();
    }


    public class TomatoScrambleEggs : Food
    {
        public override void CookPrint()
        {
            Console.WriteLine("西红柿炒鸡蛋！");
        }
    }

    public class ShreddedPorkWithPotatoes : Food
    {
        public override void CookPrint()
        {
            Console.WriteLine("一份土豆肉丝！");
        }
    }
}
