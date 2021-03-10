using System;

namespace TemplateMethodDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建一个菠菜实例并调用模板方法
            Spinach spinach = new Spinach();
            spinach.CookVegetable();
            Console.ReadLine();
        }
    }


    public abstract class Vegetable
    {
        //模板方法，不要把模板方法定义为Virtual或abstract方法，避免子类重写，防止更改流程的执行顺序
        public void CookVegetable()
        {
            Console.WriteLine("炒蔬菜的一般做法");
            this.PourOil();
            this.HeatOil();
            this.PourVegetable();
            this.Stir_fry();
        }

        public void PourOil()
        {
            Console.WriteLine("倒油");
        }

        public void HeatOil()
        {
            Console.WriteLine("把油烧热");
        }


        //油烧热后倒菜进去，具体哪种蔬菜由子类决定
        public abstract void PourVegetable();

        //开始翻炒蔬菜
        public void Stir_fry()
        {
            Console.WriteLine("翻炒");
        }
    }

    /// <summary>
    /// 菠菜
    /// </summary>
    public class Spinach : Vegetable
    {
        public override void PourVegetable()
        {
            Console.WriteLine("倒菠菜进锅中");
        }
    }

    /// <summary>
    /// 大白菜
    /// </summary>
    public class ChineseCabbage : Vegetable
    {
        public override void PourVegetable()
        {
            Console.WriteLine("到大白菜进锅中");
        }
    }
}
