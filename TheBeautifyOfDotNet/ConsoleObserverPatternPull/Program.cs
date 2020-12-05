using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleObserverPatternPull
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
        public int Tempprature { get { return temprature; } }

        protected virtual void OnBoiled()
        {
            base.Notify(this);
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

        protected virtual void Notify(IObservable obj)
        {
            container.ForEach(m => m.Update(obj));
        }
    }

    public interface IObservable
    {
        void Register(IObserver obj);  //注册IObserver
        void UnRegister(IObserver obj);  //取消IObserver注册
    }
    public interface IObserver
    {
        void Update(IObservable sender);
    }

    public class Screen : IObserver
    {
        private bool isDisplayType = false;
        public void Update(IObservable obj)
        {
            Heater heater = (Heater)obj;
            if (!isDisplayType)
            {
                Console.WriteLine($"{heater.Area} - {heater.Type}");
                Console.WriteLine();
                isDisplayType = true;
            }
            if (heater.Tempprature < 100)
                Console.WriteLine(string.Format("Screen".PadRight(7) + "：水快烧开了，当前温度是{0}", heater.Tempprature));
            else
                Console.WriteLine(string.Format("Screen".PadRight(7) + "：水已经烧开了！"));
        }
    }

    public class Alarm : IObserver
    {
        public void Update(IObservable obj)
        {
            Heater heater = obj as Heater;
            if (heater.Tempprature == 100)
                Console.WriteLine("Alarm".PadRight(7) + "：嘟嘟嘟，水烧开了");
        }
    }
}
