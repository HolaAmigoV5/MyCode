using System;

namespace ClassLibraryDemo
{
    public abstract class BaseClass
    {
        public struct DemoStruct { }
        public delegate void DemoDelegate(object sender, EventArgs e);
        public enum DemoEnum { terrible, bad, common = 4, good, wonderful = 8 }
        public interface IDemoInterface
        {
            void SayGreeting(string name);
        }

        public interface IDemoInterface2 { }

        [Record("更新", "wby", "2020-6-16", Memo = "修改ToString()方法")]
        [Record("创建", "wby", "2020-5-1")]
        public sealed class DemoClass : BaseClass, IDemoInterface, IDemoInterface2
        {
            private string name;
            public string city;
            public readonly string title;
            public const string text = "Const Field";
            public event DemoDelegate MyEvent;

            public string Name
            {
                private get { return name; }
                set { name = value; }
            }

            public DemoClass()
            {
                title = "Readonly Field";
            }

            public class NestedClass { }

            public void SayGreeting(string name)
            {
                Console.WriteLine("Morning :" + name);
            }

            public override string ToString()
            {
                return "This is a demo class";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class RecordAttribute : Attribute
    {
        private string recordType;  //记录类型：更新/创建
        private string author;      //作者
        private DateTime date;      //更新/创建日期
        private string memo;        //备注

        public RecordAttribute(string recordType, string author, string date)
        {
            this.recordType = recordType;
            this.author = author;
            this.date = Convert.ToDateTime(date);
        }

        //位置参数，通常只提供get访问器
        public string RecordType { get { return recordType; } }
        public string Author { get { return author; } }
        public DateTime Date { get { return date; } }

        //构建一个属性，在特性中这个叫“命名参数”
        public string Memo { get { return memo; } set { memo = value; } }
    }
}
