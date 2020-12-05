// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Xunit;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Tests
{
    public class HttpRequestPipeReaderTests
    {
        [Fact]
        public async Task StopAcceptingReadsCausesReadToThrowObjectDisposedException()
        {
            var pipeReader = new HttpRequestPipeReader();
            pipeReader.StartAcceptingReads(null);
            pipeReader.StopAcceptingReads();

            // Validation for ReadAsync occurs in an async method in ReadOnlyPipeStream.
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => { await pipeReader.ReadAsync(); });
        }
        [Fact]
        public async Task AbortCausesReadToCancel()
        {
            var pipeReader = new HttpRequestPipeReader();

            pipeReader.StartAcceptingReads(null);
            pipeReader.Abort();
            await Assert.ThrowsAsync<TaskCanceledException>(() => pipeReader.ReadAsync().AsTask());
        }

        [Fact]
        public async Task AbortWithErrorCausesReadToCancel()
        {
            var pipeReader = new HttpRequestPipeReader();

            pipeReader.StartAcceptingReads(null);
            var error = new Exception();
            pipeReader.Abort(error);
            var exception = await Assert.ThrowsAsync<Exception>(() => pipeReader.ReadAsync().AsTask());
            Assert.Same(error, exception);
        }
    }
}
