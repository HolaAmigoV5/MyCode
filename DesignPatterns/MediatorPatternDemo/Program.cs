using System;

namespace MediatorPatternDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            AbstractCardPartner A = new ParterA();
            AbstractCardPartner B = new ParterB();

            //初始钱
            A.MoneyCount = 20;
            B.MoneyCount = 20;

            AbstractMediator mediator = new MediatorPater(A, B);

            //A赢了
            A.ChangeCount(5, mediator);
            Console.WriteLine("A 现在的钱是：{0}", A.MoneyCount);
            Console.WriteLine("B 现在的钱是：{0}", B.MoneyCount);

            //B赢了
            B.ChangeCount(10, mediator);
            Console.WriteLine("A 现在的钱是：{0}", A.MoneyCount);
            Console.WriteLine("B 现在的钱是：{0}", B.MoneyCount);

            Console.ReadLine();
        }
    }

    /// <summary>
    /// 抽象牌友
    /// </summary>
    public abstract class AbstractCardPartner
    {
        public int MoneyCount { get; set; }
        public AbstractCardPartner()
        {
            MoneyCount = 0;
        }

        public abstract void ChangeCount(int count, AbstractMediator mediator);
    }

    /// <summary>
    /// 具体牌友A
    /// </summary>
    public class ParterA : AbstractCardPartner
    {
        public override void ChangeCount(int count, AbstractMediator mediator)
        {
            mediator.AWin(count);
        }
    }

    /// <summary>
    /// 具体牌友B
    /// </summary>
    public class ParterB : AbstractCardPartner
    {
        public override void ChangeCount(int count, AbstractMediator mediator)
        {
            mediator.BWin(count);
        }
    }

    /// <summary>
    /// 抽象中介者
    /// </summary>
    public abstract class AbstractMediator
    {
        protected AbstractCardPartner A;
        protected AbstractCardPartner B;
        public AbstractMediator(AbstractCardPartner a, AbstractCardPartner b)
        {
            A = a;
            B = b;
        }

        public abstract void AWin(int count);
        public abstract void BWin(int count);
    }

    /// <summary>
    /// 具体中介者类
    /// </summary>
    public class MediatorPater : AbstractMediator
    {
        public MediatorPater(AbstractCardPartner a, AbstractCardPartner b) : base(a, b) { }

        public override void AWin(int count)
        {
            A.MoneyCount += count;
            B.MoneyCount -= count;
        }

        public override void BWin(int count)
        {
            B.MoneyCount += count;
            A.MoneyCount -= count;
        }
    }
}
