using System;
using System.Collections.Generic;

namespace BuilderPatternDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建指挥者和构造者
            Director director = new Director();
            Builder b1 = new ConcretBuilder1();
            Builder b2 = new ConcretBuilder2();

            //老板叫员工1去组装第一台电脑
            director.Construct(b1);

            //组装完成，组装人员搬来组装好的电脑
            Computer computer1 = b1.GetComputer();
            computer1.Show();


            //老板让员工2去组装电脑，并搬回来
            director.Construct(b2);
            Computer computer2 = b2.GetComputer();
            computer2.Show();


            Console.ReadLine();
        }
    }

    /// <summary>
    /// 指挥者，指挥创建过程类
    /// </summary>
    public class Director
    {
        public void Construct(Builder builder)
        {
            builder.BuildPartCPU();
            builder.BuildPartMainBoard();
        }
    }

    public class Computer
    {
        //电脑组件集合
        private readonly IList<string> parts = new List<string>();


        //添加电脑组件到集合中
        public void Add(string part)
        {
            parts.Add(part);
        }

        public void Show()
        {
            Console.WriteLine("电脑开始组装....");
            foreach (var part in parts)
            {
                Console.WriteLine("组件" + part + "已经安装好！");
            }
            Console.WriteLine("电脑组装好了....");
        }

    }

    public abstract class Builder
    {
        //装CPU
        public abstract void BuildPartCPU();

        //装主板
        public abstract void BuildPartMainBoard();

        //获得组装好的电脑
        public abstract Computer GetComputer();

    }

    public class ConcretBuilder1 : Builder
    {
        Computer computer = new Computer();
        public override void BuildPartCPU()
        {
            computer.Add("CPU1");
        }

        public override void BuildPartMainBoard()
        {
            computer.Add("Main board1");
        }

        public override Computer GetComputer()
        {
            return computer;
        }
    }

    public class ConcretBuilder2 : Builder
    {
        Computer computer = new Computer();
        public override void BuildPartCPU()
        {
            computer.Add("CPU2");
        }

        public override void BuildPartMainBoard()
        {
            computer.Add("Main board2");
        }

        public override Computer GetComputer()
        {
            return computer;
        }
    }
}
