// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http
{
    /// <summary>
    /// The upgrade stream uses the raw connection stream instead of going through the RequestBodyPipe. This
    /// removes the redundant copy from the transport pipe to the body pipe.
    /// </summary>
    internal class Http1UpgradeMessageBody : Http1MessageBody
    {
        public bool _completed;
        public Http1UpgradeMessageBody(Http1Connection context)
            : base(context)
        {
            RequestUpgrade = true;
        }

        // This returns IsEmpty so we can avoid draining the body (since it's basically an endless stream)
        public override bool IsEmpty => true;

        public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
        {
            if (_completed)
            {
                throw new InvalidOperationException("Reading is not allowed after the reader was completed.");
            }
            return _context.Input.ReadAsync(cancellationToken);
        }

        public override bool TryRead(out ReadResult result)
        {
            if (_completed)
            {
                throw new InvalidOperationException("Reading is not allowed after the reader was completed.");
            }
            return _context.Input.TryRead(out result);
        }

        public override void AdvanceTo(SequencePosition consumed)
        {
            _context.Input.AdvanceTo(consumed);
        }

        public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
        {
            _context.Input.AdvanceTo(consumed, examined);
        }

        public override void Complete(Exception exception)
        {
            // Don't call Connection.Complete.
            _context.ReportApplicationError(exception);
            _completed = true;
        }

        public override void CancelPendingRead()
        {
            _context.Input.CancelPendingRead();
        }

        public override void OnWriterCompleted(Action<Exception, object> callback, object state)
        {
            _context.Input.OnWriterCompleted(callback, state);
        }

        public override Task ConsumeAsync()
        {
            return Task.CompletedTask;
        }

        public override Task StopAsync()
        {
            return Task.CompletedTask;
        }
    }
}
