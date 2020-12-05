using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ConsoleAttribute
{
    [Serializable]
    [DefaultMember("Main")]
    [DebuggerDisplay("Richter", Name = "Jeff", Target = typeof(Program))]
    public sealed class Program
    {
        public Program()
        {

        }
        
        //[CLSCompliant(true)]
        [STAThread]
        public static void Main(string[] args)
        {
            ShowAttributes(typeof(Program));

            //获取与类型关联的方法集
            var members = from m in typeof(Program).GetTypeInfo().DeclaredMembers.OfType<MethodBase>()
                          where m.IsPublic
                          select m;
            foreach (MemberInfo member in members)
            {
                ShowAttributes(member);
            }

            Console.ReadLine();
        }

        [Conditional("Debug")]
        [Conditional("Release")]
        public void DoSomething() { }

        private static void ShowAttributes(MemberInfo attributeTarget)
        {
            IList<CustomAttributeData> caList = CustomAttributeData.GetCustomAttributes(attributeTarget);
            Console.WriteLine("Attributes applied to {0}:{1}",
                attributeTarget.Name, caList.Count() == 0 ? "None" : string.Empty);

            foreach (CustomAttributeData attributeData in caList)
            {
                //显示所应用的每个特性的类型
                Type t = attributeData.Constructor.DeclaringType;
                Console.WriteLine(" {0}", t.ToString());
                Console.WriteLine("   Constructor called={0}", attributeData.Constructor);

                IList<CustomAttributeTypedArgument> posArgs = attributeData.ConstructorArguments;
                Console.WriteLine("  Positional arguments passed to constructor:" 
                    + ((posArgs.Count == 0) ? " None" : string.Empty));
                foreach (CustomAttributeTypedArgument pa in posArgs)
                {
                    Console.WriteLine("   Type={0}, Value={1}", pa.ArgumentType, pa.Value);
                }

                IList<CustomAttributeNamedArgument> namedArgs = attributeData.NamedArguments;
                Console.WriteLine("   Named arguments set after construction:" 
                    + ((namedArgs.Count() == 0) ? " None" : string.Empty));

                foreach (CustomAttributeNamedArgument na in namedArgs)
                {
                    Console.WriteLine("   Name={0}, Type={1}, Value={2}",
                        na.MemberInfo.Name, na.TypedValue.ArgumentType, na.TypedValue.Value);
                }
                Console.WriteLine();
            }

            //var attributes = attributeTarget.GetCustomAttributes<Attribute>();
            //Console.WriteLine("Attributes applied to {0}:{1}", 
            //    attributeTarget.Name, attributes.Count() == 0 ? "None" : string.Empty);

            //foreach (Attribute attribute in attributes)
            //{
            //    //显示所应用的每个特性的类型
            //    Console.WriteLine(" {0}", attribute.GetType().ToString());

            //    if (attribute is DefaultMemberAttribute dma)
            //        Console.WriteLine(" MemberName={0}", dma.MemberName);
            //    if (attribute is ConditionalAttribute ca)
            //        Console.WriteLine(" ConditionString={0}", ca.ConditionString);
            //    if (attribute is CLSCompliantAttribute CLSCa)
            //        Console.WriteLine(" IsCompliant={0}", CLSCa.IsCompliant);

            //    if (attribute is DebuggerDisplayAttribute da)
            //        Console.WriteLine(" Value={0}, Name={1}, Target={2}", da.Value, da.Name, da.Target);
            //}

            Console.WriteLine();
        }

    }
}
