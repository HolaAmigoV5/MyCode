// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.DataProtection.KeyManagement
{
    internal static class KeyEscrowServiceProviderExtensions
    {
        /// <summary>
        /// Gets an aggregate <see cref="IKeyEscrowSink"/> from the underlying <see cref="IServiceProvider"/>.
        /// This method may return null if no sinks are registered.
        /// </summary>
        public static IKeyEscrowSink GetKeyEscrowSink(this IServiceProvider services)
        {
            var escrowSinks = services?.GetService<IEnumerable<IKeyEscrowSink>>()?.ToList();
            return (escrowSinks != null && escrowSinks.Count > 0) ? new AggregateKeyEscrowSink(escrowSinks) : null;
        }

        private sealed class AggregateKeyEscrowSink : IKeyEscrowSink
        {
            private readonly List<IKeyEscrowSink> _sinks;

            public AggregateKeyEscrowSink(List<IKeyEscrowSink> sinks)
            {
                _sinks = sinks;
            }

            public void Store(Guid keyId, XElement element)
            {
                foreach (var sink in _sinks)
                {
                    sink.Store(keyId, element);
                }
            }
        }
    }
}
