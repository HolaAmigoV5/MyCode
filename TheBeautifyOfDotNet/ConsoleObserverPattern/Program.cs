using System;
using System.Collections.Generic;

namespace ConsoleObserverPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Heater heater = new Heater();
            Screen screen = new Screen();
            heater.Register(screen);
            heater.BoilWater();
            Console.ReadLine();
        }
    }

    public class Heater : SubjectBase
    {
        private string type;
        private string area;
        private int temprature;
        public Heater(string type, string area)
        {
            this.type = type;
            this.area = area;
            temprature = 0;
        }
        public Heater() : this("XiaoMi 001", "China Xi'an") { }

        public string Type { get { return type; } }
        public string Area { get { return area; } }

        protected virtual void OnBoiled(BoiledEventArgs e)
        {
            base.Notify(e);
        }

        public void BoilWater()
        {
            for (int i = 0; i < 100; i++)
            {
                temprature = i + 1;
                if (temprature > 97)
                {
                    BoiledEventArgs e = new BoiledEventArgs(temprature, type, area);
                    OnBoiled(e);
                }
            }
        }
    }

    public abstract class SubjectBase : IObservable
    {
        private List<IObserver> container = new List<IObserver>();
        public void Register(IObserver obj)
        {
            container.Add(obj);
        }

        public void UnRegister(IObserver obj)
        {
            container.Remove(obj);
        }

        protected virtual void Notify(BoiledEventArgs e)
        {
            container.ForEach(m => m.Update(e));
        }
    }

    public interface IObservable
    {
        void Register(IObserver obj);  //注册IObserver
        void UnRegister(IObserver obj);  //取消IObserver注册
    }
    public interface IObserver
    {
        void Update(BoiledEventArgs e);
    }

    public class Screen : IObserver
    {
        private bool isDisplayedType = false;
        public void Update(BoiledEventArgs e)
        {
            //产地和型号只打印一次
            if (!isDisplayedType)
            {
                Console.WriteLine($"{e.Area} - {e.Type}");
                Console.WriteLine();
                isDisplayedType = true;
            }
            if (e.Temperature < 100)
                Console.WriteLine(string.Format("Screen".PadRight(7) + "：水快烧开了，当前温度：{0}", e.Temperature));
            else
                Console.WriteLine("Screen".PadRight(7) + "：水已经烧开了!!");
        }
    }

    public class Alarm : IObserver
    {
        private bool isDisplayedType = false;
        public void Update(BoiledEventArgs e)
        {
            //产地和型号只打印一次
            if (!isDisplayedType)
            {
                Console.WriteLine($"{e.Area} - {e.Type}");
                Console.WriteLine();
                isDisplayedType = true;
            }
            if (e.Temperature < 100)
                Console.WriteLine(string.Format("Alarm".PadRight(7) + "：水快烧开了，当前温度：{0}", e.Temperature));
            else
                Console.WriteLine("Alarm".PadRight(7) + "：水已经烧开了!!");
        }
    }

    public class BoiledEventArgs
    {
        private int temperature;
        private string type;
        private string area;
        public BoiledEventArgs(int temperature, string type, string area)
        {
            this.temperature = temperature;
            this.type = type;
            this.area = area;
        }

        public int Temperature { get { return temperature; } }
        public string Type { get { return type; } }
        public string Area { get { return area; } }
    }
}
