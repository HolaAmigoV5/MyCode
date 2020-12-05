// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Components.Rendering
{
    [DebuggerDisplay("{_state,nq}")]
    internal class RendererSynchronizationContext : SynchronizationContext, IDispatcher
    {
        private static readonly ContextCallback ExecutionContextThunk = (object state) =>
        {
            var item = (WorkItem)state;
            item.SynchronizationContext.ExecuteSynchronously(null, item.Callback, item.State);
        };

        private static readonly Action<Task, object> BackgroundWorkThunk = (Task task, object state) =>
        {
            var item = (WorkItem)state;
            item.SynchronizationContext.ExecuteBackground(item);
        };

        private readonly State _state;

        public event UnhandledExceptionEventHandler UnhandledException;

        public RendererSynchronizationContext()
            : this(new State())
        {
        }

        private RendererSynchronizationContext(State state)
        {
            _state = state;
        }

        public Task Invoke(Action action)
        {
            var completion = new TaskCompletionSource<object>();
            Post(_ =>
            {
                try
                {
                    action();
                    completion.SetResult(null);
                }
                catch (OperationCanceledException)
                {
                    completion.SetCanceled();
                }
                catch (Exception exception)
                {
                    completion.SetException(exception);
                }
            }, null);

            return completion.Task;
        }

        public Task InvokeAsync(Func<Task> asyncAction)
        {
            var completion = new TaskCompletionSource<object>();
            Post(async (_) =>
            {
                try
                {
                    await asyncAction();
                    completion.SetResult(null);
                }
                catch (OperationCanceledException)
                {
                    completion.SetCanceled();
                }
                catch (Exception exception)
                {
                    completion.SetException(exception);
                }
            }, null);

            return completion.Task;
        }

        public Task<TResult> Invoke<TResult>(Func<TResult> function)
        {
            var completion = new TaskCompletionSource<TResult>();
            Post(_ =>
            {
                try
                {
                    var result = function();
                    completion.SetResult(result);
                }
                catch (OperationCanceledException)
                {
                    completion.SetCanceled();
                }
                catch (Exception exception)
                {
                    completion.SetException(exception);
                }
            }, null);

            return completion.Task;
        }

        public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> asyncFunction)
        {
            var completion = new TaskCompletionSource<TResult>();
            Post(async (_) =>
            {
                try
                {
                    var result = await asyncFunction();
                    completion.SetResult(result);
                }
                catch (OperationCanceledException)
                {
                    completion.SetCanceled();
                }
                catch (Exception exception)
                {
                    completion.SetException(exception);
                }
            }, null);

            return completion.Task;
        }

        // asynchronously runs the callback
        public override void Post(SendOrPostCallback d, object state)
        {
            TaskCompletionSource<object> completion;
            lock (_state.Lock)
            {
                if (!_state.Task.IsCompleted)
                {
                    _state.Task = Enqueue(_state.Task, d, state);
                    return;
                }

                // We can execute this synchronously because nothing is currently running
                // or queued.
                completion = new TaskCompletionSource<object>();
                _state.Task = completion.Task;
            }

            ExecuteSynchronously(completion, d, state);
        }

        // synchronously runs the callback
        public override void Send(SendOrPostCallback d, object state)
        {
            Task antecedant;
            var completion = new TaskCompletionSource<object>();

            lock (_state.Lock)
            {
                antecedant = _state.Task;
                _state.Task = completion.Task;
            }

            // We have to block. That's the contract of Send - we don't expect this to be used
            // in many scenarios in Components.
            //
            // Using Wait here is ok because the antecedant task will never throw.
            antecedant.Wait();

            ExecuteSynchronously(completion, d, state);
        }

        // shallow copy
        public override SynchronizationContext CreateCopy()
        {
            return new RendererSynchronizationContext(_state);
        }

        private Task Enqueue(Task antecedant, SendOrPostCallback d, object state)
        {
            // If we get here is means that a callback is being queued while something is currently executing
            // in this context. Let's instead add it to the queue and yield.
            //
            // We use our own queue here to maintain the execution order of the callbacks scheduled here. Also
            // we need a queue rather than just scheduling an item in the thread pool - those items would immediately
            // block and hurt scalability.
            //
            // We need to capture the execution context so we can restore it later. This code is similar to
            // the call path of ThreadPool.QueueUserWorkItem and System.Threading.QueueUserWorkItemCallback.
            ExecutionContext executionContext = null;
            if (!ExecutionContext.IsFlowSuppressed())
            {
                executionContext = ExecutionContext.Capture();
            }

            return antecedant.ContinueWith(BackgroundWorkThunk, new WorkItem()
            {
                SynchronizationContext = this,
                ExecutionContext = executionContext,
                Callback = d,
                State = state,
            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        private void ExecuteSynchronously(
            TaskCompletionSource<object> completion,
            SendOrPostCallback d,
            object state)
        {
            var original = Current;
            try
            {
                SetSynchronizationContext(this);
                _state.IsBusy = true;

                d(state);
            }
            finally
            {
                _state.IsBusy = false;
                SetSynchronizationContext(original);

                completion?.SetResult(null);
            }
        }

        private void ExecuteBackground(WorkItem item)
        {
            if (item.ExecutionContext == null)
            {
                try
                {
                    ExecuteSynchronously(null, item.Callback, item.State);
                }
                catch (Exception ex)
                {
                    DispatchException(ex);
                }

                return;
            }

            // Perf - using a static thunk here to avoid a delegate allocation.
            try
            {
                ExecutionContext.Run(item.ExecutionContext, ExecutionContextThunk, item);
            }
            catch (Exception ex)
            {
                DispatchException(ex);
            }
        }

        private void DispatchException(Exception ex)
        {
            var handler = UnhandledException;
            if (handler != null)
            {
                handler(this, new UnhandledExceptionEventArgs(ex, isTerminating: false));
            }
        }

        private class State
        {
            public bool IsBusy; // Just for debugging
            public object Lock = new object();
            public Task Task = Task.CompletedTask;

            public override string ToString()
            {
                return $"{{ Busy: {IsBusy}, Pending Task: {Task} }}";
            }
        }

        private class WorkItem
        {
            public RendererSynchronizationContext SynchronizationContext;
            public ExecutionContext ExecutionContext;
            public SendOrPostCallback Callback;
            public object State;
        }
    }
}
