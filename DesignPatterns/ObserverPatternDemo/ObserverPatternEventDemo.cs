using System;

namespace ObserverPatternDemo
{
    public class ObserverPatternEventDemo
    {
        public void Call()
        {
            TenXun2 tenXun2 = new TenXunGame2("TenXun Game", "Have a new game published....");
            var tom = new Subscriber2("Tom");

            tenXun2.AddObserver(new NotifyEventHandler(new Subscriber2("Learning Hard").ReceiveAndPrint));
            tenXun2.AddObserver(new NotifyEventHandler(tom.ReceiveAndPrint));

            tenXun2.Update();

            Console.WriteLine("---------------------------------");
            Console.WriteLine("移除Tom订阅者");
            tenXun2.RemoveObserver(new NotifyEventHandler(tom.ReceiveAndPrint));
            tenXun2.Update();

            Console.ReadLine();
        }
    }

    //委托充当订阅者接口类
    public delegate void NotifyEventHandler(object sender);

    /// <summary>
    /// 被观察者抽象——订阅号抽象类
    /// </summary>
    public abstract class TenXun2
    {
        public NotifyEventHandler NotifyEvent;

        public string Symbol { get; set; }
        public string Info { get; set; }
        public TenXun2(string symbol, string info)
        {
            Symbol = symbol;
            Info = info;
        }

        #region 订阅号列表的维护操作

        public void AddObserver(NotifyEventHandler ob)
        {
            NotifyEvent += ob;
        }

        public void RemoveObserver(NotifyEventHandler ob)
        {
            NotifyEvent -= ob;
        }
        #endregion

        public void Update()
        {
            NotifyEvent?.Invoke(this);
        }
    }

    /// <summary>
    /// 具体被观察者——具体订阅号类
    /// </summary>
    public class TenXunGame2 : TenXun2
    {
        public TenXunGame2(string symbol, string info) : base(symbol, info) { }
    }


    /// <summary>
    /// 具体的观察者(订阅者)
    /// </summary>
    public class Subscriber2
    {
        public string Name { get; set; }
        public Subscriber2(string name)
        {
            this.Name = name;
        }

        public void ReceiveAndPrint(object obj)
        {
            if (obj is TenXun2 tenXun)
                Console.WriteLine("Notified {0} of {1}'s" + " Info is:{2}", Name, tenXun.Symbol, tenXun.Info);
        }
    }
}
