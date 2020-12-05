// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Internal;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.SignalR.Microbenchmarks
{
    public class DefaultHubDispatcherBenchmark
    {
        private DefaultHubDispatcher<TestHub> _dispatcher;
        private HubConnectionContext _connectionContext;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSignalRCore();

            var provider = serviceCollection.BuildServiceProvider();

            var serviceScopeFactory = provider.GetService<IServiceScopeFactory>();

            _dispatcher = new DefaultHubDispatcher<TestHub>(
                serviceScopeFactory,
                new HubContext<TestHub>(new DefaultHubLifetimeManager<TestHub>(NullLogger<DefaultHubLifetimeManager<TestHub>>.Instance)),
                Options.Create(new HubOptions<TestHub>()),
                Options.Create(new HubOptions()),
                new Logger<DefaultHubDispatcher<TestHub>>(NullLoggerFactory.Instance));

            var pair = DuplexPipe.CreateConnectionPair(PipeOptions.Default, PipeOptions.Default);
            var connection = new DefaultConnectionContext(Guid.NewGuid().ToString(), pair.Application, pair.Transport);

            _connectionContext = new NoErrorHubConnectionContext(connection, TimeSpan.Zero, NullLoggerFactory.Instance);

            _connectionContext.Protocol = new FakeHubProtocol();
        }

        public class FakeHubProtocol : IHubProtocol
        {
            public string Name { get; }
            public int Version => 1;
            public int MinorVersion => 0;
            public TransferFormat TransferFormat { get; }

            public bool IsVersionSupported(int version)
            {
                return true;
            }

            public bool TryParseMessage(ref ReadOnlySequence<byte> input, IInvocationBinder binder, out HubMessage message)
            {
                message = null;
                return false;
            }

            public void WriteMessage(HubMessage message, IBufferWriter<byte> output)
            {
            }

            public ReadOnlyMemory<byte> GetMessageBytes(HubMessage message)
            {
                return HubProtocolExtensions.GetMessageBytes(this, message);
            }
        }

        public class NoErrorHubConnectionContext : HubConnectionContext
        {
            public TaskCompletionSource<object> ReceivedCompleted = new TaskCompletionSource<object>();

            public NoErrorHubConnectionContext(ConnectionContext connectionContext, TimeSpan keepAliveInterval, ILoggerFactory loggerFactory) : base(connectionContext, keepAliveInterval, loggerFactory)
            {
            }

            public override ValueTask WriteAsync(HubMessage message, CancellationToken cancellationToken)
            {
                if (message is CompletionMessage completionMessage)
                {
                    ReceivedCompleted.TrySetResult(null);

                    if (!string.IsNullOrEmpty(completionMessage.Error))
                    {
                        throw new Exception("Error invoking hub method: " + completionMessage.Error);
                    }
                }

                return default;
            }
        }

        public class TestHub : Hub
        {
            public void Invocation()
            {
            }

            public Task InvocationAsync()
            {
                return Task.CompletedTask;
            }

            public int InvocationReturnValue()
            {
                return 1;
            }

            public Task<int> InvocationReturnAsync()
            {
                return Task.FromResult(1);
            }

            public ValueTask<int> InvocationValueTaskAsync()
            {
                return new ValueTask<int>(1);
            }

            public ChannelReader<int> StreamChannelReader()
            {
                var channel = Channel.CreateUnbounded<int>();
                channel.Writer.Complete();

                return channel;
            }

            public Task<ChannelReader<int>> StreamChannelReaderAsync()
            {
                var channel = Channel.CreateUnbounded<int>();
                channel.Writer.Complete();

                return Task.FromResult<ChannelReader<int>>(channel);
            }

            public ValueTask<ChannelReader<int>> StreamChannelReaderValueTaskAsync()
            {
                var channel = Channel.CreateUnbounded<int>();
                channel.Writer.Complete();

                return new ValueTask<ChannelReader<int>>(channel);
            }

            public ChannelReader<int> StreamChannelReaderCount(int count)
            {
                var channel = Channel.CreateUnbounded<int>();

                _ = Task.Run(async () =>
                {
                    for (var i = 0; i < count; i++)
                    {
                        await channel.Writer.WriteAsync(i);
                    }
                    channel.Writer.Complete();
                });

                return channel.Reader;
            }

            public async Task UploadStream(ChannelReader<string> channelReader)
            {
                while (await channelReader.WaitToReadAsync())
                {
                    while (channelReader.TryRead(out var item))
                    {
                    }
                }
            }
        }

        [Benchmark]
        public Task Invocation()
        {
           return _dispatcher.DispatchMessageAsync(_connectionContext, new InvocationMessage("123", "Invocation", Array.Empty<object>()));
        }

        [Benchmark]
        public Task InvocationAsync()
        {
           return _dispatcher.DispatchMessageAsync(_connectionContext, new InvocationMessage("123", "InvocationAsync", Array.Empty<object>()));
        }

        [Benchmark]
        public Task InvocationReturnValue()
        {
           return _dispatcher.DispatchMessageAsync(_connectionContext, new InvocationMessage("123", "InvocationReturnValue", Array.Empty<object>()));
        }

        [Benchmark]
        public Task InvocationReturnAsync()
        {
           return _dispatcher.DispatchMessageAsync(_connectionContext, new InvocationMessage("123", "InvocationReturnAsync", Array.Empty<object>()));
        }

        [Benchmark]
        public Task InvocationValueTaskAsync()
        {
           return _dispatcher.DispatchMessageAsync(_connectionContext, new InvocationMessage("123", "InvocationValueTaskAsync", Array.Empty<object>()));
        }

        [Benchmark]
        public Task StreamChannelReader()
        {
           return _dispatcher.DispatchMessageAsync(_connectionContext, new StreamInvocationMessage("123", "StreamChannelReader", Array.Empty<object>()));
        }

        [Benchmark]
        public Task StreamChannelReaderAsync()
        {
           return _dispatcher.DispatchMessageAsync(_connectionContext, new StreamInvocationMessage("123", "StreamChannelReaderAsync", Array.Empty<object>()));
        }

        [Benchmark]
        public Task StreamChannelReaderValueTaskAsync()
        {
           return _dispatcher.DispatchMessageAsync(_connectionContext, new StreamInvocationMessage("123", "StreamChannelReaderValueTaskAsync", Array.Empty<object>()));
        }

        [Benchmark]
        public async Task StreamChannelReaderCount_Zero()
        {
           await _dispatcher.DispatchMessageAsync(_connectionContext, new StreamInvocationMessage("123", "StreamChannelReaderCount", new object[] { 0 }));

           await (_connectionContext as NoErrorHubConnectionContext).ReceivedCompleted.Task;
           (_connectionContext as NoErrorHubConnectionContext).ReceivedCompleted = new TaskCompletionSource<object>();
        }

        [Benchmark]
        public async Task StreamChannelReaderCount_One()
        {
            await _dispatcher.DispatchMessageAsync(_connectionContext, new StreamInvocationMessage("123", "StreamChannelReaderCount", new object[] { 1 }));

            await (_connectionContext as NoErrorHubConnectionContext).ReceivedCompleted.Task;
           (_connectionContext as NoErrorHubConnectionContext).ReceivedCompleted = new TaskCompletionSource<object>();
        }

        [Benchmark]
        public async Task StreamChannelReaderCount_Thousand()
        {
            await _dispatcher.DispatchMessageAsync(_connectionContext, new StreamInvocationMessage("123", "StreamChannelReaderCount", new object[] { 1000 }));

            await (_connectionContext as NoErrorHubConnectionContext).ReceivedCompleted.Task;
           (_connectionContext as NoErrorHubConnectionContext).ReceivedCompleted = new TaskCompletionSource<object>();
        }

        [Benchmark]
        public async Task UploadStream_One()
        {
           await _dispatcher.DispatchMessageAsync(_connectionContext, new InvocationMessage("123", nameof(TestHub.UploadStream), Array.Empty<object>(), streamIds: new string[] { "1" }));
           await _dispatcher.DispatchMessageAsync(_connectionContext, new StreamItemMessage("1", "test"));
           await _dispatcher.DispatchMessageAsync(_connectionContext, CompletionMessage.Empty("1"));

           await (_connectionContext as NoErrorHubConnectionContext).ReceivedCompleted.Task;
           (_connectionContext as NoErrorHubConnectionContext).ReceivedCompleted = new TaskCompletionSource<object>();
        }

        [Benchmark]
        public async Task UploadStream_Thousand()
        {
           await _dispatcher.DispatchMessageAsync(_connectionContext, new InvocationMessage("123", nameof(TestHub.UploadStream), Array.Empty<object>(), streamIds: new string[] { "1" }));
           for (var i = 0; i < 1000; ++i)
           {
               await _dispatcher.DispatchMessageAsync(_connectionContext, new StreamItemMessage("1", "test"));
           }
           await _dispatcher.DispatchMessageAsync(_connectionContext, CompletionMessage.Empty("1"));

           await (_connectionContext as NoErrorHubConnectionContext).ReceivedCompleted.Task;
           (_connectionContext as NoErrorHubConnectionContext).ReceivedCompleted = new TaskCompletionSource<object>();
        }
    }
}
