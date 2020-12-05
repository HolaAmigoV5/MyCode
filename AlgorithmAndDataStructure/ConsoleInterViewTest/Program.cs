using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleInterViewTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //object a = "123";
            //object b = "123";
            //Console.WriteLine(Equals(a, b));
            //Console.WriteLine(ReferenceEquals(a,b));

            //string sa = "123";
            //Console.WriteLine(Equals(a, sa));
            //Console.WriteLine(ReferenceEquals(a, sa));


            //int i = 5;
            //object obj = i;  //1次装箱
            //IFormattable ftt = i;  //1次装箱
            //Console.WriteLine(System.Object.ReferenceEquals(i, obj));  //1次装箱
            //Console.WriteLine(System.Object.ReferenceEquals(i, ftt));  //1次装箱
            //Console.WriteLine(System.Object.ReferenceEquals(ftt, obj));  //不装箱和拆箱
            //                                                             //2次装箱，1次拆箱。i装箱，obj先装箱再拆箱
            //Console.WriteLine(System.Object.ReferenceEquals(i, (int)obj));
            //Console.WriteLine(System.Object.ReferenceEquals(i, (int)ftt)); //2次装箱1次拆箱

            //string st1 = "123" + "abc";
            //string st2 = "123abc";
            //Console.WriteLine(st1 == st2);  //输出结果为true
            //Console.WriteLine(ReferenceEquals(st1, st2)); //输出结果为true

            //int[] arrInt = { 3, 4, 5, 2, 6, 2, 11, 21, 31, 78, 32 };

            //int i= arrInt.Count();
            //int j = arrInt.Length;
            //int count = arrInt.Length - (arrInt.Length - arrInt.Distinct().Count()) * 2;
            //Console.WriteLine(count);

            //for (int i = 0; i < 1000; i++)
            //{
            //    var arr = GenerateIntArr();
            //    var intArr = BubbleSort(arr);
            //    ShowText(intArr);
            //}

            //int[] newArr = BubbleSort(lists.Distinct().ToArray());
            //int[] newArr = BubbleSort(arrInt);
            //newArr.ToList().ForEach(a => Console.WriteLine(a));

            //Console.WriteLine(RemoveSpace("  ds阿斯  顿发大   水a fs dfs   dfk   "));
            //int sum = GetSumPrime(0, 5);
            //Console.WriteLine(sum);
            //Console.WriteLine(ReverseStr("123456"));

            //Dog dog = new Dog();
            //dog.Shout();
            //Animal animal = new Dog();
            //animal.Shout();

            //Console.WriteLine("X={0},Y={1}", A.X, B.Y);

            //Console.WriteLine(Foo(0));

            //Class1 o1 = new Class1();
            //Console.WriteLine(Class1.count);
            //Class1 o2 = new Class1();
            //Console.WriteLine(Class1.count);


            //int i = string.Compare("AbC", "abc",false);
            //Console.WriteLine(i);
            //Solution s = new Solution();
            //int[] intlist = { 1,1,2 };
            //var list = intlist.Distinct().ToArray();
            //int i = s.RemoveDuplicates(intlist);

            int[] nums1 = { 1, 2, 3, 4 };
            int[] nums2 = { 2, 5, 6 };
            int[] tem = new int[nums1.Length + nums2.Length];
            Array.Copy(nums1, tem, nums1.Length);
            Array.Copy(nums2, 0, tem, nums1.Length, nums2.Length);
            Array.Sort(tem);

            //List<int> templist = new List<int>(nums1.Length + nums2.Length);
            //templist.AddRange(nums1);
            //templist.AddRange(nums2);
            //templist.Sort();

            //templist.ForEach(m => Console.WriteLine(m));

            Array.ForEach(tem, m => { Console.Write(m); });
            Console.ReadLine();
           
        }


        public class Solution
        {
            public int RemoveDuplicates(int[] nums)
            {
                if (nums == null || nums.Length == 0) return 0;
                int p = 0, q = 1;
                while (q < nums.Length)
                {
                    if (nums[p] != nums[q])
                    {
                        if (q - p > 1)
                            nums[p + 1] = nums[q];
                        p++;
                    }
                    q++;
                }
                return p + 1;
            }

        }

        class Class1
        {
            internal static int count = 0;
            static Class1()
            {
                count++;
            }
            public Class1()
            {
                count++;
            }
        }


        public static bool isPower(int n)
        {
            if (n < 1)
                return false;
            return (n & (n - 1)) == 0;

            //int i = 1;
            //while (i<=n)
            //{
            //    if (i == n)
            //        return true;
            //    i = i << 1;
            //}
            //return false;
        }

        public static int? CountPower(int n)
        {
            if (isPower(n))
            {
                //if (n == 1)
                //    return 0;
                //else
                //    return CountPower(n >> 1) + 1;

                int x = 0;
                while (n > 1)
                {
                    n = n >> 1;
                    x++;
                }
                return x;
            }
            return null;
        }

        public static int GetSumPrime(int p1, int p2)
        {
            int sum = 0;
            for (int i = p1; i < p2 + 1; i++)
            {
                if (IsPrime(i))
                    sum += i;
            }
            return sum;
        }

        private static bool IsPrime(int number)
        {
            if (number < 2)
                return false;

            for (int i = 2; i < number; i++)
            {
                if (number % i == 0)
                    return false;
            }
            return true;
        }

        private static string ReverseStr(string str)
        {
            //return new string(str.Reverse().ToArray());

            string result = string.Empty;
            for (int i = str.Length - 1; i > -1; i--)
            {
                result += str[i];
            }
            return result;
        }

        private static string RemoveSpace(string str)
        {
            string result = Regex.Replace(str.Trim(), @"\s+", " ");
            return result;
        }

        private static void ShowText(int[] intArr)
        {
            intArr.ToList().ForEach(a => Console.Write(a + "  "));
            Console.WriteLine("\r");
            Console.WriteLine("--------------------------------------------");
        }

        public static int[] GenerateIntArr()
        {
            Random r = new Random();
            List<int> lists = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                lists.Add(r.Next(1, 500));
            }
            return lists.Distinct().ToArray();
        }

        public static int[] BubbleSort(int[] arr)
        {
            int temp;
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (arr[i] < arr[j])
                    {
                        temp = arr[i];
                        arr[i] = arr[j];
                        arr[j] = temp;
                    }
                }
            }
           
            return arr;
        }

        public static int Foo(int n)
        {
            if (n < 3)
                return 1;
            else
                return Foo(n - 1) + Foo(n - 2);
        }
    }

    public abstract class Animal
    {
        public Animal()
        {
            Console.WriteLine("New Animal");
        }

        public virtual void Shout()
        {
            Console.WriteLine("Animal Shout");
        }
    }

    public class Dog : Animal
    {
        public Dog()
        {
            Console.WriteLine("New Dog");
        }
        public override void Shout()
        {
            Console.WriteLine("Dog Shout");
        }
    }

    class A
    {
        public static int X = 2;
        static A()
        {
            X = B.Y + 1;
        }
    }
    class B
    {
        public static int Y = A.X + 1;
        static B() { }
        //static void Main()
        //{
        //    Console.WriteLine("X={0},Y={1}", A.X, B.Y);
        //    Console.Read();
        //}
    }

}
