using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleBaseThreadSynchronize
{
    public class Program
    {
        static void Main(string[] args)
        {
            //AsyncCoordinatorDemo.Go();
            LockComparison.Go();
        }
    }

    internal static class AsyncCoordinatorDemo
    {
        public static void Go()
        {
            const int timeout = 5000;
            MultiWebRequests act = new MultiWebRequests(timeout);
            Console.WriteLine("All operations initiated (Timeout={0}).Hit <Enter> to cancel.)",
                (timeout == Timeout.Infinite) ? "Infinite" : (timeout.ToString() + "ms"));

            Console.ReadLine();
            act.Cancel();

            Console.WriteLine();
            Console.WriteLine("Hit enter to terminate.");
            Console.ReadLine();
        }

        private sealed class MultiWebRequests
        {
            private AsyncCoordinator m_ac = new AsyncCoordinator();
            private Dictionary<string, object> m_servers = new Dictionary<string, object>
            {
                { "https://www.baidu.com/",null },{"https://www.jd.com/",null },{"http://www.google.com/",null }
            };

            public MultiWebRequests(int timeout = Timeout.Infinite)
            {
                //以异步方式一次性发起所有请求
                var httpClient = new HttpClient();
                foreach (var server in m_servers.Keys)
                {
                    m_ac.AboutToBegin(1);
                    httpClient.GetByteArrayAsync(server).ContinueWith(task => ComputeResult(server, task));
                }

                //告诉AsyncCoordinator所有操作都已发起，并在所有操作完成调用Cancel或者发生超时的时候调用AllDone
                m_ac.AllBegun(AllDone, timeout);
            }

            private void ComputeResult(string server, Task<Byte[]> task)
            {
                object result;
                if (task.Exception != null)
                    result = task.Exception.InnerExceptions;
                else
                    result = task.Result.Length;

                m_servers[server] = result;
                m_ac.JustEnded();
            }

            public void Cancel() { m_ac.Cancel(); }

            private void AllDone(CoordinationStatus status)
            {
                switch (status)
                {
                    case CoordinationStatus.AllDone:
                        Console.WriteLine("Operation completed; results below: ");
                        ShowResult();
                        break;
                    case CoordinationStatus.Timeout:
                        Console.WriteLine("Operation timed-out.");
                        ShowResult();
                        break;
                    case CoordinationStatus.Cancel:
                        Console.WriteLine("Operation canceled.");
                        ShowResult();
                        break;
                    default:
                        break;
                }
            }

            private void ShowResult()
            {
                foreach (var server in m_servers)
                {
                    Console.Write("{0}", server.Key);
                    object result = server.Value;
                    if (result is Exception)
                        Console.WriteLine("Failed due to {0}.", result.GetType().Name);
                    else
                        Console.WriteLine("returned {0:N0} bytes", result);
                }
            }
        }

        private enum CoordinationStatus
        {
            AllDone, Timeout, Cancel
        }

        private sealed class AsyncCoordinator
        {
            private int m_opCount = 1;
            private int m_statusReported = 0;
            private Action<CoordinationStatus> m_callback;

            private Timer m_timer;

            public void AboutToBegin(int opsToAdd = 1)
            {
                Interlocked.Add(ref m_opCount, opsToAdd);
            }

            public void JustEnded()
            {
                if (Interlocked.Decrement(ref m_opCount) == 0)
                    ReportStatus(CoordinationStatus.AllDone);
            }

            public void AllBegun(Action<CoordinationStatus> callback, int timeout = Timeout.Infinite)
            {
                m_callback = callback;
                if (timeout != Timeout.Infinite)
                    m_timer = new Timer(TimeExpired, null, timeout, Timeout.Infinite);
                JustEnded();
            }

            private void TimeExpired(object o) { ReportStatus(CoordinationStatus.Timeout); }

            public void Cancel()
            {
                if (m_callback == null)
                    throw new InvalidOperationException("Cancel cannot be called before AllBegun");
                ReportStatus(CoordinationStatus.Cancel);
            }

            private void ReportStatus(CoordinationStatus status)
            {
                if (m_timer != null)
                {
                    Timer timer = Interlocked.Exchange(ref m_timer, null);
                    if (timer != null) timer.Dispose();
                }

                if (Interlocked.Exchange(ref m_statusReported, 1) == 0)
                    m_callback(status);
            }
        }
    }

    internal static class LockComparison
    {
        public static void Go()
        {
            int x = 0;
            const int iterations = 1000000;
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                x++;
            }
            Console.WriteLine("Incrementing x: {0:N0}",sw.ElapsedMilliseconds);

            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                M();x++;M();
            }
            Console.WriteLine("Incrementing x in M: {0:N0}", sw.ElapsedMilliseconds);

            SimpleSpinLock ssl = new SimpleSpinLock();
            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                ssl.Enter();x++;ssl.Leave();
            }
            Console.WriteLine("Incrementing x in SimpleSpinLock:{0:N0}", sw.ElapsedMilliseconds);

            SpinLock sl = new SpinLock(false);
            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                bool taken = false;
                sl.Enter(ref taken);x++;sl.Exit(false);
            }
            Console.WriteLine("Incrementing x in SpinLock: {0:N0}", sw.ElapsedMilliseconds);

            using (SimpleWaitLock swl=new SimpleWaitLock())
            {
                sw.Restart();
                for (int i = 0; i < iterations; i++)
                {
                    swl.Enter();x++;swl.Leave();
                }
                Console.WriteLine("Incrementing x in SimpleWaitLock: {0:N0}", sw.ElapsedMilliseconds);
            }
            Console.ReadLine();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void M() { }

        internal struct SimpleSpinLock
        {
            private int m_ResourceInUser;

            public void Enter()
            {
                while (true)
                {
                    if (Interlocked.Exchange(ref m_ResourceInUser, 1) == 0) return;
                }
            }

            public void Leave()
            {
                Volatile.Write(ref m_ResourceInUser, 0);
            }
        }

        private sealed class SimpleWaitLock : IDisposable
        {
            private readonly AutoResetEvent m_available;
            public SimpleWaitLock()
            {
                m_available = new AutoResetEvent(true);
            }

            public void Dispose()
            {
                m_available.Dispose();
            }

            public void Enter()
            {
                m_available.WaitOne();
            }

            public void Leave()
            {
                m_available.Set();
            }
        }

        internal sealed class RecursiveAutoResetEvent : IDisposable
        {
            private AutoResetEvent m_lock = new AutoResetEvent(true);
            private int m_owningThreadId = 0;
            private int m_recursionCount = 0;

            public void Dispose()
            {
                m_lock.Dispose();
            }

            public void Enter()
            {
                int currentThreadId = Thread.CurrentThread.ManagedThreadId;
                if (m_owningThreadId == currentThreadId)
                {
                    m_recursionCount++;
                    return;
                }

                m_lock.WaitOne();

                m_owningThreadId = currentThreadId;
                m_recursionCount--;
            }

            public void Leave()
            {
                if (m_owningThreadId != Thread.CurrentThread.ManagedThreadId)
                    throw new InvalidOperationException();

                if (--m_recursionCount == 0)
                {
                    m_owningThreadId = 0;
                    m_lock.Set();
                }
            }
        }
    }
}
