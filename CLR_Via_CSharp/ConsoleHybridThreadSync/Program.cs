using System;
using System.Diagnostics;
using System.Threading;

namespace ConsoleHybridThreadSync
{
    public class Program
    {
        static void Main(string[] args)
        {
            HybridLocks.Go();
            Console.ReadLine();
        }
    }

    internal static class HybridLocks
    {
        public static void Go()
        {
            int x = 0;
            const int iterations = 10000000;

            var shl = new SimpleHybridLock();
            shl.Enter(); x++; shl.Leave();
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                shl.Enter(); x++; shl.Leave();
            }
            Console.WriteLine("Incrementing x in simpleHybridLock:{0:N0}", sw.ElapsedMilliseconds);

            using (var ahl = new AnotherHybridLock())
            {
                ahl.Enter(); x++; ahl.Leave();
                sw.Restart();
                for (int i = 0; i < iterations; i++)
                {
                    ahl.Enter(); x++; ahl.Leave();
                }
                Console.WriteLine("Incrementing x in AnotherHybridLock: {0:N0}", sw.ElapsedMilliseconds);
            }

            using (var ahl = new AnotherHybridLock())
            {
                ahl.Enter(); x++; ahl.Leave();
                sw.Restart();
                for (int i = 0; i < iterations; i++)
                {
                    ahl.Enter(); x++; ahl.Leave();
                }
                Console.WriteLine("Incrementing x in AnotherHybridLock: {0:N0}", sw.ElapsedMilliseconds);
            }
        }

        public sealed class SimpleHybridLock : IDisposable
        {
            private int m_waiters = 0;
            private readonly AutoResetEvent m_waiterLock = new AutoResetEvent(false);

            public void Enter()
            {
                if (Interlocked.Increment(ref m_waiters) == 1)
                    return;
                m_waiterLock.WaitOne();
            }

            public void Leave()
            {
                if (Interlocked.Decrement(ref m_waiters) == 0)
                    return;
                m_waiterLock.Set();
            }

            public void Dispose()
            {
                m_waiterLock.Dispose();
            }
        }

        public sealed class AnotherHybridLock : IDisposable
        {
            private int m_waiters = 0;
            private AutoResetEvent m_waiterLock = new AutoResetEvent(false);
            private int m_spincount = 4000;
            private int m_owningThreadId = 0, m_recursion = 0;

            public void Dispose()
            {
                m_waiterLock.Dispose();
            }

            public void Enter()
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                if (threadId == m_owningThreadId) { m_recursion++; return; }
                SpinWait spinwait = new SpinWait();
                for (int spinCount = 0; spinCount < m_spincount; spinCount++)
                {
                    if (Interlocked.CompareExchange(ref m_waiters, 1, 0) == 0) goto GotLock;
                    spinwait.SpinOnce();
                }

                if (Interlocked.Increment(ref m_waiters) > 1)
                {
                    m_waiterLock.WaitOne();
                }

            GotLock:
                m_owningThreadId = threadId; m_recursion = 1;
            }

            public void Leave()
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                if (threadId != m_owningThreadId)
                    throw new SynchronizationLockException("Lock not owned by calling thread");
                if (--m_recursion > 0) return;
                m_owningThreadId = 0;
                if (Interlocked.Decrement(ref m_waiters) == 0) return;
                m_waiterLock.Set();
            }
        }

        private sealed class Transactions : IDisposable
        {
            private readonly ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            private DateTime m_timeOfLastTrans;

            public void PerformTransaction()
            {
                m_lock.EnterWriteLock();
                m_timeOfLastTrans = DateTime.Now;
                m_lock.ExitWriteLock();
            }

            public void Dispose()
            {
                m_lock.Dispose();
            }

            public DateTime LastTransaction
            {
                get
                {
                    m_lock.EnterReadLock();
                    DateTime temp = m_timeOfLastTrans;
                    m_lock.ExitReadLock();
                    return temp;
                }
            }
        }
    }
}
