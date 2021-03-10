using System;

namespace AdapterPatternDemo
{
    //类的适配器模式实现
    class Program
    {
        static void Main(string[] args)
        {
            //现在客户端可以通过电话配要使用2个孔的插头了
            IThreeHole threeHole = new PowerAdapter();
            threeHole.Request();
            Console.ReadLine();
        }
    }

    /// <summary>
    /// 三个孔插头，也就是适配器模式中的目标角色
    /// </summary>
    public interface IThreeHole
    {
        void Request();
    }

    /// <summary>
    /// 两个孔的插头，源角色——需要适配器的类
    /// </summary>
    public abstract class TwoHole
    {
        public void SpecificRequest()
        {
            Console.WriteLine("我是两个孔的插头");
        }
    }

    /// <summary>
    /// 适配器类，接口要放在类的后面
    /// </summary>
    public class PowerAdapter : TwoHole, IThreeHole
    {
        /// <summary>
        /// 实现三个孔接口方法
        /// </summary>
        public void Request()
        {
            //调用两个孔插头方法
            this.SpecificRequest();
        }
    }
}
