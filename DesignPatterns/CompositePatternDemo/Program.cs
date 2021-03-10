using System;
using System.Collections.Generic;

namespace CompositePatternDemo
{
    /* 安全式的组合模式，此方法实现的组合模式把管理子对象的方法声明在树枝构件ComplexGraphics类中
     * 这样如果叶子节点Line、Circle使用了Add或Remove方法时，就能在编译期间出现错误
     * 但这种方式虽然解决了透明式组合模式的问题，但是它使得叶子节点和树枝构件具有不一样的接口
     * 所以两种方式实现的组合模式各有优缺点，具体使用哪个，可以根据问题的实际情况而定
     * */
    class Program
    {
        static void Main(string[] args)
        {
            ComplexGraphics complexGraphics = new ComplexGraphics("一个复杂图形和两条线段组成的复杂图形");
            complexGraphics.Add(new Line("线段A"));
            ComplexGraphics compositeCG = new ComplexGraphics("一个圆和一条线组成的复杂图形");
            compositeCG.Add(new Circle("圆"));
            compositeCG.Add(new Circle("线段B"));
            complexGraphics.Add(compositeCG);

            Line l = new Line("线段C");
            complexGraphics.Add(l);

            //显示复杂图形的画法
            Console.WriteLine("复杂图形的绘制如下：");
            Console.WriteLine("---------------------");
            complexGraphics.Draw();
            Console.WriteLine("复杂图形绘制完成");
            Console.WriteLine("---------------------");
            Console.WriteLine();


            //移除一个组件再显示显示复杂图形的画法
            complexGraphics.Remove(l);
            Console.WriteLine("移除线段C后，复杂图形的绘制如下：");
            Console.WriteLine("---------------------------");
            complexGraphics.Draw();
            Console.WriteLine("复杂图形绘制完成");
            Console.WriteLine("----------------------------");
            Console.ReadLine();
        }
    }

    /// <summary>
    /// 图形抽象类
    /// </summary>

    public abstract class Graphics
    {
        public string Name { get; set; }
        public Graphics(string name)
        {
            this.Name = name;
        }

        public abstract void Draw();

        //移除Add和Remove方法，把管理子对象的方法放到ComplexGraphics类中进行管理
        //因为这些方法只在复杂图形中才有意义
        //public abstract void Add(Graphics g);
        //public abstract void Remove(Graphics g);
    }

    /// <summary>
    /// 简单图形类——线
    /// </summary>
    public class Line : Graphics
    {
        public Line(string name) : base(name) { }

        /// <summary>
        /// 因为简单图形在添加或移除其他图形，所以简单图形Add或Remove方法没有任何意义
        /// 如果客户端调用了简单图形的Add或Remove方法将会在运行时抛出异常
        /// 我们可以在客户端捕获该类移除并处理
        /// </summary>
        /// <param name="g"></param>
        //public override void Add(Graphics g)
        //{
        //    throw new Exception("不能向简单图形Line添加其他图形");
        //}

        /// <summary>
        /// 重写父类抽象方法
        /// </summary>
        public override void Draw()
        {
            Console.WriteLine("画 " + Name);
        }

        //public override void Remove(Graphics g)
        //{
        //    throw new Exception("不能向简单图形Line移除其他图形");
        //}
    }

    /// <summary>
    /// 简单图形类——圆
    /// </summary>
    public class Circle : Graphics
    {
        public Circle(string name) : base(name) { }

        //public override void Add(Graphics g)
        //{
        //    throw new Exception("不能向简单图形Circle添加其他图形");
        //}

        /// <summary>
        /// 重写父类抽象方法
        /// </summary>
        public override void Draw()
        {
            Console.WriteLine("画 " + Name);
        }

        //public override void Remove(Graphics g)
        //{
        //    throw new Exception("不能向简单图形Circle移除其他图形");
        //}
    }

    /// <summary>
    /// 复杂图形，由一些简单图形组成，这里假设该复杂图形由一个圆两条线组成的复杂图形
    /// </summary>
    public class ComplexGraphics : Graphics
    {
        private List<Graphics> complexGraphicsList = new List<Graphics>();
        public ComplexGraphics(string name) : base(name) { }

        public void Add(Graphics g)
        {
            complexGraphicsList.Add(g);
        }

        /// <summary>
        /// 复杂图形的画法
        /// </summary>
        public override void Draw()
        {
            foreach (Graphics g in complexGraphicsList)
            {
                g.Draw();
            }
        }

        public void Remove(Graphics g)
        {
            complexGraphicsList.Remove(g);
        }
    }
}
