﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.DataProtection.Test
{
    public class HostingTests
    {
        [Fact]
        public async Task LoadsKeyRingBeforeServerStarts()
        {
            var tcs = new TaskCompletionSource<object>();
            var mockKeyRing = new Mock<IKeyRingProvider>();
            mockKeyRing.Setup(m => m.GetCurrentKeyRing())
                .Returns(Mock.Of<IKeyRing>())
                .Callback(() => tcs.TrySetResult(null));

            var builder = new WebHostBuilder()
                .UseStartup<TestStartup>()
                .ConfigureServices(s =>
                    s.AddDataProtection()
                    .Services
                    .Replace(ServiceDescriptor.Singleton(mockKeyRing.Object))
                    .AddSingleton<IServer>(
                        new FakeServer(onStart: () => tcs.TrySetException(new InvalidOperationException("Server was started before key ring was initialized")))));

            using (var host = builder.Build())
            {
                await host.StartAsync();
            }

            await tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(10));
            mockKeyRing.VerifyAll();
        }

        [Fact]
        public async Task StartupContinuesOnFailureToLoadKey()
        {
            var mockKeyRing = new Mock<IKeyRingProvider>();
            mockKeyRing.Setup(m => m.GetCurrentKeyRing())
                .Throws(new NotSupportedException("This mock doesn't actually work, but shouldn't kill the server"))
                .Verifiable();

            var builder = new WebHostBuilder()
                .UseStartup<TestStartup>()
                .ConfigureServices(s =>
                    s.AddDataProtection()
                    .Services
                    .Replace(ServiceDescriptor.Singleton(mockKeyRing.Object))
                    .AddSingleton(Mock.Of<IServer>()));

            using (var host = builder.Build())
            {
                await host.StartAsync();
            }
            
            mockKeyRing.VerifyAll();
        }

        private class TestStartup
        {
            public void Configure(IApplicationBuilder app)
            {
            }
        }

        public class FakeServer : IServer
        {
            private readonly Action _onStart;

            public FakeServer(Action onStart)
            {
                _onStart = onStart;
            }

            public IFeatureCollection Features => new FeatureCollection();

            public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
            {
                _onStart();
                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

            public void Dispose()
            {
            }
        }
    }
}
