// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Internal;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.WebUtilities
{
    /// <summary>
    /// Used to read an 'application/x-www-form-urlencoded' form.
    /// Internally reads from a PipeReader.
    /// </summary>
    public class FormPipeReader
    {
        private const int StackAllocThreshold = 128;
        private const int DefaultValueCountLimit = 1024;
        private const int DefaultKeyLengthLimit = 1024 * 2;
        private const int DefaultValueLengthLimit = 1024 * 1024 * 4;

        // Used for UTF8/ASCII (precalculated for fast path)
        private static ReadOnlySpan<byte> UTF8EqualEncoded => new byte[] { (byte)'=' };
        private static ReadOnlySpan<byte> UTF8AndEncoded => new byte[] { (byte)'&' };

        // Used for other encodings
        private byte[] _otherEqualEncoding;
        private byte[] _otherAndEncoding;

        private readonly PipeReader _pipeReader;
        private readonly Encoding _encoding;

        public FormPipeReader(PipeReader pipeReader)
            : this(pipeReader, Encoding.UTF8)
        {
        }

        public FormPipeReader(PipeReader pipeReader, Encoding encoding)
        {
            if (encoding == Encoding.UTF7)
            {
                throw new ArgumentException("UTF7 is unsupported and insecure. Please select a different encoding.");
            }

            _pipeReader = pipeReader;
            _encoding = encoding;

            if (_encoding != Encoding.UTF8 && _encoding != Encoding.ASCII)
            {
                _otherEqualEncoding = _encoding.GetBytes("=");
                _otherAndEncoding = _encoding.GetBytes("&");
            }
        }

        /// <summary>
        /// The limit on the number of form values to allow in ReadForm or ReadFormAsync.
        /// </summary>
        public int ValueCountLimit { get; set; } = DefaultValueCountLimit;

        /// <summary>
        /// The limit on the length of form keys.
        /// </summary>
        public int KeyLengthLimit { get; set; } = DefaultKeyLengthLimit;

        /// <summary>
        /// The limit on the length of form values.
        /// </summary>
        public int ValueLengthLimit { get; set; } = DefaultValueLengthLimit;

        /// <summary>
        /// Parses an HTTP form body.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The collection containing the parsed HTTP form body.</returns>
        public async Task<Dictionary<string, StringValues>> ReadFormAsync(CancellationToken cancellationToken = default)
        {
            KeyValueAccumulator accumulator = default;
            while (true)
            {
                var readResult = await _pipeReader.ReadAsync(cancellationToken);

                var buffer = readResult.Buffer;

                if (!buffer.IsEmpty)
                {
                    ParseFormValues(ref buffer, ref accumulator, readResult.IsCompleted);
                }

                if (readResult.IsCompleted)
                {
                    _pipeReader.AdvanceTo(buffer.End);

                    if (!buffer.IsEmpty)
                    {
                        throw new InvalidOperationException("End of body before form was fully parsed.");
                    }
                    break;
                }

                _pipeReader.AdvanceTo(buffer.Start, buffer.End);
            }

            return accumulator.GetResults();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void ParseFormValues(
            ref ReadOnlySequence<byte> buffer,
            ref KeyValueAccumulator accumulator,
            bool isFinalBlock)
        {
            if (buffer.IsSingleSegment)
            {
                ParseFormValuesFast(buffer.First.Span,
                    ref accumulator,
                    isFinalBlock,
                    out var consumed);

                buffer = buffer.Slice(consumed);
                return;
            }

            ParseValuesSlow(ref buffer,
                ref accumulator,
                isFinalBlock);
        }

        // Fast parsing for single span in ReadOnlySequence
        private void ParseFormValuesFast(ReadOnlySpan<byte> span,
            ref KeyValueAccumulator accumulator,
            bool isFinalBlock,
            out int consumed)
        {
            ReadOnlySpan<byte> key = default;
            ReadOnlySpan<byte> value = default;
            consumed = 0;
            var equalsDelimiter = GetEqualsForEncoding();
            var andDelimiter = GetAndForEncoding();

            while (span.Length > 0)
            {
                var equals = span.IndexOf(equalsDelimiter);

                if (equals == -1)
                {
                    if (span.Length > KeyLengthLimit)
                    {
                        ThrowKeyTooLargeException();
                    }
                    break;
                }

                if (equals > KeyLengthLimit)
                {
                    ThrowKeyTooLargeException();
                }

                key = span.Slice(0, equals);

                span = span.Slice(key.Length + equalsDelimiter.Length);
                value = span;

                var ampersand = span.IndexOf(andDelimiter);

                if (ampersand == -1)
                {
                    if (span.Length > ValueLengthLimit)
                    {
                        ThrowValueTooLargeException();
                        return;
                    }

                    if (!isFinalBlock)
                    {
                        // We can't know that what is currently read is the end of the form value, that's only the case if this is the final block
                        // If we're not in the final block, then consume nothing
                        break;
                    }

                    // If we are on the final block, the remaining content in value is what we want to add to the KVAccumulator.
                    // Clear out the remaining span such that the loop will exit.
                    span = Span<byte>.Empty;
                }
                else
                {
                    if (ampersand > ValueLengthLimit)
                    {
                        ThrowValueTooLargeException();
                    }

                    value = span.Slice(0, ampersand);
                    span = span.Slice(ampersand + andDelimiter.Length);
                }

                var decodedKey = GetDecodedString(key);
                var decodedValue = GetDecodedString(value);

                AppendAndVerify(ref accumulator, decodedKey, decodedValue);

                // Cover case where we don't have an ampersand at the end.
                consumed += key.Length + value.Length + (ampersand == -1 ? equalsDelimiter.Length : equalsDelimiter.Length + andDelimiter.Length);
            }
        }

        // For multi-segment parsing of a read only sequence
        private void ParseValuesSlow(
            ref ReadOnlySequence<byte> buffer,
            ref KeyValueAccumulator accumulator,
            bool isFinalBlock)
        {
            var sequenceReader = new SequenceReader<byte>(buffer);
            var consumed = sequenceReader.Position;
            var equalsDelimiter = GetEqualsForEncoding();
            var andDelimiter = GetAndForEncoding();

            while (!sequenceReader.End)
            {
                // TODO seems there is a bug with TryReadTo (advancePastDelimiter: true). It isn't advancing past the delimiter on second read.
                if (!sequenceReader.TryReadTo(out ReadOnlySequence<byte> key, equalsDelimiter, advancePastDelimiter: false) ||
                    !sequenceReader.IsNext(equalsDelimiter, true))
                {
                    if (sequenceReader.Consumed > KeyLengthLimit)
                    {
                        ThrowKeyTooLargeException();
                    }

                    break;
                }

                if (key.Length > KeyLengthLimit)
                {
                    ThrowKeyTooLargeException();
                }

                if (!sequenceReader.TryReadTo(out ReadOnlySequence<byte> value, andDelimiter, false) ||
                    !sequenceReader.IsNext(andDelimiter, true))
                {
                    if (!isFinalBlock)
                    {
                        if (sequenceReader.Consumed - key.Length > ValueLengthLimit)
                        {
                            ThrowValueTooLargeException();
                        }
                        break;
                    }

                    value = buffer.Slice(sequenceReader.Position);

                    sequenceReader.Advance(value.Length);
                }

                if (value.Length > ValueLengthLimit)
                {
                    ThrowValueTooLargeException();
                }

                // Need to call ToArray if the key/value spans multiple segments 
                var decodedKey = GetDecodedStringFromReadOnlySequence(key);
                var decodedValue = GetDecodedStringFromReadOnlySequence(value);

                AppendAndVerify(ref accumulator, decodedKey, decodedValue);

                consumed = sequenceReader.Position;
            }

            buffer = buffer.Slice(consumed);
        }

        private void ThrowKeyTooLargeException()
        {
            throw new InvalidDataException($"Form key length limit {KeyLengthLimit} exceeded.");
        }

        private void ThrowValueTooLargeException()
        {
            throw new InvalidDataException($"Form value length limit {ValueLengthLimit} exceeded.");
        }

        private string GetDecodedStringFromReadOnlySequence(ReadOnlySequence<byte> ros)
        {
            if (ros.IsSingleSegment)
            {
                return GetDecodedString(ros.First.Span);
            }

            if (ros.Length < StackAllocThreshold)
            {
                Span<byte> buffer = stackalloc byte[(int)ros.Length];
                ros.CopyTo(buffer);
                return GetDecodedString(buffer);
            }
            else
            {
                var byteArray = ArrayPool<byte>.Shared.Rent((int)ros.Length);

                try
                {
                    Span<byte> buffer = byteArray.AsSpan(0, (int)ros.Length);
                    ros.CopyTo(buffer);
                    return GetDecodedString(buffer);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(byteArray);
                }
            }
        }

        // Check that key/value constraints are met and appends value to accumulator.
        private void AppendAndVerify(ref KeyValueAccumulator accumulator, string decodedKey, string decodedValue)
        {
            accumulator.Append(decodedKey, decodedValue);

            if (accumulator.ValueCount > ValueCountLimit)
            {
                throw new InvalidDataException($"Form value count limit {ValueCountLimit} exceeded.");
            }
        }

        private string GetDecodedString(ReadOnlySpan<byte> readOnlySpan)
        {
            if (readOnlySpan.Length == 0)
            {
                return string.Empty;
            }
            else if (_encoding == Encoding.UTF8 || _encoding == Encoding.ASCII)
            {
                // UrlDecoder only works on UTF8 (and implicitly ASCII)

                // We need to create a Span from a ReadOnlySpan. This cast is safe because the memory is still held by the pipe
                // We will also create a string from it by the end of the function.
                var span = MemoryMarshal.CreateSpan(ref Unsafe.AsRef(readOnlySpan[0]), readOnlySpan.Length);

                var bytes = UrlDecoder.DecodeInPlace(span, isFormEncoding: true);
                span = span.Slice(0, bytes);

                return _encoding.GetString(span);
            }
            else
            {
                // Slow path for Unicode and other encodings.
                // Just do raw string replacement.
                var decodedString = _encoding.GetString(readOnlySpan);
                decodedString = decodedString.Replace('+', ' ');
                return Uri.UnescapeDataString(decodedString);
            }
        }

        private ReadOnlySpan<byte> GetEqualsForEncoding()
        {
            if (_encoding == Encoding.UTF8 || _encoding == Encoding.ASCII)
            {
                return UTF8EqualEncoded;
            }
            else
            {
                return _otherEqualEncoding;
            }
        }

        private ReadOnlySpan<byte> GetAndForEncoding()
        {
            if (_encoding == Encoding.UTF8 || _encoding == Encoding.ASCII)
            {
                return UTF8AndEncoded;
            }
            else
            {
                return _otherAndEncoding;
            }
        }
    }
}
