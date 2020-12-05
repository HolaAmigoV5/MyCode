// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Internal
{
    public static partial class AwaitableThreadPool
    {
        public static Microsoft.AspNetCore.Internal.AwaitableThreadPool.Awaitable Yield() { throw null; }
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, Size=1)]
        public readonly partial struct Awaitable : System.Runtime.CompilerServices.ICriticalNotifyCompletion, System.Runtime.CompilerServices.INotifyCompletion
        {
            public bool IsCompleted { get { throw null; } }
            public Microsoft.AspNetCore.Internal.AwaitableThreadPool.Awaitable GetAwaiter() { throw null; }
            public void GetResult() { }
            public void OnCompleted(System.Action continuation) { }
            public void UnsafeOnCompleted(System.Action continuation) { }
        }
    }
}
namespace Microsoft.AspNetCore.SignalR.Client
{
    public partial class HubConnection
    {
        public static readonly System.TimeSpan DefaultHandshakeTimeout;
        public static readonly System.TimeSpan DefaultKeepAliveInterval;
        public static readonly System.TimeSpan DefaultServerTimeout;
        public HubConnection(Microsoft.AspNetCore.SignalR.Client.IConnectionFactory connectionFactory, Microsoft.AspNetCore.SignalR.Protocol.IHubProtocol protocol, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory) { }
        public HubConnection(Microsoft.AspNetCore.SignalR.Client.IConnectionFactory connectionFactory, Microsoft.AspNetCore.SignalR.Protocol.IHubProtocol protocol, System.IServiceProvider serviceProvider, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory) { }
        public string ConnectionId { get { throw null; } }
        public System.TimeSpan HandshakeTimeout { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute]set { } }
        public System.TimeSpan KeepAliveInterval { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute]set { } }
        public System.TimeSpan ServerTimeout { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute]set { } }
        public Microsoft.AspNetCore.SignalR.Client.HubConnectionState State { get { throw null; } }
        public event System.Func<System.Exception, System.Threading.Tasks.Task> Closed { add { } remove { } }
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public System.Threading.Tasks.Task DisposeAsync() { throw null; }
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public System.Threading.Tasks.Task<object> InvokeCoreAsync(string methodName, System.Type returnType, object[] args, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public System.IDisposable On(string methodName, System.Type[] parameterTypes, System.Func<object[], object, System.Threading.Tasks.Task> handler, object state) { throw null; }
        public void Remove(string methodName) { }
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public System.Threading.Tasks.Task SendCoreAsync(string methodName, object[] args, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public System.Threading.Tasks.Task StartAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public System.Threading.Tasks.Task StopAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public System.Threading.Tasks.Task<System.Threading.Channels.ChannelReader<object>> StreamAsChannelCoreAsync(string methodName, System.Type returnType, object[] args, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
    }
    public partial class HubConnectionBuilder : Microsoft.AspNetCore.SignalR.Client.IHubConnectionBuilder, Microsoft.AspNetCore.SignalR.ISignalRBuilder
    {
        public HubConnectionBuilder() { }
        public Microsoft.Extensions.DependencyInjection.IServiceCollection Services { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } }
        public Microsoft.AspNetCore.SignalR.Client.HubConnection Build() { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public override bool Equals(object obj) { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public override int GetHashCode() { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public new System.Type GetType() { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public override string ToString() { throw null; }
    }
    public static partial class HubConnectionBuilderExtensions
    {
        public static Microsoft.AspNetCore.SignalR.Client.IHubConnectionBuilder ConfigureLogging(this Microsoft.AspNetCore.SignalR.Client.IHubConnectionBuilder hubConnectionBuilder, System.Action<Microsoft.Extensions.Logging.ILoggingBuilder> configureLogging) { throw null; }
    }
    public static partial class HubConnectionExtensions
    {
        public static System.Threading.Tasks.Task InvokeAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task InvokeAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task InvokeAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task InvokeAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task InvokeAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task InvokeAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task InvokeAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task InvokeAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task InvokeAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task InvokeAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task InvokeAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TResult> InvokeAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TResult> InvokeAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TResult> InvokeAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TResult> InvokeAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TResult> InvokeAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TResult> InvokeAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TResult> InvokeAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TResult> InvokeAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TResult> InvokeAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TResult> InvokeAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TResult> InvokeAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task InvokeCoreAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object[] args, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static System.Threading.Tasks.Task<TResult> InvokeCoreAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object[] args, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.IDisposable On(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Action handler) { throw null; }
        public static System.IDisposable On(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Func<System.Threading.Tasks.Task> handler) { throw null; }
        public static System.IDisposable On(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Type[] parameterTypes, System.Func<object[], System.Threading.Tasks.Task> handler) { throw null; }
        public static System.IDisposable On<T1>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Action<T1> handler) { throw null; }
        public static System.IDisposable On<T1>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Func<T1, System.Threading.Tasks.Task> handler) { throw null; }
        public static System.IDisposable On<T1, T2>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Action<T1, T2> handler) { throw null; }
        public static System.IDisposable On<T1, T2>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Func<T1, T2, System.Threading.Tasks.Task> handler) { throw null; }
        public static System.IDisposable On<T1, T2, T3>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Action<T1, T2, T3> handler) { throw null; }
        public static System.IDisposable On<T1, T2, T3>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Func<T1, T2, T3, System.Threading.Tasks.Task> handler) { throw null; }
        public static System.IDisposable On<T1, T2, T3, T4>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Action<T1, T2, T3, T4> handler) { throw null; }
        public static System.IDisposable On<T1, T2, T3, T4>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Func<T1, T2, T3, T4, System.Threading.Tasks.Task> handler) { throw null; }
        public static System.IDisposable On<T1, T2, T3, T4, T5>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Action<T1, T2, T3, T4, T5> handler) { throw null; }
        public static System.IDisposable On<T1, T2, T3, T4, T5>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Func<T1, T2, T3, T4, T5, System.Threading.Tasks.Task> handler) { throw null; }
        public static System.IDisposable On<T1, T2, T3, T4, T5, T6>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Action<T1, T2, T3, T4, T5, T6> handler) { throw null; }
        public static System.IDisposable On<T1, T2, T3, T4, T5, T6>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Func<T1, T2, T3, T4, T5, T6, System.Threading.Tasks.Task> handler) { throw null; }
        public static System.IDisposable On<T1, T2, T3, T4, T5, T6, T7>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Action<T1, T2, T3, T4, T5, T6, T7> handler) { throw null; }
        public static System.IDisposable On<T1, T2, T3, T4, T5, T6, T7>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Func<T1, T2, T3, T4, T5, T6, T7, System.Threading.Tasks.Task> handler) { throw null; }
        public static System.IDisposable On<T1, T2, T3, T4, T5, T6, T7, T8>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Action<T1, T2, T3, T4, T5, T6, T7, T8> handler) { throw null; }
        public static System.IDisposable On<T1, T2, T3, T4, T5, T6, T7, T8>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, System.Threading.Tasks.Task> handler) { throw null; }
        public static System.Threading.Tasks.Task SendAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task SendAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task SendAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task SendAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task SendAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task SendAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task SendAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task SendAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task SendAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task SendAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task SendAsync(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Threading.Channels.ChannelReader<TResult>> StreamAsChannelAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Threading.Channels.ChannelReader<TResult>> StreamAsChannelAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Threading.Channels.ChannelReader<TResult>> StreamAsChannelAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Threading.Channels.ChannelReader<TResult>> StreamAsChannelAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Threading.Channels.ChannelReader<TResult>> StreamAsChannelAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Threading.Channels.ChannelReader<TResult>> StreamAsChannelAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Threading.Channels.ChannelReader<TResult>> StreamAsChannelAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, object arg4, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Threading.Channels.ChannelReader<TResult>> StreamAsChannelAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, object arg3, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Threading.Channels.ChannelReader<TResult>> StreamAsChannelAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, object arg2, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Threading.Channels.ChannelReader<TResult>> StreamAsChannelAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object arg1, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Threading.Channels.ChannelReader<TResult>> StreamAsChannelAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static System.Threading.Tasks.Task<System.Threading.Channels.ChannelReader<TResult>> StreamAsChannelCoreAsync<TResult>(this Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection, string methodName, object[] args, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
    }
    public enum HubConnectionState
    {
        Connected = 1,
        Disconnected = 0,
    }
    public partial interface IConnectionFactory
    {
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Connections.ConnectionContext> ConnectAsync(Microsoft.AspNetCore.Connections.TransferFormat transferFormat, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        System.Threading.Tasks.Task DisposeAsync(Microsoft.AspNetCore.Connections.ConnectionContext connection);
    }
    public partial interface IHubConnectionBuilder : Microsoft.AspNetCore.SignalR.ISignalRBuilder
    {
        Microsoft.AspNetCore.SignalR.Client.HubConnection Build();
    }
}
