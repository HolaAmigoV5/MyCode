// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;

namespace Microsoft.AspNetCore.Server.IISIntegration.FunctionalTests
{
    [SkipIfHostableWebCoreNotAvailable]
    [OSSkipCondition(OperatingSystems.Windows, WindowsVersions.Win7, "https://github.com/aspnet/IISIntegration/issues/866")]
    public class ResponseAbortTests : StrictTestServerTests
    {
        [ConditionalFact]
        public async Task ClosesWithoutSendingAnything()
        {
            using (var testServer = await TestServer.Create(
                ctx => {
                    ctx.Abort();
                    return Task.CompletedTask;
                }, LoggerFactory))
            {
                using (var connection = testServer.CreateConnection())
                {
                    await SendContentLength1Post(connection);
                    await connection.WaitForConnectionClose();
                }
            }
        }

        [ConditionalFact]
        public async Task ClosesAfterDataSent()
        {
            var bodyReceived = CreateTaskCompletionSource();
            using (var testServer = await TestServer.Create(
                async ctx => {
                    await ctx.Response.WriteAsync("Abort");
                    await ctx.Response.Body.FlushAsync();
                    await bodyReceived.Task.DefaultTimeout();
                    ctx.Abort();
                }, LoggerFactory))
            {
                using (var connection = testServer.CreateConnection())
                {
                    await SendContentLength1Post(connection);
                    await connection.Receive(
                        "HTTP/1.1 200 OK",
                        "");
                    await connection.ReceiveHeaders(
                        "Transfer-Encoding: chunked");

                    await connection.ReceiveChunk("Abort");
                    bodyReceived.SetResult(true);
                    await connection.WaitForConnectionClose();
                }
            }
        }

        [ConditionalFact]
        public async Task ReadsThrowAfterAbort()
        {
            Exception exception = null;

            using (var testServer = await TestServer.Create(
                async ctx => {
                    ctx.Abort();
                    try
                    {
                        var a = new byte[10];
                        await ctx.Request.Body.ReadAsync(a);
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                }, LoggerFactory))
            {
                using (var connection = testServer.CreateConnection())
                {
                    await SendContentLength1Post(connection);
                    await connection.WaitForConnectionClose();
                }
            }

            Assert.IsType<ConnectionAbortedException>(exception);
        }

        [ConditionalFact]
        public async Task WritesNoopAfterAbort()
        {
            Exception exception = null;

            using (var testServer = await TestServer.Create(
                async ctx => {
                    ctx.Abort();
                    try
                    {
                        await ctx.Response.Body.WriteAsync(new byte[10]);
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                }, LoggerFactory))
            {
                using (var connection = testServer.CreateConnection())
                {
                    await SendContentLength1Post(connection);
                    await connection.WaitForConnectionClose();
                }
            }

            Assert.Null(exception);
        }

        [ConditionalFact]
        public async Task RequestAbortedIsTrippedAfterAbort()
        {
            bool tokenAborted = false;
            using (var testServer = await TestServer.Create(
                ctx => {
                    ctx.Abort();
                    tokenAborted = ctx.RequestAborted.IsCancellationRequested;
                    return Task.CompletedTask;
                }, LoggerFactory))
            {
                using (var connection = testServer.CreateConnection())
                {
                    await SendContentLength1Post(connection);
                    await connection.WaitForConnectionClose();
                }
            }

            Assert.True(tokenAborted);
        }

        private static async Task SendContentLength1Post(TestConnection connection)
        {
            await connection.Send(
                "POST / HTTP/1.1",
                "Content-Length: 1",
                "Host: localhost",
                "",
                "");
        }
    }
}
