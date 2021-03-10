using System;
using System.Collections;

namespace VistorPatternDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjectStructure objectStructure = new ObjectStructure();
            foreach (Element e in objectStructure.Elements)
            {
                //每个元素接受访问者访问
                e.Accept(new ConcreteVistor());
            }

            Console.ReadLine();
        }
    }

    /// <summary>
    /// 抽象元素角色
    /// </summary>
    public abstract class Element
    {
        public abstract void Accept(IVistor vistor);
        public abstract void Print();
    }

    /// <summary>
    /// 具体元素A
    /// </summary>
    public class ElementA : Element
    {
        public override void Accept(IVistor vistor)
        {
            //调用访问者visit方法
            vistor.Visit(this);
        }

        public override void Print()
        {
            Console.WriteLine("这是元素A");
        }
    }


    /// <summary>
    /// 具体元素B
    /// </summary>
    public class ElementB : Element
    {
        public override void Accept(IVistor vistor)
        {
            vistor.Visit(this);
        }

        public override void Print()
        {
            Console.WriteLine("我是元素B");
        }
    }

    /// <summary>
    /// 抽象访问者
    /// </summary>
    public interface IVistor
    {
        void Visit(ElementA a);
        void Visit(ElementB b);
    }

    /// <summary>
    /// 具体访问者
    /// </summary>
    public class ConcreteVistor : IVistor
    {
        //visit方法而是再去调用元素的Accept方法
        public void Visit(ElementA a)
        {
            a.Print();
        }

        public void Visit(ElementB b)
        {
            b.Print();
        }
    }


    /// <summary>
    /// 对象结构
    /// </summary>
    public class ObjectStructure
    {
        private ArrayList elements = new ArrayList();
        public ArrayList Elements
        {
            get { return elements; }
        }

        public ObjectStructure()
        {
            Random ran = new Random();
            for (int i = 0; i < 6; i++)
            {
                int ranNum = ran.Next(10);
                if (ranNum > 5)
                    elements.Add(new ElementA());
                else
                    elements.Add(new ElementB());
            }
        }
    }
}
