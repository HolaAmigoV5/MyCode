using System;
using System.Collections.Generic;

namespace ConsoleSubjectAndObserver
{
    class Program
    {
        static void Main(string[] args)
        {
            Heater heater = new Heater();
            Screen screen = new Screen();
            Alarm alarm = new Alarm();

            heater.Register(screen);
            heater.Register(alarm);

            heater.BoilWater();
            heater.UnRegister(alarm);

            Console.WriteLine();
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

        protected virtual void OnBoiled()
        {
            base.Notify();
        }

        public void BoilWater()
        {
            for (int i = 0; i < 100; i++)
            {
                temprature = i + 1;
                if (temprature > 97)
                    OnBoiled();
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

        protected virtual void Notify()
        {
            container.ForEach(m => m.Update());
        }
    }

    public interface IObservable
    {
        void Register(IObserver obj);  //注册IObserver
        void UnRegister(IObserver obj);  //取消IObserver注册
    }
    public interface IObserver
    {
        void Update();
    }

    public class Screen : IObserver
    {
        public void Update()
        {
            Console.WriteLine("Screen".PadRight(7) + "：水快烧开了。");
        }
    }

    public class Alarm : IObserver
    {
        public void Update()
        {
            Console.WriteLine("Alarm".PadRight(7) + "：嘟嘟嘟，水温快烧开了。");
        }
    }
}
