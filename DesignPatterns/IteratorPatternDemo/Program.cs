using System;

namespace IteratorPatternDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ITerator terator;
            IListCollection list = new ConcreteList();
            terator = list.GetIterator();

            while (terator.MoveNext())
            {
                int i = (int)terator.GetCurrent();
                Console.WriteLine(i.ToString());
                terator.Next();
            }

            Console.Read();
        }
    }

    public interface IListCollection
    {
        ITerator GetIterator();
    }

    public interface ITerator
    {
        bool MoveNext();
        object GetCurrent();
        void Next();
        void Reset();
    }


    /// <summary>
    /// 具体聚合类
    /// </summary>
    public class ConcreteList : IListCollection
    {
        int[] collection;
        public ConcreteList()
        {
            collection = new int[] { 2, 4, 6, 8 };
        }
        public ITerator GetIterator()
        {
            return new ConcreteIterator(this);
        }

        public int Length
        {
            get { return collection.Length; }
        }

        public int GetElement(int index)
        {
            return collection[index];
        }
    }


    /// <summary>
    /// 具体迭代器类
    /// </summary>
    public class ConcreteIterator : ITerator
    {
        private ConcreteList _list;
        private int _index;
        public ConcreteIterator(ConcreteList list)
        {
            _list = list;
            _index = 0;
        }
        public object GetCurrent()
        {
            return _list.GetElement(_index);
        }

        public bool MoveNext()
        {
            if (_index < _list.Length)
                return true;
            return false;
        }

        public void Next()
        {
            if (_index < _list.Length)
                _index++;
        }

        public void Reset()
        {
            _index = 0;
        }
    }
}
