using System;
using System.Collections;
using System.Collections.Generic;

namespace ConsoleMyList
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductCollection col = Product.GetSampleCollection();
            PrintProductTitle();
            //for (int i = 0; i < col.Count; i++)
            //{
            //    string line = col[i].ToString();
            //    Console.WriteLine(line);
            //}

            //foreach (string key in col.Keys)
            //{
            //    string line = col[key].ToString();
            //    Console.WriteLine(line);
            //}

            IEnumerator<Product> e = col.GetEnumerator();
            while (e.MoveNext())
            {
                string line = e.Current.ToString();
                Console.WriteLine(line);
            }

            //foreach (Product item in col)
            //{
            //    string line = item.ToString();
            //    Console.WriteLine(line);
            //}
            Console.ReadLine();
        }

        private static void PrintProductTitle()
        {
            string[] titleStr = { "<Id>", "<Category>", "<Code>", "<Name>", "<Price>", "<ProduceDate>" };
            Console.WriteLine(string.Join("".PadLeft(5), titleStr));
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public DateTime ProduceDate { get; set; }

        public override string ToString()
        {
            return string.Format($"{this.Id,2} {this.Category,15} {this.Code,7} " +
                $"{this.Name,17} {this.Price,7} { this.ProduceDate,13:yyyy-M-d}");
        }

        public static ProductCollection GetSampleCollection()
        {
            ProductCollection collection = new ProductCollection(
                new Product { Id = 1, Code = "1001", Category = "Red Wine", Name = "Torres Coronas", Price = 123, ProduceDate = DateTime.Now },
                new Product { Id = 2, Code = "2001", Category = "Beer", Name = "Torres Coronas", Price = 234, ProduceDate = DateTime.Now },
                new Product { Id = 3, Code = "1011", Category = "White Spirit", Name = "Torres Coronas", Price = 22, ProduceDate = DateTime.Now },
                new Product { Id = 4, Code = "1221", Category = "White Spirit", Name = "Torres Coronas", Price = 123, ProduceDate = DateTime.Now },
                new Product { Id = 5, Code = "2011", Category = "Red Wine", Name = "Torres Coronas", Price = 32, ProduceDate = DateTime.Now },
                new Product { Id = 6, Code = "3001", Category = "Red Wine", Name = "Torres Coronas", Price = 56, ProduceDate = DateTime.Now },
                new Product { Id = 7, Code = "1301", Category = "Red Wine", Name = "Torres Coronas", Price = 675, ProduceDate = DateTime.Now });
            return collection;
        }
    }

    public class ProductCollection:IEnumerable<Product>
    {
        private Hashtable table;
        public ProductCollection()
        {
            table = new Hashtable();
        }
        public ProductCollection(params Product[] array)
        {
            table = new Hashtable();
            foreach (Product item in array)
            {
                this.Add(item);
            }
        }
        public ICollection Keys { get { return table.Keys; } }
        public int Count { get { return table.Keys.Count; } }
        public Product this[int index]
        {
            get
            {
                string key = getKey(index);
                return table[key] as Product;
            }
            set
            {
                string key = getKey(index);
                table[key] = value;
            }
        }

        public Product this[string key]
        {
            get
            {
                return table[key] as Product;
            }
            set
            {
                table[key] = value;
            }
        }

        private string getKey(string key)
        {
            foreach (string k in table.Keys)
            {
                if (key == k)
                    return key;
            }
            throw new Exception("不存在此键值");
        }

        private string getKey(int index)
        {
            if (index < 0 || index > table.Keys.Count)
                throw new Exception("索引超出了范围");
            string selected = "";
            int i = 0;
            foreach (string key in table.Keys)
            {
                if (i == index)
                {
                    selected = key;
                    break;
                }
                i++;
            }
            return selected;
        }

        public void Add(Product item)
        {
            foreach (string key in table.Keys)
            {
                if (key == item.Code)
                    throw new Exception("产品代码不能重复");
            }
            table.Add(item.Code, item);
        }
        public void Insert(int index, Product item)
        {
            table.Add(getKey(index), item);
        }
        public bool Remove(Product item)
        {
            try
            {
                table.Remove(item.Code);
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }
        public bool RemoveAt(int index)
        {
            try
            {
                table.Remove(getKey(index));
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }
        public void Clear() { table.Clear(); }

        public IEnumerator<Product> GetEnumerator()
        {
            return new ProductEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ProductEnumerator(this);
        }

        public class ProductEnumerator : IEnumerator<Product>
        {
            private ProductCollection collection;
            private int index;
            public ProductEnumerator(ProductCollection col)
            {
                this.collection = col;
                index = -1;
            }
            public Product Current { get { return collection[index]; } }

            object IEnumerator.Current { get { return collection[index]; } }

            public void Dispose()
            {
                collection?.Clear();
            }

            public bool MoveNext()
            {
                index++;
                if (index >= collection.Count)
                    return false;
                else
                    return true;
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }
}
