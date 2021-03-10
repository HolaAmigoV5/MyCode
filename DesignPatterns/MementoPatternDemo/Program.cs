using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MementoPatternDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ContactPerson> persons = new List<ContactPerson>()
            {
                new ContactPerson(){ Name="Learning Hard", MobileNum="12345"},
                new ContactPerson(){ Name="Tony", MobileNum="234565" },
                new ContactPerson(){ Name="Jock", MobileNum="12321"}
            };

            MobileOwner mobileOwner = new MobileOwner(persons);
            mobileOwner.Show();

            //创建备忘录并保存备忘录对象
            Caretaker caretaker = new Caretaker();
            caretaker.ContactMementoDic.Add(DateTime.Now.ToString(), mobileOwner.CreateMemento());

            //更改发起人联系人列表
            Console.WriteLine("----------移除最后一个联系人----------");
            mobileOwner.ContactPersons.RemoveAt(2);
            mobileOwner.Show();

            //创建第二个备份
            Thread.Sleep(1000);
            caretaker.ContactMementoDic.Add(DateTime.Now.ToString(), mobileOwner.CreateMemento());

            //恢复到原始状态
            //Console.WriteLine("---------恢复联系人列表--------");
            //mobileOwner.RestoreMemento(caretaker.ContactM);
            //mobileOwner.Show();

            Console.WriteLine("-----------恢复联系人列表，请从以下列表选择恢复日期----------");
            var keyCollection = caretaker.ContactMementoDic.Keys;
            foreach (string k in keyCollection)
            {
                Console.WriteLine("Key={0}", k);
            }

            while (true)
            {
                Console.WriteLine("请输入数字，按窗口的关闭键退出：");

                int index;
                try
                {
                    index = Int32.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("输入的格式错误");
                    continue;
                }

                if (index < keyCollection.Count && caretaker.ContactMementoDic
                    .TryGetValue(keyCollection.ElementAt(index), out ContactMemento contactMemento))
                {
                    mobileOwner.RestoreMemento(contactMemento);
                    mobileOwner.Show();
                }
                else
                    Console.WriteLine("输入的索引大于集合的长度！");
            }
        }
    }

    /// <summary>
    /// 联系人
    /// </summary>
    public class ContactPerson
    {
        public string Name { get; set; }
        public string MobileNum { get; set; }
    }

    /// <summary>
    /// 发起人
    /// </summary>
    public class MobileOwner
    {
        //发起人需要保存的内部状态
        public List<ContactPerson> ContactPersons { get; set; }
        public MobileOwner(List<ContactPerson> persons)
        {
            ContactPersons = persons;
        }

        //创建备忘录，将当期要保存的联系人列表导入到备忘录中
        public ContactMemento CreateMemento()
        {
            //这里也应该传递深拷贝，new List方式传递的是浅拷贝
            //因为ContactPerson类中都是string类型，所以这里new list方式对ContactPerson对象执行了深拷贝
            //如果ContactPerson包含非string的引用类型就会有问题，所以这里也应该用序列化传递深拷贝
            return new ContactMemento(new List<ContactPerson>(this.ContactPersons));
        }

        //将备忘录中的数据备份导入到联系人列表中
        public void RestoreMemento(ContactMemento memento)
        {
            if (memento != null)
            {
                //下面这种方式是错误的，因为这样传递的是引用
                //则删除一次可以恢复，但恢复之后再删除的话就恢复不了
                //所以应该传递contactPersonBack的深拷贝，深拷贝可以使用序列化来完成。
                this.ContactPersons = memento.contactPersonBack;
            }
        }

        public void Show()
        {
            Console.WriteLine("联系人列表中有{0}个人，他们是：",ContactPersons.Count);
            foreach (ContactPerson p in ContactPersons)
            {
                Console.WriteLine("姓名：{0} 号码为：{1}", p.Name, p.MobileNum);
            }
        }
    }

    /// <summary>
    /// 备忘录
    /// </summary>
    public class ContactMemento
    {
        //保存发起人的内部状态
        public List<ContactPerson> contactPersonBack;
        public ContactMemento(List<ContactPerson> people)
        {
            contactPersonBack = people;
        }
    }

    /// <summary>
    /// 管理者
    /// </summary>
    public class Caretaker
    {
        //使用多个备忘录来存储多个备份点
        public Dictionary<string, ContactMemento> ContactMementoDic { get; set; }
        public Caretaker()
        {
            ContactMementoDic = new Dictionary<string, ContactMemento>();
        }
        //public ContactMemento ContactM { get; set; }
    }
}
