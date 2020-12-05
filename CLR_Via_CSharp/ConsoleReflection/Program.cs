using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleReflection
{
    public class Program
    {
        static void Main(string[] args)
        {
            //ExceptionTree.Go();
            //MemberDiscover.Go();
            //Invoker.Go();
            TransferHandle.Go();
            Console.ReadLine();
        }
    }

    public static class ExceptionTree
    {
        public static void Go()
        {
            //显示加载要反射的程序集
            LoadAssemblies();

            //对所有类型进行筛选和排序
            var allTypes = (from a in AppDomain.CurrentDomain.GetAssemblies()
                            from t in a.ExportedTypes
                            where typeof(Exception).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo())
                            orderby t.Name
                            select t).ToArray();

            //生成并显示继承层次结构
            Console.WriteLine(WalkInheritanceHierarchy(new StringBuilder(), 0, typeof(Exception), allTypes));
        }

        private static StringBuilder WalkInheritanceHierarchy(StringBuilder sb, int indent, Type baseType, IEnumerable<Type> allTypes)
        {
            string spaces = new string(' ', indent * 3);
            sb.AppendLine(spaces + baseType.FullName);
            foreach (var t in allTypes)
            {
                if (t.GetTypeInfo().BaseType != baseType) continue;
                WalkInheritanceHierarchy(sb, indent + 1, t, allTypes);
            }
            return sb;
        }

        private static void LoadAssemblies()
        {
            string[] assemblies = {
                "System,                            PublicKeyToken={0}",
                "System.Core,                       PublicKeyToken={0}",
                "System.Data,                       PublicKeyToken={0}",
                "System.Design,                     PublicKeyToken={1}",
                "System.DirectoryServices,          PublicKeyToken={1}",
                "System.Drawing,                    PublicKeyToken={1}",
                "System.Drawing.Design,             PublicKeyToken={1}",
                "System.Management,                 PublicKeyToken={1}",
                "System.Messaging,                  PublicKeyToken={1}",
                "System.Runtime.Remoting,           PublicKeyToken={0}",
                "System.Security,                   PublicKeyToken={1}",
                "System.ServiceProcess,             PublicKeyToken={1}",
                "System.Web,                        PublicKeyToken={1}",
                "System.Web.RegularExpressions,     PublicKeyToken={1}",
                "System.Web.Services,               PublicKeyToken={1}",
                "System.Xml,                        PublicKeyToken={0}",
            };

            string EcmaPublicKeyToken = "b77a5c561934e089";
            string MSPublicKeyToken = "b03f5f7f11d50a3a";

            Version version = typeof(object).Assembly.GetName().Version;

            //显示加载想要反射的程序集
            foreach (string a in assemblies)
            {
                string AssemblyIdentity = string.Format(a, EcmaPublicKeyToken, MSPublicKeyToken) + ", Culture=neutral, Version=" + version;
                Assembly.Load(AssemblyIdentity);
            }
        }
    }

    public static class ConstructingGenericType
    {
        private sealed class Dictionary<TKey, TValue> { }

        public static void Go()
        {
            //获取泛型的类型对象的引用
            Type openType = typeof(Dictionary<,>);

            //使用TKey=String， TValue=int 封闭泛型类型
            Type closedType = openType.MakeGenericType(typeof(string), typeof(int));

            //构造封闭类型实例
            object o = Activator.CreateInstance(closedType);

            //验证构造的类型可以工作
            Console.WriteLine(o.GetType());
        }
    }

    public static class MemberDiscover
    {
        public static void Go()
        {
            //遍历当前AppDomain中加载的所有程序集
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                Show(0, "Assembly : {0}", a);

                //查找程序集中的类型
                foreach (Type t in a.ExportedTypes)
                {
                    Show(1, "Type : {0}", t);
                    
                    //发现类型成员
                    foreach (MemberInfo mi in t.GetTypeInfo().DeclaredMembers)
                    {
                        string typeName = string.Empty;
                        if (mi is Type) typeName = "(Nested) Type"; //嵌套类型
                        if (mi is FieldInfo) typeName = "FieldInfo";
                        if (mi is MethodInfo) typeName = "MethodInfo";
                        if (mi is ConstructorInfo) typeName = "ConstrucorInfo";
                        if (mi is PropertyInfo) typeName = "PropertyInfo";
                        if (mi is EventInfo) typeName = "EventInfo";
                        Show(2, "{0}: {1}", typeName, mi);
                    }
                }
            }
        }

        private static void Show(int indent, string format, params object[] args)
        {
            Console.WriteLine(new string(' ', 3 * indent) + format, args);
        }
    }

    //调用类型的成员
    internal static class Invoker
    {
        private sealed class SomeType
        {
            private int m_someField;
            public SomeType(ref int x) { x *= 2; }
            public override string ToString() { return m_someField.ToString(); }
            public int SomeProp
            {
                get { return m_someField; }
                set
                {
                    if (value < 1)
                        throw new ArgumentOutOfRangeException("value");
                    m_someField = value;
                }
            }

            public event EventHandler SomeEvent;
            private void NoCompilerWarnings() { SomeEvent.ToString(); }
        }

        public static void Go()
        {
            Type t = typeof(SomeType);
            BindToMemberThenInvokeTheMember(t);
            Console.WriteLine();

            BindToMemberCreateDelegateToMemberThenInvokeTheMember(t);
            Console.WriteLine();

            UseDynamicToBindAndInvokeTheMember(t);
            Console.WriteLine();
        }

        private static void BindToMemberThenInvokeTheMember(Type t)
        {
            Console.WriteLine("BindToMemberThenInvokeTheMember");

            Type ctorArgument = Type.GetType("System.Int32&"); //相当于typeof(int32).MakeByRefType();
            //Type ctoArgument2 = typeof(Int32).MakeByRefType();

            //构造实例
            ConstructorInfo ctor = t.GetTypeInfo().DeclaredConstructors
                .First(c => c.GetParameters()[0].ParameterType == ctorArgument);
            object[] args = new object[] { 12 }; //构造器的实参
            Console.WriteLine("x before constructor called: " + args[0]);
            object obj = ctor.Invoke(args);
            Console.WriteLine("Type: " + obj.GetType());
            Console.WriteLine("x after constructor returns: " + args[0]);

            //读写字段
            FieldInfo fi = obj.GetType().GetTypeInfo().GetDeclaredField("m_someField");
            fi.SetValue(obj, 33);
            Console.WriteLine("someField: " + fi.GetValue(obj));

            //调用方法
            MethodInfo mi = obj.GetType().GetTypeInfo().GetDeclaredMethod("ToString");
            string s = (string)mi.Invoke(obj, null);
            Console.WriteLine("ToString: " + s);

            //调用属性
            PropertyInfo pi = obj.GetType().GetTypeInfo().GetDeclaredProperty("SomeProp");
            try
            {
                pi.SetValue(obj, 0, null);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException.GetType() != typeof(ArgumentOutOfRangeException)) throw;
                Console.WriteLine("Property set catch " + e.Message);
            }
            pi.SetValue(obj, 2, null);
            Console.WriteLine("SomeProp: " + pi.GetValue(obj, null));

            //为事件添加和删除委托
            EventInfo ei = obj.GetType().GetTypeInfo().GetDeclaredEvent("SomeEvent");
            EventHandler eh = new EventHandler(EventCallback); //这是一个包装了回调方法的委托
            ei.AddEventHandler(obj, eh);
            ei.RemoveEventHandler(obj, eh);
        }

        private static void BindToMemberCreateDelegateToMemberThenInvokeTheMember(Type t)
        {
            Console.WriteLine("BindToMemberCreateDelegateToMemberThenInvokeTheMember");

            //构造实例（不能创建对构造器的委托）
            object[] args = new object[] { 12 }; //构造器实参
            Console.WriteLine("x before constructor called: " + args[0]);
            object obj = Activator.CreateInstance(t, args);
            Console.WriteLine("Type: " + obj.GetType().ToString());
            Console.WriteLine("x after constructor returns: " + args[0]);

            //调用方法
            MethodInfo mi = obj.GetType().GetTypeInfo().GetDeclaredMethod("ToString");
            var toString = mi.CreateDelegate<Func<string>>(obj);
            string s = toString();
            Console.WriteLine("ToString: " + s);

            //读写属性
            PropertyInfo pi = obj.GetType().GetTypeInfo().GetDeclaredProperty("SomeProp");
            var setSomeProp = pi.SetMethod.CreateDelegate<Action<int>>(obj);
            try
            {
                setSomeProp(0);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Property set catch " + e.Message);
            }

            setSomeProp(2);

            var getSomeProp = pi.GetMethod.CreateDelegate<Func<int>>(obj);
            Console.WriteLine("SomeProp: " + getSomeProp());

            //向事件增删委托
            EventInfo ei = obj.GetType().GetTypeInfo().GetDeclaredEvent("SomeEvent");
            var addSomeEvent = ei.AddMethod.CreateDelegate<Action<EventHandler>>(obj);
            addSomeEvent(EventCallback);
            var removeSomeEvent = ei.RemoveMethod.CreateDelegate<Action<EventHandler>>(obj);
            removeSomeEvent(EventCallback);
        }

        private static void UseDynamicToBindAndInvokeTheMember(Type t)
        {
            Console.WriteLine("UseDynamicToBindAndInvokeTheMember");

            //构造实例
            object[] args = new object[] { 12 };//构造器的实参
            Console.WriteLine("x before constructor called: " + args[0]);
            dynamic obj = Activator.CreateInstance(t, args);
            Console.WriteLine("Type: " + obj.GetType().ToString());
            Console.WriteLine("x after constructor returns: " + args[0]);

            //读写字段
            try
            {
                obj.m_someField = 5;
                int v = (int)obj.m_someField;
                Console.WriteLine("SomeField: " + v);
            }
            catch (RuntimeBinderException e)
            {
                Console.WriteLine("Failed to access field: " + e.Message);
            }

            //调用方法
            string s = (string)obj.ToString();
            Console.WriteLine("ToString: " + s);

            //读写属性
            try
            {
                obj.SomeProp = 0;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Property set catch " + e.Message);
            }
            obj.SomeProp = 2;
            int val = (int)obj.SomeProp;
            Console.WriteLine("SomeProp: " + val);

            //从事件增删委托
            obj.SomeEvent += new EventHandler(EventCallback);
            obj.SomeEvent -= new EventHandler(EventCallback);
        }

        //添加到事件的回调方法
        private static void EventCallback(object sender, EventArgs e) { }

    }

    internal static class TransferHandle
    {
        private const BindingFlags c_bf = BindingFlags.FlattenHierarchy | BindingFlags.Instance 
            | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public static void Go()
        {
            //显示在任何反射操作之前堆的大小
            Show("Before doing anything");

            //为MSCorlib.dll中的所有方法构建MethodInfo对象
            List<MethodBase> methodInfos = new List<MethodBase>();
            Type[] types = typeof(object).Assembly.GetExportedTypes();
            foreach (Type t in types)
            {
                //跳过泛型类型
                if (t.IsGenericTypeDefinition) continue;
                MethodBase[] mb = t.GetMethods(c_bf);
                methodInfos.AddRange(mb);
            }

            Console.WriteLine("# of methods = {0:N0}", methodInfos.Count);
            Show("After building cache of MethodInfo objects");


            //为所有MethodInfo对象构建RuntimeMethodhandle缓存
            List<RuntimeMethodHandle> methodHandles = methodInfos.ConvertAll<RuntimeMethodHandle>(mb => mb.MethodHandle);
            Show("Holding MethodInfo and RuntimeMethodHandle cache");
            GC.KeepAlive(methodInfos); //阻止缓存被过早垃圾回收

            methodInfos = null;  //现在允许缓存垃圾回收
            Show("After freeing MethodInfo objects");

            methodInfos = methodHandles.ConvertAll<MethodBase>(rmh => MethodBase.GetMethodFromHandle(rmh));
            Show("Size of heap after re-creating MethodInfo objects");
            GC.KeepAlive(methodHandles);
            GC.KeepAlive(methodInfos);

            methodHandles = null;
            methodInfos = null;
            Show("After freeing MethodInfos and RuntimeMethodHandles");
        }

        private static void Show(String s)
        {
            Console.WriteLine("Heap size = {0, 12:N0} - {1}", GC.GetTotalMemory(true), s);
        }
    }

    internal static class ReflectionExtensions
    {
        public static TDelegate CreateDelegate<TDelegate>(this MethodInfo mi, object target = null)
        {
            return (TDelegate)(object)mi.CreateDelegate(typeof(TDelegate), target);
        }
    }
}
