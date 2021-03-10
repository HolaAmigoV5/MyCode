using System;

namespace PrototypePatternDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            //原型模式与工厂方法模式的实现非常相似，其中原型模式中的Clone方法就类似于工厂方法模式中的工厂方法，只是工厂方法模式的工厂方法
            //是通过new运算符重新创建一个新的对象（相当于原型模式的深拷贝）,而原型模式是通过MemberwiseClone方法进行浅拷贝


            MonkeyKingPrototype prototypeMonkeyKing = new ConcreteProtoType("MonkeyKing");

            //变一个
            MonkeyKingPrototype cloneMonkeyKing = prototypeMonkeyKing.Clone() as ConcreteProtoType;
            Console.WriteLine("Cloned:\t" + cloneMonkeyKing.Id);


            //变两个
            MonkeyKingPrototype cloneMonkeyKing2 = prototypeMonkeyKing.Clone() as ConcreteProtoType;
            Console.WriteLine("Cloned2:\t" + cloneMonkeyKing2.Id);
            Console.ReadLine();

        }
    }

    /// <summary>
    /// 猴王原型
    /// </summary>
    public abstract class MonkeyKingPrototype
    {
        public string Id { get; set; }
        public MonkeyKingPrototype(string id)
        {
            this.Id = id;
        }

        //克隆方法
        public abstract MonkeyKingPrototype Clone();
    }

    /// <summary>
    /// 创建具体原型
    /// </summary>
    public class ConcreteProtoType : MonkeyKingPrototype
    {
        public ConcreteProtoType(string id) : base(id) { }

        /// <summary>
        /// 浅拷贝
        /// </summary>
        /// <returns></returns>
        public override MonkeyKingPrototype Clone()
        {
            //调用MemberwiseClone方法实现的是浅拷贝，另外还有深拷贝。
            return (MonkeyKingPrototype)this.MemberwiseClone();
        }
    }
}
