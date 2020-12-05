// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Components
{
    /// <summary>
    /// Wraps a callback delegate associated with an event.
    /// </summary>
    public struct EventCallbackWorkItem
    {
        /// <summary>
        /// An empty <see cref="EventCallbackWorkItem"/>.
        /// </summary>
        public static readonly EventCallbackWorkItem Empty = new EventCallbackWorkItem(null);

        private readonly MulticastDelegate _delegate;

        /// <summary>
        /// Creates a new <see cref="EventCallbackWorkItem"/> with the provided <paramref name="delegate"/>.
        /// </summary>
        /// <param name="delegate">The callback delegate.</param>
        public EventCallbackWorkItem(MulticastDelegate @delegate)
        {
            _delegate = @delegate;
        }

        /// <summary>
        /// Invokes the delegate associated with this <see cref="EventCallbackWorkItem"/>.
        /// </summary>
        /// <param name="arg">The argument to provide to the delegate. May be <c>null</c>.</param>
        /// <returns>A <see cref="Task"/> then will complete asynchronously once the delegate has completed.</returns>
        public Task InvokeAsync(object arg)
        {
            return InvokeAsync<object>(_delegate, arg);
        }

        internal static Task InvokeAsync<T>(MulticastDelegate @delegate, T arg)
        {
            switch (@delegate)
            {
                case null:
                    return Task.CompletedTask;

                case Action action:
                    action.Invoke();
                    return Task.CompletedTask;

                case Action<T> actionEventArgs:
                    actionEventArgs.Invoke(arg);
                    return Task.CompletedTask;

                case Func<Task> func:
                    return func.Invoke();

                case Func<T, Task> funcEventArgs:
                    return funcEventArgs.Invoke(arg);

                default:
                    {
                        try
                        {
                            return @delegate.DynamicInvoke(arg) as Task ?? Task.CompletedTask;
                        }
                        catch (TargetInvocationException e)
                        {
                            // Since we fell into the DynamicInvoke case, any exception will be wrapped
                            // in a TIE. We can expect this to be thrown synchronously, so it's low overhead
                            // to unwrap it.
                            return Task.FromException(e.InnerException);
                        }
                    }
            }
        }
    }
}
