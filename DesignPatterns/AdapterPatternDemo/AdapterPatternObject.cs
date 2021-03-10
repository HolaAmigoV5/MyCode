using System;

namespace AdapterPatternDemo2
{
    class AdapterPatternObject
    {
        static void Main()
        {
            //现在客户端可以通过适配器使用2个孔的插头了
            ThreeHole threeHole = new PowerAdapter();
            threeHole.Request();
            Console.ReadLine();
        }
    }

    /// <summary>
    /// 三个孔的插头，也就是适配器模式中的目标(Target)角色
    /// </summary>
    public class ThreeHole
    {
        //客户端需要的方法
        public virtual void Request()
        {
            //可以把一般实现放在这里
        }
    }

    /// <summary>
    /// 两个孔的插头，源角色——需要适配的类
    /// </summary>
    public class TwoHole
    {
        public void SpecificRequest()
        {
            Console.WriteLine("我是两个孔的插头");
        }
    }


    /// <summary>
    /// 适配器类，这里适配器类没有TwoHole类
    /// 而引用了TwoHole对象，所以是对象的适配器模式实现
    /// </summary>
    public class PowerAdapter : ThreeHole
    {
        //引用两个孔插头的实例，从而将客户端与TwoHole联系起来
        public TwoHole twoholeAdapter = new TwoHole();

        /// <summary>
        /// 实现三孔插头接口方法
        /// </summary>
        public override void Request()
        {
            twoholeAdapter.SpecificRequest();
        }
    }
}
