// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http
{
    internal sealed partial class HttpResponseHeaders : HttpHeaders
    {
        private static ReadOnlySpan<byte> _CrLf => new[] { (byte)'\r', (byte)'\n' };
        private static ReadOnlySpan<byte> _colonSpace => new[] { (byte)':', (byte)' ' };

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        protected override IEnumerator<KeyValuePair<string, StringValues>> GetEnumeratorFast()
        {
            return GetEnumerator();
        }

        internal void CopyTo(ref BufferWriter<PipeWriter> buffer)
        {
            CopyToFast(ref buffer);
            if (MaybeUnknown != null)
            {
                foreach (var kv in MaybeUnknown)
                {
                    foreach (var value in kv.Value)
                    {
                        if (value != null)
                        {
                            buffer.Write(_CrLf);
                            buffer.WriteAsciiNoValidation(kv.Key);
                            buffer.Write(_colonSpace);
                            buffer.WriteAsciiNoValidation(value);
                        }
                    }
                }
            }
        }

        private static long ParseContentLength(string value)
        {
            if (!HeaderUtilities.TryParseNonNegativeInt64(value, out var parsed))
            {
                ThrowInvalidContentLengthException(value);
            }

            return parsed;
        }

        private static void ThrowInvalidContentLengthException(string value)
        {
            throw new InvalidOperationException(CoreStrings.FormatInvalidContentLength_InvalidNumber(value));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void SetValueUnknown(string key, in StringValues value)
        {
            ValidateHeaderNameCharacters(key);
            Unknown[key] = value;
        }

        public partial struct Enumerator : IEnumerator<KeyValuePair<string, StringValues>>
        {
            private readonly HttpResponseHeaders _collection;
            private readonly long _bits;
            private int _next;
            private KeyValuePair<string, StringValues> _current;
            private readonly bool _hasUnknown;
            private Dictionary<string, StringValues>.Enumerator _unknownEnumerator;

            internal Enumerator(HttpResponseHeaders collection)
            {
                _collection = collection;
                _bits = collection._bits;
                _next = 0;
                _current = default;
                _hasUnknown = collection.MaybeUnknown != null;
                _unknownEnumerator = _hasUnknown
                    ? collection.MaybeUnknown.GetEnumerator()
                    : default;
            }

            public KeyValuePair<string, StringValues> Current => _current;

            object IEnumerator.Current => _current;

            public void Dispose()
            {
            }

            public void Reset()
            {
                _next = 0;
            }
        }

    }
}
