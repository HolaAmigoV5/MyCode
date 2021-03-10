using System;

namespace DesignPatterns
{
    public class SimpleFactoryDemo
    {
        public SimpleFactoryDemo()
        {
            Food f1 = FoodSimpleFactory.CreateFood("西红柿炒蛋");
            f1.CookPrint();

            Food f2 = FoodSimpleFactory.CreateFood("土豆肉丝");
            f2.CookPrint();
        }
    }

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
