using System;
using System.Collections.Generic;

namespace ObserverPatternDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            TenXun tenXun = new TenXunGame("TenXun Game", "Have a new game published....");

            //添加订阅者
            tenXun.AddObserver(new Subscriber("Learning Hard"));
            tenXun.AddObserver(new Subscriber("Tom"));


            tenXun.Update();

            Console.ReadLine();
        }
    }


    /// <summary>
    /// 被观察者抽象——订阅号抽象类
    /// </summary>
    public abstract class TenXun
    {
        //保存订阅者列表
        private List<IObserver> observers = new List<IObserver>();


        public string Symbol { get; set; }
        public string Info { get; set; }
        public TenXun(string symbol, string info)
        {
            Symbol = symbol;
            Info = info;
        }

        #region 订阅号列表的维护操作
        public void AddObserver(IObserver ob)
        {
            observers.Add(ob);
        }

        public void RemoveObserver(IObserver ob)
        {
            observers.Remove(ob);
        }

        #endregion

        public void Update()
        {
            //遍历订阅者列表进行通知
            foreach (IObserver ob in observers)
            {
                if (ob != null)
                    ob.ReceiveAndPrint(this);
            }
        }
    }

    /// <summary>
    /// 具体被观察者——具体订阅号类
    /// </summary>
    public class TenXunGame : TenXun
    {
        public TenXunGame(string symbol, string info) : base(symbol, info) { }
    }

    /// <summary>
    /// 观察者(订阅者)接口
    /// </summary>
    public interface IObserver
    {
        void ReceiveAndPrint(TenXun tenXun);
    }


    /// <summary>
    /// 具体的观察者(订阅者)
    /// </summary>
    public class Subscriber : IObserver
    {
        public string Name { get; set; }
        public Subscriber(string name)
        {
            this.Name = name;
        }

        public void ReceiveAndPrint(TenXun tenXun)
        {
            Console.WriteLine("Notified {0} of {1}'s" + "Info is :{2}", Name, tenXun.Symbol, tenXun.Info);
        }
    }
}
