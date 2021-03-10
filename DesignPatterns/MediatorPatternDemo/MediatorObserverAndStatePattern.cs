using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorPatternDemo1
{
    class Program
    {
        static void Main(string[] args)
        {
            AbstractCardPartner A = new ParterA();
            AbstractCardPartner B = new ParterB();
            // 初始钱
            A.MoneyCount = 20;
            B.MoneyCount = 20;

            AbstractMediator mediator = new MediatorPater(new InitState());

            // A,B玩家进入平台进行游戏
            mediator.Enter(A);
            mediator.Enter(B);

            // A赢了
            mediator.State = new AWinState(mediator);
            mediator.ChangeCount(5);
            Console.WriteLine("A 现在的钱是：{0}", A.MoneyCount);// 应该是25
            Console.WriteLine("B 现在的钱是：{0}", B.MoneyCount); // 应该是15

            // B赢了
            mediator.State = new BWinState(mediator);
            mediator.ChangeCount(10);
            Console.WriteLine("A 现在的钱是：{0}", A.MoneyCount);// 应该是25
            Console.WriteLine("B 现在的钱是：{0}", B.MoneyCount); // 应该是15
            Console.Read();
        }
    }

    // 抽象牌友类
    public abstract class AbstractCardPartner
    {
        public int MoneyCount { get; set; }

        public AbstractCardPartner()
        {
            MoneyCount = 0;
        }

        public abstract void ChangeCount(int Count, AbstractMediator mediator);
    }

    // 牌友A类
    public class ParterA : AbstractCardPartner
    {
        // 依赖与抽象中介者对象
        public override void ChangeCount(int Count, AbstractMediator mediator)
        {
            mediator.ChangeCount(Count);
        }
    }

    // 牌友B类
    public class ParterB : AbstractCardPartner
    {
        // 依赖与抽象中介者对象
        public override void ChangeCount(int Count, AbstractMediator mediator)
        {
            mediator.ChangeCount(Count);
        }
    }

    // 抽象状态类
    public abstract class State
    {
        protected AbstractMediator meditor;
        public abstract void ChangeCount(int count);
    }

    // A赢状态类
    public class AWinState : State
    {
        public AWinState(AbstractMediator concretemediator)
        {
            this.meditor = concretemediator;
        }

        public override void ChangeCount(int count)
        {
            foreach (AbstractCardPartner p in meditor.list)
            {
                if (p is ParterA a)
                {
                    a.MoneyCount += count;
                }
                else
                {
                    p.MoneyCount -= count;
                }
            }
        }
    }

    // B赢状态类
    public class BWinState : State
    {
        public BWinState(AbstractMediator concretemediator)
        {
            this.meditor = concretemediator;
        }

        public override void ChangeCount(int count)
        {
            foreach (AbstractCardPartner p in meditor.list)
            {
                // 如果集合对象中时B对象，则对B的钱添加
                if (p is ParterB b)
                {
                    b.MoneyCount += count;
                }
                else
                {
                    p.MoneyCount -= count;
                }
            }
        }
    }

    // 初始化状态类
    public class InitState : State
    {
        public InitState()
        {
            Console.WriteLine("游戏才刚刚开始,暂时还有玩家胜出");
        }

        public override void ChangeCount(int count)
        {
            //
            return;
        }
    }

    // 抽象中介者类
    public abstract class AbstractMediator
    {
        public List<AbstractCardPartner> list = new List<AbstractCardPartner>();

        public State State { get; set; }

        public AbstractMediator(State state)
        {
            this.State = state;
        }

        public void Enter(AbstractCardPartner partner)
        {
            list.Add(partner);
        }

        public void Exit(AbstractCardPartner partner)
        {
            list.Remove(partner);
        }

        public void ChangeCount(int count)
        {
            State.ChangeCount(count);
        }
    }

    // 具体中介者类
    public class MediatorPater : AbstractMediator
    {
        public MediatorPater(State initState) : base(initState) { }
    }
}
