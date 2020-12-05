using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Configuration;

namespace ConsoleThreading
{
    class Program
    {
        static void Main(string[] args)
        {
            //ThreadingStart();
            //ThreadStartWithParameter();
            //AyncThreadCall();
            //MonitorCall();
            //DeadlockDemo();
            //MutexCallTest();

            Console.ReadLine();
        }

        public static void ThreadingStart()
        {
            ThreadStart ts = new ThreadStart(() =>
            {
                SharedResource resource = new SharedResource();
                resource.Add("Item3");
            });
            Thread worker = new Thread(ts);
            worker.Start();
            Console.WriteLine("Main Thread ends.");
            Console.ReadKey();
        }

        public static void ThreadStartWithParameter()
        {
            ParameterizedThreadStart ts = new ParameterizedThreadStart(obj =>
            {
                SharedResource resource = new SharedResource();
                string[] itemArray = (string[])obj;
                foreach (string item in itemArray)
                {
                    resource.Add(item);
                }
            });
            Thread thread = new Thread(ts);
            string[] para = { "item3", "item4" };
            thread.Start(para);
            Console.WriteLine("Main Thread ends.");
            Console.ReadKey();
        }

        static int i = 0;
        public static void AyncThreadCall()
        {
            new Thread(ThreadEntity).Start();
            ThreadEntity();
        }

        public static void ThreadEntity()
        {
            Console.WriteLine("i={0}", i);
            i++;
        }

        public static void MonitorCall()
        {
            Resource resource = new Resource();
            object lockobj = new object();
            Thread.CurrentThread.Name = "Main";
            Thread worker = new Thread(ThreadEntiry)
            {
                Name = "Worker"
            };
            worker.Start();
            ThreadEntiry();

            void ThreadEntiry()
            {
                //try
                //{
                //    Monitor.Enter(lockobj);
                //    resource.Record();
                //}
                //catch (Exception e)
                //{
                //    throw e;
                //}
                //finally
                //{
                //    Monitor.Exit(lockobj);
                //} 
                
                //等价于以上代码
                lock (resource)
                {
                    try
                    {
                        resource.Record();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                   
                }
            }
        }


        Resource2 mainRes = new Resource2() { Data = "mainRes" };
        Resource2 workerRes = new Resource2() { Data = "workerRes" };
        public static void DeadlockDemo()
        {
            Thread.CurrentThread.Name = "Main";
            Program p = new Program();
            Thread worker = new Thread(p.T2)
            {
                Name = "Worker"
            };
            worker.Start();
            T1(p);
        }

        static void T1(Program p)
        {
            lock (p.mainRes)
            {
                Thread.Sleep(10);
                int i = 0;
                while (i<3)
                {
                    if (Monitor.TryEnter(p.workerRes))
                    {
                        Console.WriteLine(p.workerRes.Data);
                        Monitor.Exit(p.workerRes);
                        break;
                    }
                    else
                        Thread.Sleep(1000);
                    i++;
                }
                if (i == 3)
                    Console.WriteLine("{0}：Tried 3 times, Deadlock", Thread.CurrentThread.Name);
                //lock (p.workerRes) { Console.WriteLine(p.workerRes.Data); }
            }
        }

        void T2()
        {
            lock (workerRes)
            {
                Thread.Sleep(10);
                int i = 0;
                while (i<3)
                {
                    if (Monitor.TryEnter(mainRes))
                    {
                        Console.WriteLine(mainRes.Data);
                        Monitor.Exit(mainRes);
                        break;
                    }
                    else
                        Thread.Sleep(1000);
                    i++;
                }
                if (i == 3)
                    Console.WriteLine("{0}：Tried 2 times, Deadlock", Thread.CurrentThread.Name);
                //lock (mainRes) { Console.WriteLine(mainRes.Data); }
            }
        }

        public static void MutexCallTest()
        {
            MutexCallDemo md = new MutexCallDemo();
            md.MutexCall();
        }
    }

    public class MutexCallDemo
    {
        string called = "";
        Mutex mtx = new Mutex();

        public void MutexCall()
        {
            Thread.CurrentThread.Name = "Main  ";
            Thread worker = new Thread(ThreadEntry);
            worker.Name = "Worker";
            worker.Start();
            ThreadEntry();
        }

        private void ThreadEntry()
        {
            this.mtx.WaitOne();
            string name = Thread.CurrentThread.Name;
            called += string.Format("{0}：[{1}]", name, DateTime.Now.Millisecond);
            Console.WriteLine(called);
            this.mtx.ReleaseMutex();
        }
    }

    public class SharedResource
    {
        public List<string> list = new List<string> { "Item0", "Item1", "Item2" };
        public void Add(string item)
        {
            Console.WriteLine("Add " + item);
            list.Add(item);
        }
    }

    public class Resource
    {
        public string Called;
        public void Record()
        {
            this.Called += string.Format("{0} [{1}]", Thread.CurrentThread.Name, DateTime.Now.Millisecond);
            Console.WriteLine(Called);
        }
    }

    //线程安全的SafeResource
    public class SafeResource
    {
        public string Called;
        public void Record()
        {
            lock (this)
            {
                this.Called += string.Format("{0} [{1}]", Thread.CurrentThread.Name,
                    DateTime.Now.Millisecond);
                Console.WriteLine(Called);
            }
        }

        //等价于以上代码
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public void Record()
        //{
        //    this.Called += string.Format("{0} [{1}]", Thread.CurrentThread.Name,
        //            DateTime.Now.Millisecond);
        //    Console.WriteLine(Called);
        //}
    }

    public class Resource2
    {
        public string Data;
    }
}
