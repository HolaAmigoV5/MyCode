using System;

namespace BridgePatternDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建一个遥控器
            RemoteControl remoteControl = new ConcreteRemote
            {
                //长虹电视机
                Implementor = new ChangHong()
            };
            remoteControl.On();
            remoteControl.SetChannel();
            remoteControl.Off();
            Console.WriteLine();

            //三星牌电视机
            remoteControl.Implementor = new Samsung();
            remoteControl.On();
            remoteControl.SetChannel();
            remoteControl.Off();
            Console.ReadLine();
        }
    }

    /// <summary>
    /// 抽象概念中的遥控器，扮演抽象化角色
    /// </summary>
    public class RemoteControl
    {
        //字段
        private TV implementor;

        public TV Implementor
        {
            get { return implementor; }
            set { implementor = value; }
        }

        /// <summary>
        /// 开电视机，这里抽象类中不再提供实现了，而是调用实现类中的实现
        /// </summary>
        public virtual void On()
        {
            implementor.On();
        }

        /// <summary>
        /// 关电视机
        /// </summary>
        public virtual void Off()
        {
            implementor.Off();
        }

        /// <summary>
        /// 换频道
        /// </summary>
        public virtual void SetChannel()
        {
            implementor.TuneChannel();
        }
    }

    /// <summary>
    /// 具体遥控器
    /// </summary>
    public class ConcreteRemote : RemoteControl
    {
        public override void SetChannel()
        {
            Console.WriteLine("------------------");
            base.SetChannel();
            Console.WriteLine("------------------");
        }
    }

    /// <summary>
    /// 电视机，提供抽象方法
    /// </summary>
    public abstract class TV
    {
        public abstract void On();
        public abstract void Off();
        public abstract void TuneChannel();
    }

    /// <summary>
    /// 长虹牌电视机，重写基类的抽象方法
    /// 提供具体的实现
    /// </summary>
    public class ChangHong : TV
    {
        public override void Off()
        {
            Console.WriteLine("长虹电视机已经关掉了");
        }

        public override void On()
        {
            Console.WriteLine("长虹电视机已经打开了");
        }

        public override void TuneChannel()
        {
            Console.WriteLine("长虹电视机换频道");
        }
    }

    /// <summary>
    /// 三星牌电视机，重写基类的抽象方法
    /// </summary>
    public class Samsung : TV
    {
        public override void Off()
        {
            Console.WriteLine("三星牌电视机已经关掉了");
        }

        public override void On()
        {
            Console.WriteLine("三星牌电视机已经开机了");
        }

        public override void TuneChannel()
        {
            Console.WriteLine("三星牌电视机换频道");
        }
    }
}
