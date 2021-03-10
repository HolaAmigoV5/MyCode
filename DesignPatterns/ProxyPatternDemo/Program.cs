using System;

namespace ProxyPatternDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建一个代理对象并发出请求
            Person proxy = new Friend();
            proxy.BuyProduct();
            Console.ReadLine();
        }
    }

    /// <summary>
    /// 抽象主题角色
    /// </summary>
    public abstract class Person
    {
        public abstract void BuyProduct();
    }


    /// <summary>
    /// 真实主题角色
    /// </summary>
    public class RealBuyPerson : Person
    {
        public override void BuyProduct()
        {
            Console.WriteLine("帮我买一个IPhone和一台苹果电脑");
        }
    }

    //代理角色
    public class Friend : Person
    {
        //引用真实主题实例
        RealBuyPerson realSubject;
        public override void BuyProduct()
        {
            Console.WriteLine("通过代理类访问真实实体对象的方法");
            if (realSubject == null)
                realSubject = new RealBuyPerson();

            
        }

        //代理角色执行操作：买之前列清单
        public void PreBuyProduct()
        {
            //可能不止一个朋友叫这位朋友带东西，首先这位出国的朋友要对每一位朋友要带的东西列一个清单等
            Console.WriteLine("我怕弄糊涂了，需要列一张清单，张三：要带相机，李四：要带Iphone...........");
        }

        //买之后，东西分发
        public void PostBuyProduct()
        {
            Console.WriteLine("终于买完了，现在要对东西分一下，相机是张三的；iPhone是李四的");
        }
    }


}
