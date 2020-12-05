using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleThreading
{
    class Program
    {
        static void Main(string[] args)
        {
            //CancellationDemo.Go();
            //CancellationDemo.RegisterGo();
            //CancellationDemo.RegisterLinkGo();
            //TaskDemo.Go();
            //ParallelDemo.Go();
            //PLinqDemo.Go();
            //TimerDemo.Go();

            //ThreadHistory threadHistory = new ThreadHistory();
            //threadHistory.Go();

            //int x = 5;
            //int y = x++;
            //Console.WriteLine(y);

            //y = ++x;
            //Console.WriteLine(y);

            //int i = 0;
            //F(i--, i--, i--);
            //F(z: i--, x: i--);
            //F(++i, ++i, ++i);  //输出1,2,3
            //F(z: ++i, x: ++i); //输出x=5,y=-1,z=4

            //int[] num = new int[] { 1, 3, 5 };
            //ArrayList arr = new ArrayList();
            //int[] num2 = { 1, 3, 5 };
            //for (int i = 0; i < num.Length; i++)
            //{
            //    arr.Add(num[i]);
            //}
            //Console.WriteLine(arr[2]);

            //int count = 3;
            //while (count>1)
            //{
            //    Console.WriteLine(count);
            //    --count;
            //}
            //MyAnimals a = new MyAnimals();
            //a.BodyTemp = 56;
            //a.PrintBodyTemp();

            //float f = 123.56f;
            //object obj = f;
            //f = 789.123f;
            //Console.WriteLine($"f={f}");
            //Console.WriteLine($"obj={obj}");


            //string str;
            //str = Console.ReadLine();
            //bool A = str.Equals("A");
            //Console.WriteLine(A.ToString());
            //int B = str.Length;
            //Console.WriteLine(B.ToString());
            //B b = new B();

            int[] arrrays = new int[10] { 3, 5, 6, 3, 4, 6, 32, 232, 22, 22 };
            //int[] ls = arrrays.Distinct().ToArray();
            //int count = arrrays.Length - (arrrays.Length - ls.Length) * 2;
            //Console.WriteLine(count);

            //int[] arrInt = new int[] { 3, 5, 9, 8, 10, 5, 3, 34, 4, 6 };
            //int[] distinctArr = arrInt.Distinct().ToArray();
            //int noRepeatCount = arrInt.Length - (arrInt.Length - distinctArr.Length) * 2;
            //Console.WriteLine(noRepeatCount);
            //int i = Foo(9);
            //Console.WriteLine(i);

            //BubbleSort(arrrays);
            //Array.ForEach(arrrays, m => Console.WriteLine(m));
            //int sum = Sum(4);
            //Console.WriteLine(sum);

            //string str = " 23   dsfsd sdd     234esdf  ";
            //string result = RemoveSpace(str);

            //int result = GetSumPrime(1, 7);
            //Console.WriteLine(result);

            //var aa= ChangeValue(3, 5);

            string str = "1234567";
            string result = Reverse(str);
            Console.WriteLine(result);
            Console.ReadLine();
        }

        public static string Reverse(string str)
        {
            return new string(str.ToCharArray().Reverse().ToArray());
        }

        public static Tuple<int, int> ChangeValue(int a, int b)
        {
            a = a + b;
            b = a - b;
            a = a - b;
            Tuple<int, int> tuple = new Tuple<int, int>(a, b);
            return tuple;
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

        public static int Foo(int n)
        {
            if (n < 3)
                return 1;
            else
                return Foo(n - 1) + Foo(n - 2);
        }

        public static int[] BubbleSort(int[] arr)
        {
            int temp;
            for (int i = 0; i < arr.Length - 1; i++)
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

        public static int Sum(int n)
        {
            int sum = 0;
            for (int i = 0; i <= n; i++)
            {
                if (i % 2 == 0)
                    sum -= i;
                else
                    sum += i;
            }
            return sum;
        }

        public static string RemoveSpace(string str)
        {
            string result = Regex.Replace(str.Trim(), @"\s+", " ");
            return result;
        }

        public sealed class Singleton
        {
            private static volatile Singleton uniqueInstance;
            private static readonly object locker = new object();
            private Singleton() { }
            public static Singleton GetInstance()
            {
                if(uniqueInstance==null)
                {
                    lock (locker)
                    {
                        if (uniqueInstance == null)
                            uniqueInstance = new Singleton();
                    }
                }
                return uniqueInstance;
            }
        }

        class A
        {
            public A() { PrintFields(); }
            public virtual void PrintFields() { }
        }

        class B : A
        {
            int x = 1;
            int y;
            public B() : base() { 
                //y = -1; 
            }
            public override void PrintFields()
            {
                Console.WriteLine($"x={x}, y={y}");
            }
        }

        abstract class C:IDisposable
        {
            public abstract void Test();
            public void AA()
            {
                Console.WriteLine("sasdfs");
            }
            List<string> ls = new List<string>();
            public void BB(string  aa)
            {
                switch (aa)
                {
                    case "abc":
                        Console.WriteLine("121");
                        break;
                    case "d":
                        Console.WriteLine("23");
                        break;
                    default:
                        break;
                }
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        class D : C
        {
            public override void Test()
            {
                throw new NotImplementedException();
            }
        }

        static void F(int x, int y = -1, int z = -2)
        {
            Console.WriteLine($"x={x}, y={y}, z={z}");
        }

        class MyAnimals
        {
            private int bodyTemp = 98;
            public int BodyTemp { set { bodyTemp = value; } get { return bodyTemp; } }
            public void PrintBodyTemp() { Console.WriteLine($"温度为：{BodyTemp}"); }
        }
    }

    internal class InterviewTest
    {
        public readonly int a;
        public const int b = 123;
        public InterviewTest()
        {
            a = 123;
        }
        public void Go()
        {
            //tryException();
            Apple apple = new Apple();
            apple.FruitName = "苹果";
            Console.WriteLine();
            //Fruit fruit = new Fruit();
            ArrayList list = new ArrayList() { 1, 3 };
        }

        public void tryException()
        {
            try
            {
                checked
                {
                    int a = 2140000000;
                    int b = 2140000000;
                    int c = a + b;
                    Console.WriteLine($"a+b={c}");
                }

            }
            catch (OverflowException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public class Fruit
        {
            private double price;
            public string FruitName { get; set; }

            protected double Size { get; set; }
            internal int aa = 1;

        }

        public class Apple : Fruit
        {
            public string Origin { get; set; }
            private double SalePrice;

            public void SayHi()
            {
                Size = 1232;
            }

            StringBuilder sb = new StringBuilder();
        }

        public class Oranage
        {
            public const Color cc = Color.Red;
            public readonly int ab;
            public double GetPrice()
            {
                return 123;
                //short s1 = 1;
                ////s1 = s1 + 1;
                //s1 += 1;
            }
            public Oranage()
            {
                ArrayList list = new ArrayList();
                Hashtable hashtable = new Hashtable();


                const Color cb = Color.Green;
                if (cb == Color.Green)
                    Console.WriteLine("abc");
            }
        }

        public static class Perople
        {
            public static string Nation { get; set; }
            public static void ShowNation(string nation)
            {
                Console.WriteLine($"Show nation is {nation}");
                Oranage oranage = new Oranage();
                oranage.GetPrice();
            }
        }
        public enum Color
        {
            Red, White, Green
        }

        internal static class CancellationDemo
        {
            public static void ExeContexGo()
            {
                //将一些数据放到Main线程的逻辑调用上下文中
                CallContext.LogicalSetData("Name", "Jeffrey");
                //线程池线程能访问逻辑调用上下文数据
                ThreadPool.QueueUserWorkItem(state => Console.WriteLine("Name={0}", CallContext.LogicalGetData("Name")));

                //现在，阻止Main线程的执行上下文流动
                ExecutionContext.SuppressFlow();
                //线程池线程不能访问逻辑调用上下文数据
                ThreadPool.QueueUserWorkItem(state => Console.WriteLine("Name={0}", CallContext.LogicalGetData("Name")));
                //恢复Main线程的执行上下文流动
                ExecutionContext.RestoreFlow();
            }

            public static void Go()
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                ThreadPool.QueueUserWorkItem(o => Count(cts.Token, 100));
                Console.WriteLine("Press <Enter> to cancel the operation.");
                Console.ReadLine();
                cts.Cancel();

                Console.ReadLine();
            }

            private static void Count(CancellationToken token, int countTo)
            {
                for (int count = 0; count < countTo; count++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Count is cancelled");
                        break;
                    }
                    Console.WriteLine(count);
                    Thread.Sleep(200);
                }
                Console.WriteLine("Count is done");
            }

            public static void RegisterGo()
            {
                var cts = new CancellationTokenSource();

                //注册取消回调方法
                cts.Token.Register(() => Console.WriteLine("Canceled 1"));
                cts.Token.Register(() => Console.WriteLine("Canceled 2"));

                //出于测试目的，让我们取消它
                cts.Cancel();
            }

            public static void RegisterLinkGo()
            {
                //创建一个CancellationTokenSource
                var cts1 = new CancellationTokenSource();
                cts1.Token.Register(() => Console.WriteLine("cts1 canceled"));

                //创建领一个
                var cts2 = new CancellationTokenSource();
                cts2.Token.Register(() => Console.WriteLine("cts2 canceled"));

                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token);
                linkedCts.Token.Register(() => Console.WriteLine("linkedCts canceled"));
                cts2.Cancel();

                Console.WriteLine("cts1 canceled={0}, cts2 canceled={1}, linkedCts={2}",
                    cts1.IsCancellationRequested, cts2.IsCancellationRequested, linkedCts.IsCancellationRequested);

            }
        }

        internal static class TaskDemo
        {
            public static void Go()
            {
                //UsingTaskInsteadOfQueueUserWorkItem();
                //WaitForResult();
                //Cancel();
                //MultipleContinueWith();
                //ParentChild();
                TaskFactory();
            }

            private static void UsingTaskInsteadOfQueueUserWorkItem()
            {
                ThreadPool.QueueUserWorkItem(state => Console.WriteLine("Hello {state}", state));
                new Task(() => Sum(1000)).Start();
                Task.Run(() => Sum(1000));
            }

            private static void WaitForResult()
            {
                Task<int> t = new Task<int>(n => Sum((int)n), 10000);
                t.Start();
                t.Wait();
                Console.WriteLine("The sum is: " + t.Result);
            }

            private static void Cancel()
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                Task<int> t = Task.Run(() => Sum(cts.Token, 1000), cts.Token);
                //t.Wait();
                cts.Cancel();
                try
                {
                    Console.WriteLine("The sum is: " + t.Result);
                }
                catch (AggregateException x)
                {
                    x.Handle(e => e is OperationCanceledException);
                    Console.WriteLine("Sum was canceled");
                }
            }

            private static void ContinueWith()
            {
                Task<int> t = Task.Run(() => Sum(10000));
                Task cwt = t.ContinueWith(task => Console.WriteLine("The sum is: " + task.Result));
                cwt.Wait();
            }

            private static void MultipleContinueWith()
            {
                Task<int> t = Task.Run(() => Sum(10000));
                t.ContinueWith(task => Console.WriteLine("The Sum is: " + task.Result),
                    TaskContinuationOptions.OnlyOnRanToCompletion);
                t.ContinueWith(task => Console.WriteLine("Sum threw: " + task.Exception),
                    TaskContinuationOptions.OnlyOnFaulted);
                t.ContinueWith(task => Console.WriteLine("sum was canceled"),
                    TaskContinuationOptions.OnlyOnCanceled);

                try
                {
                    t.Wait();
                }
                catch (AggregateException)
                {
                    throw;
                }
            }

            private static void ParentChild()
            {
                Task<int[]> parent = new Task<int[]>(() =>
                {
                    var results = new int[3];

                    new Task(() => results[0] = Sum(1000), TaskCreationOptions.AttachedToParent).Start();
                    new Task(() => results[1] = Sum(2000), TaskCreationOptions.AttachedToParent).Start();
                    new Task(() => results[2] = Sum(3000), TaskCreationOptions.AttachedToParent).Start();
                    return results;
                });

                var cwt = parent.ContinueWith(parentTask => Array.ForEach(parentTask.Result, Console.WriteLine));
                parent.Start();
                cwt.Wait();
            }

            private static void TaskFactory()
            {
                Task parent = new Task(() =>
                {
                    var cts = new CancellationTokenSource();
                    var tf = new TaskFactory<int>(cts.Token, TaskCreationOptions.AttachedToParent,
                        TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

                //这个任务创建并启动3个子任务
                var childTasks = new[] {
                    tf.StartNew(()=>Sum(cts.Token,1000)),
                    tf.StartNew(()=>Sum(cts.Token,2000)),
                    tf.StartNew(()=>Sum(cts.Token,int.MaxValue))  //Too big, throws OverflowException
                };

                //任何子任务抛出异常，就取消其余子任务
                for (int task = 0; task < childTasks.Length; task++)
                    {
                        childTasks[task].ContinueWith(t => cts.Cancel(), TaskContinuationOptions.OnlyOnFaulted);
                    }

                //所有子任务完成后，从未出错的任务获取返回的最大值，然后将最大值传给另一个任务来显示最大结果。
                tf.ContinueWhenAll(childTasks, completedTasks => completedTasks.
                     Where(t => t.Status == TaskStatus.RanToCompletion).Max(t => t.Result), CancellationToken.None)
                    .ContinueWith(t => Console.WriteLine("The maximum is: " + t.Result), TaskContinuationOptions.ExecuteSynchronously)
                    .Wait();
                });

                //捕获并记录异常
                parent.ContinueWith(p =>
                {
                    StringBuilder sb = new StringBuilder("The following exeception(s) occurred:" + Environment.NewLine);
                    foreach (var e in p.Exception.Flatten().InnerExceptions)
                    {
                        sb.AppendLine("  " + e.GetType().ToString());
                    }
                    Console.WriteLine(sb.ToString());
                }, TaskContinuationOptions.OnlyOnFaulted);

                //启动父任务，使它能启动子任务。
                parent.Start();
                try
                {
                    parent.Wait();
                }
                catch (AggregateException e)
                {
                    Console.WriteLine(e.InnerException.Message);
                }
            }

            private static int Sum(CancellationToken ct, int n)
            {
                int sum = 0;
                for (; n > 0; n--)
                {
                    ct.ThrowIfCancellationRequested();  //如果取消抛出异常
                    checked { sum += n; }  //如果n太大，会抛出System.OverflowException
                }
                return sum;
            }

            private static int Sum(int n)
            {
                int sum = 0;
                for (; n > 0; n--)
                    checked { sum += n; }
                return sum;
            }
        }

        internal static class ParallelDemo
        {
            public static void Go()
            {
                //SimpleUsage();

                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Console.WriteLine("The total bytes of all files in {0} is {1:N0}.",
                    path, DirectoryBytes(path, "*.*", SearchOption.TopDirectoryOnly));
            }

            private static void SimpleUsage()
            {
                for (int i = 0; i < 1000; i++)
                {
                    DoWork(i);
                }

                //The thread pool's threads process the work in parallel
                Parallel.For(0, 1000, i => Console.WriteLine("DoWork {0}", i));

                var collection = new int[4] { 1, 2, 3, 4 };
                foreach (var item in collection)
                {
                    DoWork(item);
                }

                //The thread pool's threads process the work in parallel
                Parallel.ForEach(collection, item => DoWork(item));

                Parallel.Invoke(
                    () => Console.WriteLine("Method1"),
                    () => Console.WriteLine("Method2"),
                    () => Console.WriteLine("Method3"));
            }

            private static void DoWork(int i) { Console.WriteLine("DoWork {0}", i); }

            private static long DirectoryBytes(string path, string searchPattern, SearchOption searchOption)
            {
                var files = Directory.EnumerateFiles(path, searchPattern, searchOption);
                long masterTotal = 0;
                ParallelLoopResult result = Parallel.ForEach<string, long>(files,
                    () =>
                    {
                    //每个任务开始之前调用一次，总计数初始化为0
                    return 0; //将taskLocalTotal初始化为0 
                },
                (file, parallelLoopState, index, taskLocalTotal) =>
                {
                //body：每个工作项调用一次
                //获得这个文件的大小，把它添加到这个任务的累加值上
                long fileLength = 0;
                    FileStream fs = null;
                    try
                    {
                        fs = File.OpenRead(file);
                        fileLength = fs.Length;
                    }
                    catch (IOException) { /*忽略拒绝访问的文件 */}
                    finally { if (fs != null) fs.Dispose(); }

                    return taskLocalTotal += fileLength;
                },
                taskLocalTotal =>
                {
                //每个任务完成时调用一次
                //将这个任务的总计值(taskLocalTotal)加到总的总计值上(masterTotal)
                Interlocked.Add(ref masterTotal, taskLocalTotal);
                });
                return masterTotal;
            }
        }

        internal static class PLinqDemo
        {
            public static void Go()
            {
                ObsoleteMethods(typeof(object).Assembly);
            }

            //查找一个程序集中定义的所有过时(obsolete)方法
            private static void ObsoleteMethods(Assembly assembly)
            {
                var query = from type in assembly.GetExportedTypes().AsParallel()
                            from method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                            let obsoleteAttrType = typeof(ObsoleteAttribute)
                            where Attribute.IsDefined(method, obsoleteAttrType)
                            orderby type.FullName
                            let obsoleteAttrObj = (ObsoleteAttribute)Attribute.GetCustomAttribute(method, obsoleteAttrType)
                            select string.Format("Type={0}\nMethod={1}\nMessage={2}\n", type.FullName, method.ToString(), obsoleteAttrObj.Message);

                //foreach (var result in query)
                //{
                //    Console.WriteLine(result);
                //}

                query.ForAll(result => Console.WriteLine(result));
                //query.ForAll(Console.WriteLine);
            }
        }

        internal static class TimerDemo
        {
            private static Timer s_timer;
            public static void Go()
            {
                Console.WriteLine("Checking status every 2 seconds");
                s_timer = new Timer(Status, null, Timeout.Infinite, Timeout.Infinite);
                s_timer.Change(0, Timeout.Infinite);
                Console.ReadLine();
            }

            private static void Status(Object state)
            {
                Console.WriteLine("In status at {0}", DateTime.Now);
                Thread.Sleep(1000);
                s_timer.Change(2000, Timeout.Infinite);
            }
        }

        internal class ThreadHistory
        {
            Stopwatch sw = new Stopwatch();
            public void Go()
            {
                //CallMethod(ThreadTest);
                //CallMethod(ThreadPoolTest);
                //CallMethod(TaskTest);
                sw.Start();
                AsyncTest().Wait();
                Console.WriteLine($"the time of call asyncTest is {sw.ElapsedMilliseconds} ms");
            }


            public void ThreadTest()
            {
                sw.Start();
                Console.WriteLine($"开始调用ThreadTest, Thread id is {Thread.CurrentThread.ManagedThreadId}.");
                //Thread thread = new Thread(ShowMsg);
                //thread.Start("call ShowMsg");
                new Thread(ShowMsg).Start("this is ThreadTest");
            }

            public void ThreadPoolTest()
            {
                sw.Start();
                Console.WriteLine($"开始调用ThreadPoolTest, Thread id is {Thread.CurrentThread.ManagedThreadId}.");
                ThreadPool.QueueUserWorkItem(ShowMsg, "This is ThreadPoolTest");
            }

            public void TaskTest()
            {
                sw.Start();
                Console.WriteLine($"开始调用TaskTest, Thread id is {Thread.CurrentThread.ManagedThreadId}.");
                Task task = new Task(() => ShowMsg("this is TaskTest"));
                task.Start();
            }

            private async Task AsyncTest()
            {
                Console.WriteLine($"Before calling GetName, Thread Id:{Thread.CurrentThread.ManagedThreadId} \r\n");
                var name = await GetName();
                Console.WriteLine("End calling GetName.\r\n");
                Console.WriteLine($"Get result from GetName:{name}");
            }

            private async Task<string> GetName()
            {
                Console.WriteLine($"Before calling Task.Run, current thread id is {Thread.CurrentThread.ManagedThreadId} \r\n");
                return await Task.Run(() =>
                {
                    Thread.Sleep(5000);
                    Console.WriteLine($"GetName Thread Id:{Thread.CurrentThread.ManagedThreadId}");
                    return "wby";
                });
            }

            public void ShowMsg(object msg)
            {
                Thread.Sleep(5000);
                Console.WriteLine($"{msg}, Thread id is {Thread.CurrentThread.ManagedThreadId}");
            }

            public void CallMethod(Action action)
            {
                action();
                Console.WriteLine($"The time of call ThreadTest is {sw.ElapsedMilliseconds} ms");
                sw.Reset();
            }

        }

        internal class AsyncThreadDemo
        {
            public void Go()
            {
                string result = null;
                var doSomethingDelegate = new Func<string>(DoSomething);
                var asyncResult = doSomethingDelegate.BeginInvoke(new AsyncCallback(aresult =>
                {
                    result = doSomethingDelegate.EndInvoke(aresult);
                }), null);

                asyncResult.AsyncWaitHandle.WaitOne();
                Console.WriteLine(result);
            }

            private string DoSomething()
            {
                Thread.Sleep(2000);
                return "Finished";
            }
        }

        public class CWAttribute : Attribute
        {
            public CWAttribute(string methodName)
            {
                Console.WriteLine($"我是{methodName}");
            }
        }

    }
}
