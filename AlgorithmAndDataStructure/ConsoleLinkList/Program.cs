using System;

namespace ConsoleLinkList
{
    class Program
    {
        static void Main(string[] args)
        {
            LinkList<string> link = new LinkList<string>();
            link.Append("A");
            link.Append("B");
            link.Append("C");
            link.Append("D");
            link.Append("E");
            link.Insert("Head", 1);
            //link.Insert("F", 6);
            Console.WriteLine("单链表内容：");
            link.Display();

            link.Delete(5);
            Console.WriteLine("已经完成删除单链表中第5行记录数");
            link.Display();
            Console.WriteLine("查询单链表中第1：{0}，第3：{1}",link.GetNodeValue(1),link.GetNodeValue(3));


            //Console.WriteLine("面试题--> 单链表反转");
            //link.Reverse();

            Console.WriteLine("面试题-->获取单链表中间值");
            link.GetMiddleValue();
            Console.ReadLine();

        }
    }

    public class LinkList<T>
    {
        public Node<T> Head { get; set; }
        public LinkList()
        {
            Head = null;
        }

        public void Append(T item)
        {
            Node<T> foot = new Node<T>(item);
            Node<T> A = new Node<T>();
            if (Head == null)
            {
                Head = foot;
                return;
            }
            A = Head;
            while (A.Next!=null)
            {
                A = A.Next;
            }
            A.Next = foot;
        }

        public void Delete(int i)
        {
            Node<T> A = new Node<T>();
            if (i == 0)
            {
                A = Head;
                Head = Head.Next;
                return;
            }
            Node<T> B = new Node<T>();
            B = Head;
            int j = 1;
            while (B.Next!=null && j<i)
            {
                A = B;
                B = B.Next;
                j++;
            }
            if (j == i)
            {
                A.Next = B.Next;
            }
        }

        public void Insert(T item, int n)
        {
            if(IsEmpty()|| n < 1 || n > GetLength())
            {
                Console.WriteLine("单链表为空或节点位置有误！");
                return;
            }

            if (n == 1)
            {
                Node<T> H = new Node<T>(item);
                H.Next = Head;
                Head = H;
                return;
            }

            Node<T> A = new Node<T>();
            Node<T> B = new Node<T>();
            B = Head;
            int j = 1;
            while (B.Next!=null && j<n)
            {
                A = B;
                B = B.Next;
                j++;
            }
            if (j == n)
            {
                Node<T> C = new Node<T>(item);
                A.Next = C;
                C.Next = B;
            }
        }

        public T GetNodeValue(int i)
        {
            if (IsEmpty() || i < 1 || i > GetLength())
            {
                Console.WriteLine("单链表为空或节点位置有误!");
                return default;
            }

            Node<T> A = new Node<T>();
            A = Head;
            int j = 1;
            while (A.Next!=null && j<i)
            {
                A = A.Next;
                j++;
            }
            return A.Data;
        }

        public void Reverse()
        {
            if (GetLength() == 1 || Head == null)
                return;
            Node<T> NewNode = null;
            Node<T> CurrentNode = Head;
            Node<T> TempNode = new Node<T>();

            while (CurrentNode!=null)
            {
                TempNode = CurrentNode.Next;
                CurrentNode.Next = NewNode;
                NewNode = CurrentNode;
                CurrentNode = TempNode;
            }
            Head = NewNode;
            Display();
        }

        /// <summary>
        /// 获取单链表中间值
        /// 思路：使用两个指针，第一个每次走一步，第二个每次走两步
        /// </summary>
        public void GetMiddleValue()
        {
            Node<T> A = Head;
            Node<T> B = Head;
            while (B!=null&&B.Next!=null)
            {
                A = A.Next;
                B = B.Next.Next;
            }
            if (B != null)
                Console.WriteLine("奇数：中间值为：{0}", A.Data);
            else
                Console.WriteLine("偶数：中间值为：{0}和{1}", A.Data, A.Next.Data);
        }

        public int GetLength()
        {
            Node<T> p = Head;
            int length = 0;
            while (p!=null)
            {
                p = p.Next;
                length++;
            }
            return length;
        }
        public bool IsEmpty()
        {
            if (Head == null)
                return true;
            else
                return false;
        }
        public void Clear()
        {
            Head = null;
        }

        public void Display()
        {
            Node<T> A = new Node<T>();
            A = Head;
            while (A!=null)
            {
                Console.WriteLine(A.Data);
                A = A.Next;
            }
        }
    }

    public class Node<T>
    {
        public T Data { set; get; }
        public Node<T> Next { get; set; }

        public Node()
        {
            this.Data = default(T);
            this.Next = null;
        }

        public Node(T item)
        {
            this.Data = item;
            this.Next = null;
        }
    }
}
