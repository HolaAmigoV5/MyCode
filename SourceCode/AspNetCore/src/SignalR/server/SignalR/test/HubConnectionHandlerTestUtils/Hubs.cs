// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.AspNetCore.SignalR.Tests
{
    public class MethodHub : TestHub
    {
        public Task GroupRemoveMethod(string groupName)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public Task ClientSendMethod(string userId, string message)
        {
            return Clients.User(userId).SendAsync("Send", message);
        }

        public Task SendToMultipleUsers(IReadOnlyList<string> userIds, string message)
        {
            return Clients.Users(userIds).SendAsync("Send", message);
        }

        public Task ConnectionSendMethod(string connectionId, string message)
        {
            return Clients.Client(connectionId).SendAsync("Send", message);
        }

        public Task SendToMultipleClients(string message, IReadOnlyList<string> connectionIds)
        {
            return Clients.Clients(connectionIds).SendAsync("Send", message);
        }

        public Task GroupAddMethod(string groupName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public Task GroupSendMethod(string groupName, string message)
        {
            return Clients.Group(groupName).SendAsync("Send", message);
        }

        public Task GroupExceptSendMethod(string groupName, string message, IReadOnlyList<string> excludedConnectionIds)
        {
            return Clients.GroupExcept(groupName, excludedConnectionIds).SendAsync("Send", message);
        }

        public Task SendToMultipleGroups(string message, IReadOnlyList<string> groupNames)
        {
            return Clients.Groups(groupNames).SendAsync("Send", message);
        }

        public Task SendToOthersInGroup(string groupName, string message)
        {
            return Clients.OthersInGroup(groupName).SendAsync("Send", message);
        }

        public Task BroadcastMethod(string message)
        {
            return Clients.All.SendAsync("Broadcast", message);
        }

        public Task BroadcastItem()
        {
            return Clients.All.SendAsync("Broadcast", new Result { Message = "test", paramName = "param" });
        }

        public Task SendArray()
        {
            return Clients.All.SendAsync("Array", new[] { 1, 2, 3 });
        }

        public Task<int> TaskValueMethod()
        {
            return Task.FromResult(42);
        }

        public int ValueMethod()
        {
            return 43;
        }

        [HubMethodName("RenamedMethod")]
        public int ATestMethodThatIsRenamedByTheAttribute()
        {
            return 43;
        }

        public string Echo(string data)
        {
            return data;
        }

        public void VoidMethod()
        {
        }

        public string ConcatString(byte b, int i, char c, string s)
        {
            return $"{b}, {i}, {c}, {s}";
        }

        public Task SendAnonymousObject()
        {
            return Clients.Client(Context.ConnectionId).SendAsync("Send", new { });
        }

        public override Task OnDisconnectedAsync(Exception e)
        {
            return Task.CompletedTask;
        }

        public void MethodThatThrows()
        {
            throw new InvalidOperationException("BOOM!");
        }

        public void ThrowHubException()
        {
            throw new HubException("This is a hub exception");
        }

        public Task MethodThatYieldsFailedTask()
        {
            return Task.FromException(new InvalidOperationException("BOOM!"));
        }

        public static void StaticMethod()
        {
        }

        [Authorize("test")]
        public void AuthMethod()
        {
        }

        public Task SendToAllExcept(string message, IReadOnlyList<string> excludedConnectionIds)
        {
            return Clients.AllExcept(excludedConnectionIds).SendAsync("Send", message);
        }

        public bool HasHttpContext()
        {
            return Context.GetHttpContext() != null;
        }

        public Task SendToOthers(string message)
        {
            return Clients.Others.SendAsync("Send", message);
        }

        public Task SendToCaller(string message)
        {
            return Clients.Caller.SendAsync("Send", message);
        }

        public Task ProtocolError()
        {
            return Clients.Caller.SendAsync("Send", new string('x', 3000), new SelfRef());
        }

        public void InvalidArgument(CancellationToken token)
        {
        }

        private class SelfRef
        {
            public SelfRef()
            {
                Self = this;
            }

            public SelfRef Self;
        }

        public async Task<string> StreamingConcat(ChannelReader<string> source)
        {
            var sb = new StringBuilder();

            while (await source.WaitToReadAsync())
            {
                while (source.TryRead(out var item))
                {
                    sb.Append(item);
                }
            }

            return sb.ToString();
        }

        public async Task<int> StreamingSum(ChannelReader<int> source)
        {
            var total = 0;
            while (await source.WaitToReadAsync())
            {
                while (source.TryRead(out var item))
                {
                    total += item;
                }
            }
            return total;
        }

        public async Task<List<object>> UploadArray(ChannelReader<object> source)
        {
            var results = new List<object>();

            while (await source.WaitToReadAsync())
            {
                while (source.TryRead(out var item))
                {
                    results.Add(item);
                }
            }

            return results;
        }

        public async Task<string> TestTypeCastingErrors(ChannelReader<int> source)
        {
            try
            {
                await source.WaitToReadAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "error identified and caught";
            }

            return "wrong type accepted, this is bad";
        }

        public async Task<bool> TestCustomErrorPassing(ChannelReader<int> source)
        {
            try
            {
                await source.WaitToReadAsync();
            }
            catch (Exception ex)
            {
                return ex.Message == HubConnectionHandlerTests.CustomErrorMessage;
            }

            return false;
        }

        public Task UploadIgnoreItems(ChannelReader<string> source)
        {
            // Wait for an item to appear first then return from the hub method to end the invocation
            return source.WaitToReadAsync().AsTask();
        }

        public ChannelReader<string> StreamAndUploadIgnoreItems(ChannelReader<string> source)
        {
            var channel = Channel.CreateUnbounded<string>();
            _ = ChannelFunc(channel.Writer, source);

            return channel.Reader;

            async Task ChannelFunc(ChannelWriter<string> output, ChannelReader<string> input)
            {
                // Wait for an item to appear first then return from the hub method to end the invocation
                await input.WaitToReadAsync();
                output.Complete();
            }
        }
    }

    public abstract class TestHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var tcs = (TaskCompletionSource<bool>)Context.Items["ConnectedTask"];
            tcs?.TrySetResult(true);
            return base.OnConnectedAsync();
        }
    }

    public class DynamicTestHub : DynamicHub
    {
        public override Task OnConnectedAsync()
        {
            var tcs = (TaskCompletionSource<bool>)Context.Items["ConnectedTask"];
            tcs?.TrySetResult(true);
            return base.OnConnectedAsync();
        }

        public string Echo(string data)
        {
            return data;
        }

        public Task ClientSendMethod(string userId, string message)
        {
            return Clients.User(userId).Send(message);
        }

        public Task SendToMultipleUsers(IReadOnlyList<string> userIds, string message)
        {
            return Clients.Users(userIds).Send(message);
        }

        public Task ConnectionSendMethod(string connectionId, string message)
        {
            return Clients.Client(connectionId).Send(message);
        }

        public Task SendToMultipleClients(string message, IReadOnlyList<string> connectionIds)
        {
            return Clients.Clients(connectionIds).Send(message);
        }

        public Task GroupAddMethod(string groupName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public Task GroupSendMethod(string groupName, string message)
        {
            return Clients.Group(groupName).Send(message);
        }

        public Task GroupExceptSendMethod(string groupName, string message, IReadOnlyList<string> excludedConnectionIds)
        {
            return Clients.GroupExcept(groupName, excludedConnectionIds).Send(message);
        }

        public Task SendToOthersInGroup(string groupName, string message)
        {
            return Clients.OthersInGroup(groupName).Send(message);
        }

        public Task SendToMultipleGroups(string message, IReadOnlyList<string> groupNames)
        {
            return Clients.Groups(groupNames).Send(message);
        }

        public Task BroadcastMethod(string message)
        {
            return Clients.All.Broadcast(message);
        }

        public Task SendToAllExcept(string message, IReadOnlyList<string> excludedConnectionIds)
        {
            return Clients.AllExcept(excludedConnectionIds).Send(message);
        }

        public Task SendToOthers(string message)
        {
            return Clients.Others.Send(message);
        }

        public Task SendToCaller(string message)
        {
            return Clients.Caller.Send(message);
        }
    }

    public class HubT : Hub<Test>
    {
        public override Task OnConnectedAsync()
        {
            var tcs = (TaskCompletionSource<bool>)Context.Items["ConnectedTask"];
            tcs?.TrySetResult(true);
            return base.OnConnectedAsync();
        }

        public string Echo(string data)
        {
            return data;
        }

        public Task ClientSendMethod(string userId, string message)
        {
            return Clients.User(userId).Send(message);
        }

        public Task SendToMultipleUsers(IReadOnlyList<string> userIds, string message)
        {
            return Clients.Users(userIds).Send(message);
        }

        public Task ConnectionSendMethod(string connectionId, string message)
        {
            return Clients.Client(connectionId).Send(message);
        }

        public Task SendToMultipleClients(string message, IReadOnlyList<string> connectionIds)
        {
            return Clients.Clients(connectionIds).Send(message);
        }

        public async Task DelayedSend(string connectionId, string message)
        {
            await Task.Delay(100);
            await Clients.Client(connectionId).Send(message);
        }

        public Task GroupAddMethod(string groupName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public Task GroupSendMethod(string groupName, string message)
        {
            return Clients.Group(groupName).Send(message);
        }

        public Task GroupExceptSendMethod(string groupName, string message, IReadOnlyList<string> excludedConnectionIds)
        {
            return Clients.GroupExcept(groupName, excludedConnectionIds).Send(message);
        }

        public Task SendToMultipleGroups(string message, IReadOnlyList<string> groupNames)
        {
            return Clients.Groups(groupNames).Send(message);
        }

        public Task SendToOthersInGroup(string groupName, string message)
        {
            return Clients.OthersInGroup(groupName).Send(message);
        }

        public Task BroadcastMethod(string message)
        {
            return Clients.All.Broadcast(message);
        }

        public Task SendToAllExcept(string message, IReadOnlyList<string> excludedConnectionIds)
        {
            return Clients.AllExcept(excludedConnectionIds).Send(message);
        }

        public Task SendToOthers(string message)
        {
            return Clients.Others.Send(message);
        }

        public Task SendToCaller(string message)
        {
            return Clients.Caller.Send(message);
        }
    }

    public interface Test
    {
        Task Send(string message);
        Task Broadcast(string message);
    }

    public class OnConnectedThrowsHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(new InvalidOperationException("Hub OnConnected failed."));
            return tcs.Task;
        }
    }

    public class OnDisconnectedThrowsHub : TestHub
    {
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(new InvalidOperationException("Hub OnDisconnected failed."));
            return tcs.Task;
        }
    }

    public class InheritedHub : BaseHub
    {
        public override int VirtualMethod(int num)
        {
            return num - 10;
        }

        public override int VirtualMethodRenamed()
        {
            return 34;
        }
    }

    public class BaseHub : TestHub
    {
        public string BaseMethod(string message)
        {
            return message;
        }

        public virtual int VirtualMethod(int num)
        {
            return num;
        }

        [HubMethodName("RenamedVirtualMethod")]
        public virtual int VirtualMethodRenamed()
        {
            return 43;
        }
    }

    public class InvalidHub : TestHub
    {
        public void OverloadedMethod(int num)
        {
        }

        public void OverloadedMethod(string message)
        {
        }
    }

    public class DisposeTrackingHub : TestHub
    {
        private readonly TrackDispose _trackDispose;

        public DisposeTrackingHub(TrackDispose trackDispose)
        {
            _trackDispose = trackDispose;
        }

        protected override void Dispose(bool dispose)
        {
            if (dispose)
            {
                _trackDispose.DisposeCount++;
            }
        }
    }

    public class HubWithAsyncDisposable : TestHub
    {
        private AsyncDisposable _disposable;

        public HubWithAsyncDisposable(AsyncDisposable disposable)
        {
            _disposable = disposable;
        }

        public void Test()
        {

        }
    }

    public class AbortHub : Hub
    {
        public void Kill()
        {
            Context.Abort();
        }
    }

    public class StreamingHub : TestHub
    {
        public ChannelReader<string> CounterChannel(int count)
        {
            var channel = Channel.CreateUnbounded<string>();

            _ = Task.Run(async () =>
            {
                for (int i = 0; i < count; i++)
                {
                    await channel.Writer.WriteAsync(i.ToString());
                }
                channel.Writer.Complete();
            });

            return channel.Reader;
        }

        public async Task<ChannelReader<string>> CounterChannelAsync(int count)
        {
            await Task.Yield();
            return CounterChannel(count);
        }

        public async ValueTask<ChannelReader<string>> CounterChannelValueTaskAsync(int count)
        {
            await Task.Yield();
            return CounterChannel(count);
        }

        public async IAsyncEnumerable<string> CounterAsyncEnumerable(int count)
        {
            for (int i = 0; i < count; i++)
            {
                await Task.Yield();
                yield return i.ToString();
            }
        }

        public async Task<IAsyncEnumerable<string>> CounterAsyncEnumerableAsync(int count)
        {
            await Task.Yield();
            return CounterAsyncEnumerable(count);
        }

        public AsyncEnumerableImpl<string> CounterAsyncEnumerableImpl(int count)
        {
            return new AsyncEnumerableImpl<string>(CounterAsyncEnumerable(count));
        }

        public AsyncEnumerableImplChannelThrows<string> AsyncEnumerableIsPreferedOverChannelReader(int count)
        {
            return new AsyncEnumerableImplChannelThrows<string>(CounterChannel(count));
        }

        public ChannelReader<string> BlockingStream()
        {
            return Channel.CreateUnbounded<string>().Reader;
        }

        public ChannelReader<int> ThrowStream()
        {
            var channel = Channel.CreateUnbounded<int>();
            channel.Writer.TryComplete(new Exception("Exception from channel"));
            return channel.Reader;
        }

        public int NonStream()
        {
            return 42;
        }

        public ChannelReader<string> StreamEcho(ChannelReader<string> source)
        {
            Channel<string> output = Channel.CreateUnbounded<string>();

            _ = Task.Run(async () =>
            {
                while (await source.WaitToReadAsync())
                {
                    while (source.TryRead(out string item))
                    {
                        await output.Writer.WriteAsync("echo:" + item);
                    }
                }

                output.Writer.TryComplete();
            });

            return output.Reader;
        }

        public class AsyncEnumerableImpl<T> : IAsyncEnumerable<T>
        {
            private readonly IAsyncEnumerable<T> _inner;

            public AsyncEnumerableImpl(IAsyncEnumerable<T> inner)
            {
                _inner = inner;
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return _inner.GetAsyncEnumerator(cancellationToken);
            }
        }

        public class AsyncEnumerableImplChannelThrows<T> : ChannelReader<T>, IAsyncEnumerable<T>
        {
            private ChannelReader<T> _inner;

            public AsyncEnumerableImplChannelThrows(ChannelReader<T> inner)
            {
                _inner = inner;
            }

            public override bool TryRead(out T item)
            {
                // Not implemented to verify this is consumed as an IAsyncEnumerable<T> instead of a ChannelReader<T>.
                throw new NotImplementedException();
            }

            public override ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default)
            {
                // Not implemented to verify this is consumed as an IAsyncEnumerable<T> instead of a ChannelReader<T>.
                throw new NotImplementedException();
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new ChannelAsyncEnumerator(_inner, cancellationToken);
            }

            // Copied from AsyncEnumeratorAdapters
            private class ChannelAsyncEnumerator : IAsyncEnumerator<T>
            {
                /// <summary>The channel being enumerated.</summary>
                private readonly ChannelReader<T> _channel;
                /// <summary>Cancellation token used to cancel the enumeration.</summary>
                private readonly CancellationToken _cancellationToken;
                /// <summary>The current element of the enumeration.</summary>
                private T _current;

                public ChannelAsyncEnumerator(ChannelReader<T> channel, CancellationToken cancellationToken)
                {
                    _channel = channel;
                    _cancellationToken = cancellationToken;
                }

                public T Current => _current;

                public ValueTask<bool> MoveNextAsync()
                {
                    var result = _channel.ReadAsync(_cancellationToken);

                    if (result.IsCompletedSuccessfully)
                    {
                        _current = result.Result;
                        return new ValueTask<bool>(true);
                    }

                    return new ValueTask<bool>(MoveNextAsyncAwaited(result));
                }

                private async Task<bool> MoveNextAsyncAwaited(ValueTask<T> channelReadTask)
                {
                    try
                    {
                        _current = await channelReadTask;
                    }
                    catch (ChannelClosedException ex) when (ex.InnerException == null)
                    {
                        return false;
                    }

                    return true;
                }

                public ValueTask DisposeAsync()
                {
                    return default;
                }
            }
        }
    }

    public class SimpleHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("Send", $"{Context.ConnectionId} joined");
            await base.OnConnectedAsync();
        }
    }

    public class SimpleVoidReturningTypedHub : Hub<IVoidReturningTypedHubClient>
    {
        public override Task OnConnectedAsync()
        {
            // Derefernce Clients, to force initialization of the TypedHubClient
            Clients.All.Send("herp");
            return Task.CompletedTask;
        }
    }

    public class SimpleTypedHub : Hub<ITypedHubClient>
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.Send($"{Context.ConnectionId} joined");
            await base.OnConnectedAsync();
        }
    }

    public class LongRunningHub : Hub
    {
        private TcsService _tcsService;

        public LongRunningHub(TcsService tcsService)
        {
            _tcsService = tcsService;
        }

        public async Task<int> LongRunningMethod()
        {
            _tcsService.StartedMethod.TrySetResult(null);
            await _tcsService.EndMethod.Task;
            return 12;
        }

        public async Task<ChannelReader<string>> LongRunningStream()
        {
            _tcsService.StartedMethod.TrySetResult(null);
            await _tcsService.EndMethod.Task;
            // Never ending stream
            return Channel.CreateUnbounded<string>().Reader;
        }

        public ChannelReader<int> CancelableStreamSingleParameter(CancellationToken token)
        {
            var channel = Channel.CreateBounded<int>(10);

            Task.Run(async () =>
            {
                _tcsService.StartedMethod.SetResult(null);
                await token.WaitForCancellationAsync();
                channel.Writer.TryComplete();
                _tcsService.EndMethod.SetResult(null);
            });

            return channel.Reader;
        }

        public ChannelReader<int> CancelableStreamMultiParameter(int ignore, int ignore2, CancellationToken token)
        {
            var channel = Channel.CreateBounded<int>(10);

            Task.Run(async () =>
            {
                _tcsService.StartedMethod.SetResult(null);
                await token.WaitForCancellationAsync();
                channel.Writer.TryComplete();
                _tcsService.EndMethod.SetResult(null);
            });

            return channel.Reader;
        }

        public ChannelReader<int> CancelableStreamMiddleParameter(int ignore, CancellationToken token, int ignore2)
        {
            var channel = Channel.CreateBounded<int>(10);

            Task.Run(async () =>
            {
                _tcsService.StartedMethod.SetResult(null);
                await token.WaitForCancellationAsync();
                channel.Writer.TryComplete();
                _tcsService.EndMethod.SetResult(null);
            });

            return channel.Reader;
        }

        public async IAsyncEnumerable<int> CancelableStreamGeneratedAsyncEnumerable(CancellationToken token)
        {
            _tcsService.StartedMethod.SetResult(null);
            await token.WaitForCancellationAsync();
            _tcsService.EndMethod.SetResult(null);
            yield break;
        }

        public IAsyncEnumerable<int> CancelableStreamCustomAsyncEnumerable()
        {
            return new CustomAsyncEnumerable(_tcsService);
        }

        public int SimpleMethod()
        {
            return 21;
        }

        private class CustomAsyncEnumerable : IAsyncEnumerable<int>
        {
            private readonly TcsService _tcsService;

            public CustomAsyncEnumerable(TcsService tcsService)
            {
                _tcsService = tcsService;
            }

            public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new CustomAsyncEnumerator(_tcsService, cancellationToken);
            }

            private class CustomAsyncEnumerator : IAsyncEnumerator<int>
            {
                private readonly TcsService _tcsService;
                private readonly CancellationToken _cancellationToken;

                public CustomAsyncEnumerator(TcsService tcsService, CancellationToken cancellationToken)
                {
                    _tcsService = tcsService;
                    _cancellationToken = cancellationToken;
                }

                public int Current => throw new NotImplementedException();

                public ValueTask DisposeAsync()
                {
                    return default;
                }

                public async ValueTask<bool> MoveNextAsync()
                {
                    _tcsService.StartedMethod.SetResult(null);
                    await _cancellationToken.WaitForCancellationAsync();
                    _tcsService.EndMethod.SetResult(null);
                    return false;
                }
            }
        }
    }

    public class TcsService
    {
        public TaskCompletionSource<object> StartedMethod = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource<object> EndMethod = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public interface ITypedHubClient
    {
        Task Send(string message);
    }

    public interface IVoidReturningTypedHubClient
    {
        void Send(string message);
    }

    public class ErrorInAbortedTokenHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Context.Items[nameof(OnConnectedAsync)] = true;

            Context.ConnectionAborted.Register(() =>
            {
                throw new InvalidOperationException("BOOM");
            });

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Context.Items[nameof(OnDisconnectedAsync)] = true;

            return base.OnDisconnectedAsync(exception);
        }
    }

    public class ConnectionLifetimeHub : Hub
    {
        private readonly ConnectionLifetimeState _state;

        public ConnectionLifetimeHub(ConnectionLifetimeState state)
        {
            _state = state;
        }

        public override Task OnConnectedAsync()
        {
            _state.TokenStateInConnected = Context.ConnectionAborted.IsCancellationRequested;

            Context.ConnectionAborted.Register(() =>
            {
                _state.TokenCallbackTriggered = true;
            });

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _state.TokenStateInDisconnected = Context.ConnectionAborted.IsCancellationRequested;

            return base.OnDisconnectedAsync(exception);
        }
    }

    public class ConnectionLifetimeState
    {
        public bool TokenCallbackTriggered { get; set; }

        public bool TokenStateInConnected { get; set; }

        public bool TokenStateInDisconnected { get; set; }
    }
}
