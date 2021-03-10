using System;

namespace CommandPatternDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Receiver r = new Receiver();
            Command c = new ConcreteCommand(r);
            Invoke i = new Invoke(c);

            i.ExecuteCommand();
        }
    }

    /// <summary>
    /// 教官，负责调用命令对象执行请求
    /// </summary>
    public class Invoke
    {
        public Command _command;
        public Invoke(Command command)
        {
            _command = command;
        }

        public void ExecuteCommand()
        {
            _command.Action();
        }
    }

    /// <summary>
    /// 命令抽象类
    /// </summary>
    public abstract class Command
    {
        protected Receiver _receiver;
        public Command(Receiver receiver)
        {
            _receiver = receiver;
        }

        //命令执行方法
        public abstract void Action();
       
    }

    /// <summary>
    /// 具体命令
    /// </summary>
    public class ConcreteCommand : Command
    {
        public ConcreteCommand(Receiver receiver) : base(receiver) { }

        public override void Action()
        {
            _receiver.Run1000Meters();
        }
    }

    /// <summary>
    /// 命令接收者
    /// </summary>
    public class Receiver
    {
        public void Run1000Meters()
        {
            Console.WriteLine("跑1000米");
        }
    }
}
