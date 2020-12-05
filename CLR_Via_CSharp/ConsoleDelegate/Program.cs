using System;
using System.Linq;
using System.Reflection;

namespace ConsoleDelegate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DelegateReflection.Go("ConsoleDelegate.TwoInt");
            DelegateReflection.Go("ConsoleDelegate.TwoInt", "Add", "123", "321");
            DelegateReflection.Go("ConsoleDelegate.TwoInt", "Subtract", "123", "321");
            DelegateReflection.Go("ConsoleDelegate.OneString", "NumChars", "Hello there");
            DelegateReflection.Go("ConsoleDelegate.OneString", "Reverse", "Hello there");
            Console.ReadLine();
        }
    }

    //定义委托
    internal delegate object TwoInt(int n1, int n2);
    internal delegate object OneString(string s1);

    internal static class DelegateReflection
    {
        public static void Go(params string[] args)
        {
            if (args.Length < 2)
            {
                string usage = @"Usage:" + "{0} delType methodName [Arg1] [Arg2]" +
                     "{0}    where delType must be TwoInt or OneString" +
                     "{0}    if delType is TwoInt, methodName must be Add or Subtract" +
                     "{0}    if delType is OneString, methodName must be NumChars or Reverse" +
                     "{0}" +
                     "{0} Examples:" +
                     "{0}  TwoInt Add 123 321" +
                     "{0}  TwoInt Subtract 123 321" +
                     "{0}  OneString NumChars \"Hello there\"" +
                     "{0}  OneString Reverse \"Hello there\"";
                Console.WriteLine(usage, Environment.NewLine);
                return;
            }

            //将delType实参转换为委托类型
            Type delType = Type.GetType(args[0]);
            if (delType == null)
            {
                Console.WriteLine("Invalid delType argument:" + args[0]);
                return;
            }

            Delegate d;
            try
            {
                //将Arg1实参转换为方法
                MethodInfo mi = typeof(DelegateReflection).GetTypeInfo().GetDeclaredMethod(args[1]);
                //创建包装了静态方法的委托对象
                d = mi.CreateDelegate(delType);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid methodName agument:" + args[1]);
                return;
            }

            //创建一个数组，其中包含要通过委托对象传递给方法的参数
            object[] callbackArgs = new object[args.Length - 2];
            if (d.GetType() == typeof(TwoInt))
            {
                try
                {
                    //将string类型的参数转换为int类型的参数
                    for (int i = 2; i < args.Length; i++)
                    {
                        callbackArgs[i - 2] = Int32.Parse(args[i]);
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Parameters must be integers.");
                    return;
                }
            }

            if (d.GetType() == typeof(OneString))
            {
                //只复制string参数
                Array.Copy(args, 2, callbackArgs, 0, callbackArgs.Length);
            }
            try
            {
                //调用委托并显示结果
                object result = d.DynamicInvoke(callbackArgs);
                Console.WriteLine("Result = " + result);
            }
            catch (TargetParameterCountException)
            {
                Console.WriteLine("Incorrect number of parameters specified");
            }
        }

        private static object Add(int n1, int n2)
        {
            return n1 + n2;
        }

        private static object Subtract(int n1, int n2)
        {
            return n1 - n2;
        }

        private static object NumChars(string s)
        {
            return s.Length;
        }

        private static object Reverse(string s)
        {
            return new string(s.Reverse().ToArray());
        }
    }
}
