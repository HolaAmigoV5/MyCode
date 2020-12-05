// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Microsoft.AspNetCore.WebUtilities.Test
{
    public class FormPipeReaderTests
    {
        [Fact]
        public async Task ReadFormAsync_EmptyKeyAtEndAllowed()
        {
            var bodyPipe = await MakePipeReader("=bar");

            var formCollection = await ReadFormAsync(new FormPipeReader(bodyPipe));

            Assert.Equal("bar", formCollection[""].ToString());
        }

        [Fact]
        public async Task ReadFormAsync_EmptyKeyWithAdditionalEntryAllowed()
        {
            var bodyPipe = await MakePipeReader("=bar&baz=2");

            var formCollection = await ReadFormAsync(new FormPipeReader(bodyPipe));

            Assert.Equal("bar", formCollection[""].ToString());
            Assert.Equal("2", formCollection["baz"].ToString());
        }

        [Fact]
        public async Task ReadFormAsync_EmptyValuedAtEndAllowed()
        {
            var bodyPipe = await MakePipeReader("foo=");

            var formCollection = await ReadFormAsync(new FormPipeReader(bodyPipe));

            Assert.Equal("", formCollection["foo"].ToString());
        }

        [Fact]
        public async Task ReadFormAsync_EmptyValuedWithAdditionalEntryAllowed()
        {
            var bodyPipe = await MakePipeReader("foo=&baz=2");

            var formCollection = await ReadFormAsync(new FormPipeReader(bodyPipe));

            Assert.Equal("", formCollection["foo"].ToString());
            Assert.Equal("2", formCollection["baz"].ToString());
        }

        [Fact]
        public async Task ReadFormAsync_ValueCountLimitMet_Success()
        {
            var bodyPipe = await MakePipeReader("foo=1&bar=2&baz=3");

            var formCollection = await ReadFormAsync(new FormPipeReader(bodyPipe) { ValueCountLimit = 3 });

            Assert.Equal("1", formCollection["foo"].ToString());
            Assert.Equal("2", formCollection["bar"].ToString());
            Assert.Equal("3", formCollection["baz"].ToString());
            Assert.Equal(3, formCollection.Count);
        }

        [Fact]
        public async Task ReadFormAsync_ValueCountLimitExceeded_Throw()
        {
            var bodyPipe = await MakePipeReader("foo=1&baz=2&bar=3&baz=4&baf=5");

            var exception = await Assert.ThrowsAsync<InvalidDataException>(
                () => ReadFormAsync(new FormPipeReader(bodyPipe) { ValueCountLimit = 3 }));
            Assert.Equal("Form value count limit 3 exceeded.", exception.Message);
        }

        [Fact]
        public async Task ReadFormAsync_ValueCountLimitExceededSameKey_Throw()
        {
            var bodyPipe = await MakePipeReader("baz=1&baz=2&baz=3&baz=4");

            var exception = await Assert.ThrowsAsync<InvalidDataException>(
                () => ReadFormAsync(new FormPipeReader(bodyPipe) { ValueCountLimit = 3 }));
            Assert.Equal("Form value count limit 3 exceeded.", exception.Message);
        }

        [Fact]
        public async Task ReadFormAsync_KeyLengthLimitMet_Success()
        {
            var bodyPipe = await MakePipeReader("fooooooooo=1&bar=2&baz=3&baz=4");

            var formCollection = await ReadFormAsync(new FormPipeReader(bodyPipe) { KeyLengthLimit = 10 });

            Assert.Equal("1", formCollection["fooooooooo"].ToString());
            Assert.Equal("2", formCollection["bar"].ToString());
            Assert.Equal("3,4", formCollection["baz"].ToString());
            Assert.Equal(3, formCollection.Count);
        }

        [Fact]
        public async Task ReadFormAsync_KeyLengthLimitExceeded_Throw()
        {
            var bodyPipe = await MakePipeReader("foo=1&baz12345678=2");

            var exception = await Assert.ThrowsAsync<InvalidDataException>(
                () => ReadFormAsync(new FormPipeReader(bodyPipe) { KeyLengthLimit = 10 }));
            Assert.Equal("Form key length limit 10 exceeded.", exception.Message);
        }

        [Fact]
        public async Task ReadFormAsync_ValueLengthLimitMet_Success()
        {
            var bodyPipe = await MakePipeReader("foo=1&bar=1234567890&baz=3&baz=4");

            var formCollection = await ReadFormAsync(new FormPipeReader(bodyPipe) { ValueLengthLimit = 10 });

            Assert.Equal("1", formCollection["foo"].ToString());
            Assert.Equal("1234567890", formCollection["bar"].ToString());
            Assert.Equal("3,4", formCollection["baz"].ToString());
            Assert.Equal(3, formCollection.Count);
        }

        [Fact]
        public async Task ReadFormAsync_ValueLengthLimitExceeded_Throw()
        {
            var bodyPipe = await MakePipeReader("foo=1&baz=12345678901");

            var exception = await Assert.ThrowsAsync<InvalidDataException>(
                () => ReadFormAsync(new FormPipeReader(bodyPipe) { ValueLengthLimit = 10 }));
            Assert.Equal("Form value length limit 10 exceeded.", exception.Message);
        }

        // https://en.wikipedia.org/wiki/Percent-encoding
        [Theory]
        [InlineData("++=hello", "  ", "hello")]
        [InlineData("a=1+1", "a", "1 1")]
        [InlineData("%22%25%2D%2E%3C%3E%5C%5E%5F%60%7B%7C%7D%7E=%22%25%2D%2E%3C%3E%5C%5E%5F%60%7B%7C%7D%7E", "\"%-.<>\\^_`{|}~", "\"%-.<>\\^_`{|}~")]
        [InlineData("a=%41", "a", "A")] // ascii encoded hex
        [InlineData("a=%C3%A1", "a", "\u00e1")] // utf8 code points
        [InlineData("a=%u20AC", "a", "%u20AC")] // utf16 not supported
        public async Task ReadForm_Decoding(string formData, string key, string expectedValue)
        {
            var bodyPipe = await MakePipeReader(text: formData);

            var form = await ReadFormAsync(new FormPipeReader(bodyPipe));

            Assert.Equal(expectedValue, form[key]);
        }

        public static TheoryData<Encoding> Encodings =>
                 new TheoryData<Encoding>
                 {
                     { Encoding.UTF8 },
                     { Encoding.UTF32 },
                     { Encoding.ASCII },
                     { Encoding.Unicode }
                 };

        [Theory]
        [MemberData(nameof(Encodings))]
        public void TryParseFormValues_SingleSegmentWorks(Encoding encoding)
        {
            var readOnlySequence = new ReadOnlySequence<byte>(encoding.GetBytes("foo=bar&baz=boo"));

            KeyValueAccumulator accumulator = default;
            var formReader = new FormPipeReader(null, encoding);

            formReader.ParseFormValues(ref readOnlySequence, ref accumulator, isFinalBlock: true);

            Assert.Equal(2, accumulator.KeyCount);
            var dict = accumulator.GetResults();
            Assert.Equal("bar", dict["foo"]);
            Assert.Equal("boo", dict["baz"]);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public void TryParseFormValues_MultiSegmentWorks(Encoding encoding)
        {
            var readOnlySequence = ReadOnlySequenceFactory.SingleSegmentFactory.CreateWithContent(encoding.GetBytes("foo=bar&baz=boo&t="));

            KeyValueAccumulator accumulator = default;

            var formReader = new FormPipeReader(null, encoding);
            formReader.ParseFormValues(ref readOnlySequence, ref accumulator, isFinalBlock: true);

            Assert.Equal(3, accumulator.KeyCount);
            var dict = accumulator.GetResults();
            Assert.Equal("bar", dict["foo"]);
            Assert.Equal("boo", dict["baz"]);
            Assert.Equal("", dict["t"]);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public void TryParseFormValues_MultiSegmentSplitAcrossSegmentsWorks(Encoding encoding)
        {
            var readOnlySequence = ReadOnlySequenceFactory.SingleSegmentFactory.CreateWithContent(encoding.GetBytes("foo=bar&baz=boo&t="));

            KeyValueAccumulator accumulator = default;

            var formReader = new FormPipeReader(null, encoding);
            formReader.ParseFormValues(ref readOnlySequence, ref accumulator, isFinalBlock: true);

            Assert.Equal(3, accumulator.KeyCount);
            var dict = accumulator.GetResults();
            Assert.Equal("bar", dict["foo"]);
            Assert.Equal("boo", dict["baz"]);
            Assert.Equal("", dict["t"]);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public void TryParseFormValues_MultiSegmentWithArrayPoolAcrossSegmentsWorks(Encoding encoding)
        {
            var readOnlySequence = ReadOnlySequenceFactory.SingleSegmentFactory.CreateWithContent(encoding.GetBytes("foo=bar&baz=bo" + new string('a', 128)));

            KeyValueAccumulator accumulator = default;

            var formReader = new FormPipeReader(null, encoding);
            formReader.ParseFormValues(ref readOnlySequence, ref accumulator, isFinalBlock: true);

            Assert.Equal(2, accumulator.KeyCount);
            var dict = accumulator.GetResults();
            Assert.Equal("bar", dict["foo"]);
            Assert.Equal("bo" + new string('a', 128), dict["baz"]);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public void TryParseFormValues_MultiSegmentSplitAcrossSegmentsWithPlusesWorks(Encoding encoding)
        {
            var readOnlySequence = ReadOnlySequenceFactory.SingleSegmentFactory.CreateWithContent(encoding.GetBytes("+++=+++&++++=++++&+="));

            KeyValueAccumulator accumulator = default;

            var formReader = new FormPipeReader(null, encoding);
            formReader.ParseFormValues(ref readOnlySequence, ref accumulator, isFinalBlock: true);

            Assert.Equal(3, accumulator.KeyCount);
            var dict = accumulator.GetResults();
            Assert.Equal("    ", dict["    "]);
            Assert.Equal("   ", dict["   "]);
            Assert.Equal("", dict[" "]);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public void TryParseFormValues_DecodedPlusesWorks(Encoding encoding)
        {
            var readOnlySequence = ReadOnlySequenceFactory.SingleSegmentFactory.CreateWithContent(encoding.GetBytes("++%2B=+++%2B&++++=++++&+="));

            KeyValueAccumulator accumulator = default;

            var formReader = new FormPipeReader(null, encoding);
            formReader.ParseFormValues(ref readOnlySequence, ref accumulator, isFinalBlock: true);

            Assert.Equal(3, accumulator.KeyCount);
            var dict = accumulator.GetResults();
            Assert.Equal("    ", dict["    "]);
            Assert.Equal("   +", dict["  +"]);
            Assert.Equal("", dict[" "]);
        }

        [Theory]
        [MemberData(nameof(Encodings))]
        public void TryParseFormValues_MultiSegmentSplitAcrossSegmentsThatNeedDecodingWorks(Encoding encoding)
        {
            var readOnlySequence = ReadOnlySequenceFactory.SingleSegmentFactory.CreateWithContent(encoding.GetBytes("\"%-.<>\\^_`{|}~=\"%-.<>\\^_`{|}~&\"%-.<>\\^_`{|}=wow"));

            KeyValueAccumulator accumulator = default;

            var formReader = new FormPipeReader(null, encoding);
            formReader.ParseFormValues(ref readOnlySequence, ref accumulator, isFinalBlock: true);

            Assert.Equal(2, accumulator.KeyCount);
            var dict = accumulator.GetResults();
            Assert.Equal("\"%-.<>\\^_`{|}~", dict["\"%-.<>\\^_`{|}~"]);
            Assert.Equal("wow", dict["\"%-.<>\\^_`{|}"]);
        }

        [Fact]
        public void TryParseFormValues_MultiSegmentExceedKeyLengthThrows()
        {
            var readOnlySequence = ReadOnlySequenceFactory.SingleSegmentFactory.CreateWithContent(Encoding.UTF8.GetBytes("foo=bar&baz=boo&t="));

            KeyValueAccumulator accumulator = default;

            var formReader = new FormPipeReader(null);
            formReader.KeyLengthLimit = 2;

            var exception = Assert.Throws<InvalidDataException>(() => formReader.ParseFormValues(ref readOnlySequence, ref accumulator, isFinalBlock: true));
            Assert.Equal("Form key length limit 2 exceeded.", exception.Message);
        }

        [Fact]
        public void TryParseFormValues_MultiSegmentExceedKeyLengthThrowsInSplitSegment()
        {
            var readOnlySequence = ReadOnlySequenceFactory.CreateSegments(Encoding.UTF8.GetBytes("fo=bar&ba"), Encoding.UTF8.GetBytes("z=boo&t="));

            KeyValueAccumulator accumulator = default;

            var formReader = new FormPipeReader(null);
            formReader.KeyLengthLimit = 2;

            var exception = Assert.Throws<InvalidDataException>(() => formReader.ParseFormValues(ref readOnlySequence, ref accumulator, isFinalBlock: true));
            Assert.Equal("Form key length limit 2 exceeded.", exception.Message);
        }

        [Fact]
        public void TryParseFormValues_MultiSegmentExceedValueLengthThrows()
        {
            var readOnlySequence = ReadOnlySequenceFactory.CreateSegments(Encoding.UTF8.GetBytes("foo=bar&baz=bo"), Encoding.UTF8.GetBytes("o&t="));

            KeyValueAccumulator accumulator = default;

            var formReader = new FormPipeReader(null);
            formReader.ValueLengthLimit = 2;

            var exception = Assert.Throws<InvalidDataException>(() => formReader.ParseFormValues(ref readOnlySequence, ref accumulator, isFinalBlock: true));
            Assert.Equal("Form value length limit 2 exceeded.", exception.Message);
        }

        [Fact]
        public void TryParseFormValues_MultiSegmentExceedValueLengthThrowsInSplitSegment()
        {
            var readOnlySequence = ReadOnlySequenceFactory.CreateSegments(Encoding.UTF8.GetBytes("foo=ba&baz=bo"), Encoding.UTF8.GetBytes("o&t="));

            KeyValueAccumulator accumulator = default;

            var formReader = new FormPipeReader(null);
            formReader.ValueLengthLimit = 2;

            var exception = Assert.Throws<InvalidDataException>(() => formReader.ParseFormValues(ref readOnlySequence, ref accumulator, isFinalBlock: true));
            Assert.Equal("Form value length limit 2 exceeded.", exception.Message);
        }

        [Fact]
        public void TryParseFormValues_MultiSegmentExceedKeLengthThrowsInSplitSegmentEnd()
        {
            var readOnlySequence = ReadOnlySequenceFactory.CreateSegments(Encoding.UTF8.GetBytes("foo=ba&baz=bo"), Encoding.UTF8.GetBytes("o&asdfasdfasd="));

            KeyValueAccumulator accumulator = default;

            var formReader = new FormPipeReader(null);
            formReader.KeyLengthLimit = 10;

            var exception = Assert.Throws<InvalidDataException>(() => formReader.ParseFormValues(ref readOnlySequence, ref accumulator, isFinalBlock: true));
            Assert.Equal("Form key length limit 10 exceeded.", exception.Message);
        }

        [Fact]
        public void TryParseFormValues_MultiSegmentExceedValueLengthThrowsInSplitSegmentEnd()
        {
            var readOnlySequence = ReadOnlySequenceFactory.CreateSegments(Encoding.UTF8.GetBytes("foo=ba&baz=bo"), Encoding.UTF8.GetBytes("o&t=asdfasdfasd"));

            KeyValueAccumulator accumulator = default;

            var formReader = new FormPipeReader(null);
            formReader.ValueLengthLimit = 10;

            var exception = Assert.Throws<InvalidDataException>(() => formReader.ParseFormValues(ref readOnlySequence, ref accumulator, isFinalBlock: true));
            Assert.Equal("Form value length limit 10 exceeded.", exception.Message);
        }

        [Fact]
        public async Task ResetPipeWorks()
        {
            // Same test that is in the benchmark
            var pipe = new Pipe();
            var bytes = Encoding.UTF8.GetBytes("foo=bar&baz=boo");

            for (var i = 0; i < 1000; i++)
            {
                pipe.Writer.Write(bytes);
                pipe.Writer.Complete();
                var formReader = new FormPipeReader(pipe.Reader);
                await formReader.ReadFormAsync();
                pipe.Reader.Complete();
                pipe.Reset();
            }
        }

        internal virtual Task<Dictionary<string, StringValues>> ReadFormAsync(FormPipeReader reader)
        {
            return reader.ReadFormAsync();
        }

        private static async Task<PipeReader> MakePipeReader(string text)
        {
            var formContent = Encoding.UTF8.GetBytes(text);
            Pipe bodyPipe = new Pipe();

            await bodyPipe.Writer.WriteAsync(formContent);

            // Complete the writer so the reader will complete after processing all data.
            bodyPipe.Writer.Complete();
            return bodyPipe.Reader;
        }
    }
}
