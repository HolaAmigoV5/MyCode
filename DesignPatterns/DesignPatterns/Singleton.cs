using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns
{
    public sealed class Singleton
    {

        private static volatile Singleton uniqueInstance;

        //定义一个标识确保线程同步
        private static readonly object locker = new object();

        //定义私有构造函数，使外界不能创建该实例
        private Singleton() { }

        //定义公用方法提供一个全局访问点
        public static Singleton GetInstance()
        {
            if (uniqueInstance == null)
            {
                lock (locker)
                {
                    if (uniqueInstance == null)
                        return new Singleton();
                }
            }
            return uniqueInstance;
        }
    }
}
