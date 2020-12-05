using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleIOAsync
{
    public class Program
    {
        static void Main(string[] args)
        {
            //PipeDemo.Go().Wait();
            TaskLogger.Go().Wait();
            //Cancellation.Go().Wait();
            Console.ReadLine();
        }
    }

    internal static class PipeDemo
    {
        public static async Task Go2()
        {
            var tasks = new Task[] { Task.Delay(10000), Task.Delay(1000), Task.Delay(5000), Task.Delay(3000) };
            foreach (var t in whenEach(tasks))
            {
                Console.WriteLine(Array.IndexOf(tasks, await t));
            }
        }

        public static async Task Go()
        {
            StartServer();
            Task<string>[] requests = new Task<string>[10000];
            for (int n = 0; n < requests.Length; n++)
            {
                requests[n] = IssueClientRequestAsync("localhost", "Request #" + n);
            }

#if true
            string[] responses = await Task.WhenAll(requests);
            for (int n = 0; n < responses.Length; n++)
            {
                Console.WriteLine(responses[n]);
            }
#endif
#if true
            List<Task<String>> pendingRequests = new List<Task<string>>(requests);
            while (pendingRequests.Count > 0)
            {
                Task<String> response = await Task.WhenAny(pendingRequests);
                pendingRequests.Remove(response);
                Console.WriteLine(response.Result);
            }
#endif
#if true
            foreach (var t in whenEach(requests))
            {
                Task<string> response = await t;
                Console.WriteLine(response.Result);
            }
#endif
        }

        public static IEnumerable<Task<Task<TResult>>> whenEach<TResult>(params Task<TResult>[] tasks)
        {
            var taskCompletions = new TaskCompletionSource<Task<TResult>>[tasks.Length];
            int next = -1;
            Action<Task<TResult>> taskCompletionCallback = t => taskCompletions[Interlocked.Increment(ref next)].SetResult(t);

            for (int n = 0; n < tasks.Length; n++)
            {
                taskCompletions[n] = new TaskCompletionSource<Task<TResult>>();
                tasks[n].ContinueWith(taskCompletionCallback, TaskContinuationOptions.ExecuteSynchronously);
            }

            for (int n = 0; n < tasks.Length; n++)
            {
                yield return taskCompletions[n].Task;
            }
        }

        public static IEnumerable<Task<Task>> whenEach(params Task[] tasks)
        {
            var taskCompletions = new TaskCompletionSource<Task>[tasks.Length];
            int next = -1;
            Action<Task> taskCompletionCallback = t => taskCompletions[Interlocked.Increment(ref next)].SetResult(t);
            for (int n = 0; n < tasks.Length; n++)
            {
                taskCompletions[n] = new TaskCompletionSource<Task>();
                tasks[n].ContinueWith(taskCompletionCallback, TaskContinuationOptions.ExecuteSynchronously);
            }

            for (int n = 0; n < tasks.Length; n++) yield return taskCompletions[n].Task;
        }

        private static async void StartServer()
        {
            while (true)
            {
                var pipe = new NamedPipeServerStream("PipeName", PipeDirection.InOut, -1, 
                    PipeTransmissionMode.Message, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
                await Task.Factory.FromAsync(pipe.BeginWaitForConnection, pipe.EndWaitForConnection, null);
                ServiceClientRequestAsync(pipe);
            }
        }

        private static DateTime s_lastClientRequest = DateTime.MinValue;
        private static readonly SemaphoreSlim s_lock = new SemaphoreSlim(1);

        private static async void ServiceClientRequestAsync(NamedPipeServerStream pipe)
        {
            using (pipe)
            {
                byte[] data = new byte[1000];
                int bytesRead = await pipe.ReadAsync(data, 0, data.Length);
                DateTime now = DateTime.Now;
                await s_lock.WaitAsync();

                if (s_lastClientRequest < now) s_lastClientRequest = now;
                s_lock.Release();

                data = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(data, 0, bytesRead).ToUpper().ToCharArray());

                await pipe.WriteAsync(data, 0, data.Length);
            }
        }

        private static async Task<String> IssueClientRequestAsync(string serverName, string message)
        {
            using (var pipe = new NamedPipeClientStream(serverName, "PipeName", PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough))
            {
                pipe.Connect();
                pipe.ReadMode = PipeTransmissionMode.Message;

                byte[] request = Encoding.UTF8.GetBytes(message);
                await pipe.WriteAsync(request, 0, request.Length);

                byte[] response = new byte[1000];
                int bytesRead = await pipe.ReadAsync(response, 0, response.Length);
                return Encoding.UTF8.GetString(response, 0, bytesRead);
            }
        }
    }

    internal static class AsyncFuncCodeTransformation
    {
        public static void Go()
        {
            var s = MyMethodAsync(5).Result;
        }

        private sealed class Type1 { }
        private sealed class Type2 { }
        private static Task<Type1> Method1Async() { return Task.Run(() => { return new Type1(); }); }
        private static Task<Type2> Method2Async() { return Task.Run(() => { return new Type2(); }); }

        private static async Task<string> MyMethodAsync(int argument)
        {
            int local = argument;
            try
            {
                Type1 result1 = await Method1Async();
                for (int x = 0; x < 3; x++)
                {
                    Type2 result2 = await Method2Async();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Catch");
            }
            finally
            {
                Console.WriteLine("Finally");
            }

            return "Done";
        }

    }

    public static class TaskLogger
    {
        public static async Task Go()
        {
#if DEBUG
            LogLevel = TaskLogger.TaskLogLevel.Pending;
#endif
            var tasks = new List<Task> {
                Task.Delay(2000).Log("2s op"),
                Task.Delay(5000).Log("5s op"),
                Task.Delay(6000).Log("6s op")
            };

            try
            {
                await Task.WhenAll(tasks).WithCancellation(new CancellationTokenSource(3000).Token);
            }
            catch (OperationCanceledException) { }

            foreach (var op in TaskLogger.GetLogEntries().OrderBy(tle => tle.LogTime))
            {
                Console.WriteLine(op);
            }
        }

        public enum TaskLogLevel { None, Pending }
        public static TaskLogLevel LogLevel { get; set; }
        public sealed class TaskLogEntry
        {
            public Task Task { get; internal set; }
            public string Tag { get; internal set; }
            public DateTime LogTime { get; internal set; }
            public string CallerMemberName { get; internal set; }
            public string CallerFilePath { get; internal set; }
            public int CallerLineNumer { get; internal set; }

            public override string ToString()
            {
                return string.Format("LogTime={0}, Tag={1},Member={2},File={3}{4}",
                    LogTime, Tag ?? "(none)", CallerMemberName, CallerFilePath, CallerLineNumer);
            }
        }

        private static readonly ConcurrentDictionary<Task, TaskLogEntry> s_log =
            new ConcurrentDictionary<Task, TaskLogEntry>();
        public static IEnumerable<TaskLogEntry> GetLogEntries() { return s_log.Values; }

        public static Task<TResult> Log<TResult>(this Task<TResult> task, string tag = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = -1)
        {
            return (Task<TResult>)Log((Task)task, tag, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Log(this Task task, string tag = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (LogLevel == TaskLogLevel.None) return task;
            var logEntry = new TaskLogEntry
            {
                Task = task,
                LogTime = DateTime.Now,
                Tag = tag,
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineNumer = callerLineNumber
            };
            s_log[task] = logEntry;
            task.ContinueWith(t => { TaskLogEntry entry; s_log.TryRemove(t, out entry); },
                TaskContinuationOptions.ExecuteSynchronously);
            return task;
        }
    }

    internal static class EventAwaiterDemo
    {
        public static void Go()
        {
            ShowExceptions();
            for (int x = 0; x < 3; x++)
            {
                try
                {
                    switch (x)
                    {
                        case 0: throw new InvalidOperationException();
                        case 1: throw new ObjectDisposedException("");
                        case 2: throw new ArgumentOutOfRangeException();
                    }
                }
                catch { }
            }
        }

        private static async void ShowExceptions()
        {
            var eventAwaiter = new EventAwaiter<FirstChanceExceptionEventArgs>();
            AppDomain.CurrentDomain.FirstChanceException += eventAwaiter.EventRaised;
            while (true)
            {
                Console.WriteLine("AppDomain exception:{0}", (await eventAwaiter).Exception.GetType());
            }
        }

        public sealed class EventAwaiter<TEventArgs> : INotifyCompletion
        {
            private ConcurrentQueue<TEventArgs> m_events = new ConcurrentQueue<TEventArgs>();
            private Action m_continuation;

            public EventAwaiter<TEventArgs> GetAwaiter() { return this; }

            public bool IsCompleted { get { return m_events.Count > 0; } }
            
            //状态机告诉我们以后要调用什么方法；我们把它保存起来
            public void OnCompleted(Action continuation)
            {
                Volatile.Write(ref m_continuation, continuation);
            }

            public TEventArgs GetResult()
            {
                TEventArgs e;
                m_events.TryDequeue(out e);
                return e;
            }

            //如果引发了事件，多个线程可能同时调用
            public void EventRaised(object sender,TEventArgs eventArgs)
            {
                m_events.Enqueue(eventArgs);//保存EventArgs以便从GetResult/await返回

                //如果有一个等待进行的延续任务，该线程会运行它
                Action continuation = Interlocked.Exchange(ref m_continuation, null);
                if (continuation != null) continuation();  //恢复状态机
            }
        }
    }

    internal static class Cancellation
    {
        public static async Task Go()
        {
            var cts = new CancellationTokenSource(15000);
            var ct = cts.Token;

            try
            {
                await Task.Delay(10000).WithCancellation(ct);
                Console.WriteLine("Task completed");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Task cancelled");
            }
        }

        private struct Void { } 
        public static async Task<TResult> WithCancellation<TResult>(this Task<TResult> orignalTask, CancellationToken ct)
        {
            var cancelTask = new TaskCompletionSource<Void>();
            using (ct.Register(t => ((TaskCompletionSource<Void>)t).TrySetResult(new Void()), cancelTask))
            {
                Task any = await Task.WhenAny(orignalTask, cancelTask.Task);
                if (any == cancelTask.Task) ct.ThrowIfCancellationRequested();
            }

            return await orignalTask;
        }

        public static async Task WithCancellation(this Task task,CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<Void>();
            using (ct.Register(t => ((TaskCompletionSource<Void>)t).TrySetResult(default), tcs))
            {
                if (await Task.WhenAny(task, tcs.Task) == tcs.Task) ct.ThrowIfCancellationRequested();
            }
            await task;
        }
    }
}
