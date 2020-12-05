using ClassLibraryDemo;
using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;

namespace ConsoleReflection
{
    public class Program
    {
        static void Main(string[] args)
        {
            //AssemblyExplore();
            //Type t = typeof(ClassLibraryDemo.BaseClass);
            //TypeExplore(t);

            //MemberExplore(typeof(ClassLibraryDemo.BaseClass.DemoClass));
            //FieldExplore(typeof(ClassLibraryDemo.BaseClass.DemoClass));
            //PropertyExplore(typeof(ClassLibraryDemo.BaseClass.DemoClass));
            //MethodExplore(typeof(ClassLibraryDemo.BaseClass.DemoClass));

            //DemoClass demo = new DemoClass();
            //Console.WriteLine(demo.ToString());

            //Type type = typeof(DemoClass);
            //Console.WriteLine("下面列出应用于 {0} 的RecordAttribute属性：", type);

            ////获取所有RecordAttributes特性
            //object[] records = type.GetCustomAttributes(typeof(RecordAttribute), false);
            //foreach (RecordAttribute record in records)
            //{
            //    Console.WriteLine("\n\n");
            //    Console.WriteLine($"     {record}");
            //    Console.WriteLine($"    类型：{record.RecordType}");
            //    Console.WriteLine($"    作者：{record.Author}");
            //    Console.WriteLine($"    日期：{record.Date.ToShortDateString()}");
            //    if (!string.IsNullOrEmpty(record.Memo))
            //        Console.WriteLine($"    备注：{record.Memo}");
            //}


            //CreateInstanceNoArgs();
            //CreateInstanceWithArgs();
            InvokeMemberCallMethod();
            Console.ReadLine();
        }

        public static void CreateInstanceNoArgs()
        {
            //Assembly asm = Assembly.GetExecutingAssembly();
            //object obj = asm.CreateInstance("ConsoleReflection.Calculator", true);

            ObjectHandle handler = Activator.CreateInstance(null, "ConsoleReflection.Calculator");
            handler.Unwrap();
        }

        public static void CreateInstanceWithArgs()
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            object[] parameters = new object[2];
            parameters[0] = 2;
            parameters[1] = 5;

            object obj = ass.CreateInstance("ConsoleReflection.Calculator", true, 
                BindingFlags.Default, null, parameters, null, null);
        }

        public static void InvokeMemberCallMethod()
        {
            Type type = typeof(Calculator);
            //object obj = Activator.CreateInstance(type, 3, 5);
            //int result = (int)type.InvokeMember("Add", BindingFlags.InvokeMethod, null, obj, null);

            object[] parameters = { 3, 7 };
            type.InvokeMember("Add", BindingFlags.InvokeMethod, null, type, parameters);
        }

        public static void MemberInfoInvoke()
        {
            Type t = typeof(Calculator);
            object obj = Activator.CreateInstance(t, 3, 5);
            MethodInfo mi = t.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            mi.Invoke(obj, null);
        }

        public static void AssemblyExplore()
        {
            StringBuilder sb = new StringBuilder();
            Assembly asm = Assembly.Load("ClassLibraryDemo");

            sb.AppendLine("FullName(全名)：" + asm.FullName);
            sb.AppendLine("Location(路径)：" + asm.Location);

            Module[] modules= asm.GetModules();
            foreach (Module module in modules)
            {
                sb.AppendLine("模块：" + module);
                Type[] types = module.GetTypes();
                foreach (Type type in types)
                {
                    sb.AppendLine("    类型：" + type);
                }
            }
            Console.WriteLine(sb.ToString());
        }

        public static void TypeExplore(Type t)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("名称信息：\n");
            sb.Append("Name：" + t.Name + "\n");
            sb.Append("FullName：" + t.FullName + "\n");
            sb.Append("Namespace：" + t.Namespace + "\n");

            sb.Append("\n其他信息：\n");
            sb.Append("BaseType(基类型)：" + t.BaseType + "\n");
            sb.Append("UnderlyingSystemType：" + t.UnderlyingSystemType + "\n");

            sb.Append("\n类型信息：\n");
            sb.Append("Attributes(TypeAttributes位标记)：" + t.Attributes + "\n");
            sb.Append("IsValueType(值类型): " + t.IsValueType + "\n");
            sb.Append("IsEnum(枚举): " + t.IsEnum + "\n");
            sb.Append("IsClass(类): " + t.IsClass + "\n");
            sb.Append("IsArray(数组): " + t.IsArray + "\n");
            sb.Append("IsInterface(接口): " + t.IsInterface + "\n");

            sb.Append("IsPointer(指针): " + t.IsPointer + "\n");
            sb.Append("IsSealed(密封): " + t.IsSealed + "\n");
            sb.Append("IsPrimitive(基类型): " + t.IsPrimitive + "\n"); 
            sb.Append("IsAbstract(抽象): " + t.IsAbstract + "\n"); 
            sb.Append("IsPublic(公开): " + t.IsPublic + "\n");
            sb.Append("IsNotPublic(不公开): " + t.IsNotPublic + "\n"); 
            sb.Append("IsVisible: " + t.IsVisible + "\n");
            sb.Append("IsByRef(由引用传递): " + t.IsByRef + "\n");
            Console.WriteLine(sb.ToString());
        }

        public static void MemberExplore(Type t)
        {
            StringBuilder sb = new StringBuilder();
            //MemberInfo[] members = t.GetMembers(BindingFlags.Public|BindingFlags.Static|
            //    BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly);
            MemberInfo[] members = t.FindMembers(MemberTypes.Method,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.DeclaredOnly, Type.FilterName, "get*");
            sb.Append("查看类型 " + t.Name + "的成员信息：\n");

            foreach (MemberInfo mi in members)
            {
                sb.Append("成员：" + mi.ToString().PadRight(50) + " 类型：" + mi.MemberType + "\n");
            }
            Console.WriteLine(sb.ToString());
        }

        public static void FieldExplore(Type t)
        {
            StringBuilder sb = new StringBuilder();
            FieldInfo[] fields = t.GetFields();
            sb.Append("查看类型 " + t.Name + "的字段信息：\n");
            sb.Append(string.Empty.PadLeft(50, '-') + "\n");

            foreach (FieldInfo fi in fields)
            {
                sb.Append("名称：" + fi.Name + "\n");
                sb.Append("类型：" + fi.FieldType + "\n");
                sb.Append("特性：" + fi.Attributes + "\n\n");
            }
            Console.WriteLine(sb.ToString());
        }

        public static void PropertyExplore(Type t)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("查看类型 " + t.Name + "的属性信息：\n");
            sb.Append(string.Empty.PadLeft(50, '-') + "\n");

            PropertyInfo[] properties = t.GetProperties();

            foreach (PropertyInfo pi in properties)
            {
                sb.Append("名称：" + pi.Name + "\n");
                sb.Append("类型：" + pi.PropertyType + "\n");
                sb.Append("可读：" + pi.CanRead + "\n");
                sb.Append("可写：" + pi.CanWrite + "\n");
                sb.Append("特性：" + pi.Attributes + "\n\n");
            }
            Console.WriteLine(sb.ToString());
        }

        public static void MethodExplore(Type t)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("查看类型 " + t.Name + "的方法信息：\n");
            sb.Append(string.Empty.PadLeft(50, '-') + "\n");

            MethodInfo[] methods = t.GetMethods();

            foreach (MethodInfo method in methods)
            {
                sb.Append("名称：" + method.Name + "\n");
                sb.Append("签名：" + method.ToString() + "\n");
                sb.Append("特性：" + method.Attributes + "\n");
                sb.Append("返回值类型：" + method.ReturnType + "\n\n");
            }
            Console.WriteLine(sb.ToString());
        }
    }

    public class Calculator
    {
        private int x;
        private int y;
        public Calculator()
        {
            x = 0;
            y = 0;
            Console.WriteLine("Calculator() invoked");
        }

        public Calculator(int x, int y)
        {
            this.x = x;
            this.y = y;
            Console.WriteLine("Calculator(int x, int y) invoked");
        }

        public int Add()
        {
            int total = 0;
            total = x + y;
            Console.WriteLine("Invoke Instance Method: ");
            Console.WriteLine(string.Format("[Add]：{0} + {1} = {2}", x, y, total));
            return total;
        }

        public static void Add(int x, int y)
        {
            int total = x + y;
            Console.WriteLine("Invoke Static Method: ");
            Console.WriteLine(string.Format("[Add]：{0} + {1} = {2}", x, y, total));
        }
    }
}
