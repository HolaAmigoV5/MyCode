using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns
{
    class FactoryMethodDemo
    {
    }

    public abstract class OrderFood
    {
        public abstract void Print();
    }

    public class TomatoScrambleEggs : OrderFood
    {
        public override void Print()
        {
            Console.WriteLine("西红柿炒鸡蛋!");
        }
    }
}
