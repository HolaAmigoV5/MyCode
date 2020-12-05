// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNetCore.Components.Server
{
    public class RenderBatchWriterTest
    {
        static object NullStringMarker = new object();

        [Fact]
        public void CanSerializeEmptyRenderBatch()
        {
            // Arrange/Act
            var bytes = Serialize(new RenderBatch());

            // Assert
            AssertBinaryContents(bytes, /* startIndex */ 0,
                0,  // Length of UpdatedComponents
                0,  // Length of ReferenceFrames
                0,  // Length of DisposedComponentIds
                0,  // Length of DisposedEventHandlerIds

                0,  // Index of UpdatedComponents
                4,  // Index of ReferenceFrames
                8,  // Index of DisposedComponentIds
                12, // Index of DisposedEventHandlerIds
                16  // Index of Strings
            );
            Assert.Equal(36, bytes.Length); // No other data
        }

        [Fact]
        public void CanIncludeDisposedComponentIds()
        {
            // Arrange/Act
            var bytes = Serialize(new RenderBatch(
                default,
                default,
                new ArrayRange<int>(new[] { 123, int.MaxValue, int.MinValue, 456 }, 3), // Only use first 3 to show that param is respected
                default));

            // Assert
            AssertBinaryContents(bytes, /* startIndex */ 0,
                0,  // Length of UpdatedComponents
                0,  // Length of ReferenceFrames                
                3, 123, int.MaxValue, int.MinValue, // DisposedComponentIds as length-prefixed array
                0,  // Length of DisposedEventHandlerIds

                0,  // Index of UpdatedComponents
                4,  // Index of ReferenceFrames
                8,  // Index of DisposedComponentIds
                24, // Index of DisposedEventHandlerIds
                28  // Index of strings
            );
            Assert.Equal(48, bytes.Length); // No other data
        }

        [Fact]
        public void CanIncludeDisposedEventHandlerIds()
        {
            // Arrange/Act
            var bytes = Serialize(new RenderBatch(
                new ArrayRange<RenderTreeDiff>(),
                new ArrayRange<RenderTreeFrame>(),
                new ArrayRange<int>(),
                new ArrayRange<int>(new[] { 123, int.MaxValue, int.MinValue, 456 }, 3) // Only use first 3 to show that param is respected
                ));

            // Assert
            AssertBinaryContents(bytes, /* startIndex */ 0,
                0,  // Length of UpdatedComponents
                0,  // Length of ReferenceFrames
                0,  // Length of DisposedComponentIds
                3, 123, int.MaxValue, int.MinValue, // DisposedEventHandlerIds as length-prefixed array

                0,  // Index of UpdatedComponents
                4,  // Index of ReferenceFrames
                8,  // Index of DisposedComponentIds
                12, // Index of DisposedEventHandlerIds
                28  // Index of strings
            );
            Assert.Equal(48, bytes.Length); // No other data
        }

        [Fact]
        public void CanIncludeUpdatedComponentsWithEmptyEdits()
        {
            // Arrange/Act
            var bytes = Serialize(new RenderBatch(
                new ArrayRange<RenderTreeDiff>(new[]
                {
                    new RenderTreeDiff(123, default),
                    new RenderTreeDiff(int.MaxValue, default),
                }, 2),
                default,
                default,
                default));

            // Assert
            AssertBinaryContents(bytes, /* startIndex */ 0,
                // UpdatedComponents[0]
                123, // ComponentId
                0,   // Edits length

                // UpdatedComponents[1]
                int.MaxValue, // ComponentId
                0,   // Edits length

                2,   // Length of UpdatedComponents
                0,   // Index of UpdatedComponents[0]
                8,   // Index of UpdatedComponents[1]
                
                0,   // Length of ReferenceFrames
                0,   // Length of DisposedComponentIds
                0,   // Length of DisposedEventHandlerIds

                16,  // Index of UpdatedComponents
                28,  // Index of ReferenceFrames
                32,  // Index of DisposedComponentIds
                36,  // Index of DisposedEventHandlerIds
                40   // Index of strings
            );
            Assert.Equal(60, bytes.Length); // No other data
        }

        [Fact]
        public void CanIncludeEdits()
        {
            // Arrange/Act
            var edits = new[]
            {
                default, // Skipped (because offset=1 below)
                RenderTreeEdit.PrependFrame(456, 789),
                RenderTreeEdit.RemoveFrame(101),
                RenderTreeEdit.SetAttribute(102, 103),
                RenderTreeEdit.RemoveAttribute(104, "Some removed attribute"),
                RenderTreeEdit.UpdateText(105, 106),
                RenderTreeEdit.StepIn(107),
                RenderTreeEdit.StepOut(),
                RenderTreeEdit.UpdateMarkup(108, 109),
                RenderTreeEdit.RemoveAttribute(110, "Some removed attribute"), // To test deduplication
            };
            var bytes = Serialize(new RenderBatch(
                new ArrayRange<RenderTreeDiff>(new[]
                {
                    new RenderTreeDiff(123, new ArraySegment<RenderTreeEdit>(
                        edits, 1, edits.Length - 1)) // Skip first to show offset is respected
                }, 1),
                default,
                default,
                default));

            // Assert
            var diffsStartIndex = ReadInt(bytes, bytes.Length - 20);
            AssertBinaryContents(bytes, diffsStartIndex,
                1,  // Number of diffs
                0); // Index of diffs[0]

            AssertBinaryContents(bytes, 0,
                123, // Component ID for diff 0
                9,  // diff[0].Edits.Count
                RenderTreeEditType.PrependFrame, 456, 789, NullStringMarker,
                RenderTreeEditType.RemoveFrame, 101, 0, NullStringMarker,
                RenderTreeEditType.SetAttribute, 102, 103, NullStringMarker,
                RenderTreeEditType.RemoveAttribute, 104, 0, "Some removed attribute",
                RenderTreeEditType.UpdateText, 105, 106, NullStringMarker,
                RenderTreeEditType.StepIn, 107, 0, NullStringMarker,
                RenderTreeEditType.StepOut, 0, 0, NullStringMarker,
                RenderTreeEditType.UpdateMarkup, 108, 109, NullStringMarker,
                RenderTreeEditType.RemoveAttribute, 110, 0, "Some removed attribute"
            );

            // We can deduplicate attribute names
            Assert.Equal(new[] { "Some removed attribute" }, ReadStringTable(bytes));
        }

        [Fact]
        public void CanIncludeReferenceFrames()
        {
            // Arrange/Act
            var renderer = new FakeRenderer();
            var bytes = Serialize(new RenderBatch(
                default,
                new ArrayRange<RenderTreeFrame>(new[] {
                    RenderTreeFrame.Attribute(123, "Attribute with string value", "String value"),
                    RenderTreeFrame.Attribute(124, "Attribute with nonstring value", 1),
                    RenderTreeFrame.Attribute(125, "Attribute with delegate value", new Action(() => { }))
                        .WithAttributeEventHandlerId(789),
                    RenderTreeFrame.ChildComponent(126, typeof(object))
                        .WithComponentSubtreeLength(5678)
                        .WithComponent(new ComponentState(renderer, 2000, new FakeComponent(), null)),
                    RenderTreeFrame.ComponentReferenceCapture(127, value => { }, 1001),
                    RenderTreeFrame.Element(128, "Some element")
                        .WithElementSubtreeLength(1234),
                    RenderTreeFrame.ElementReferenceCapture(129, value => { })
                        .WithElementReferenceCaptureId("my unique ID"),
                    RenderTreeFrame.Region(130)
                        .WithRegionSubtreeLength(1234),
                    RenderTreeFrame.Text(131, "Some text"),
                    RenderTreeFrame.Markup(132, "Some markup"),
                    RenderTreeFrame.Text(133, "\n\t  "),

                    // Testing deduplication
                    RenderTreeFrame.Attribute(134, "Attribute with string value", "String value"),
                    RenderTreeFrame.Element(135, "Some element") // Will be deduplicated
                        .WithElementSubtreeLength(999),
                    RenderTreeFrame.Text(136, "Some text"), // Will not be deduplicated
                    RenderTreeFrame.Markup(137, "Some markup"), // Will not be deduplicated
                    RenderTreeFrame.Text(138, "\n\t  "), // Will be deduplicated
                }, 16),
                default,
                default));

            // Assert
            var referenceFramesStartIndex = ReadInt(bytes, bytes.Length - 16);
            AssertBinaryContents(bytes, referenceFramesStartIndex,
                16, // Number of frames
                RenderTreeFrameType.Attribute, "Attribute with string value", "String value", 0,
                RenderTreeFrameType.Attribute, "Attribute with nonstring value", NullStringMarker, 0,
                RenderTreeFrameType.Attribute, "Attribute with delegate value", NullStringMarker, 789,
                RenderTreeFrameType.Component, 5678, 2000, 0,
                RenderTreeFrameType.ComponentReferenceCapture, 0, 0, 0,
                RenderTreeFrameType.Element, 1234, "Some element", 0,
                RenderTreeFrameType.ElementReferenceCapture, "my unique ID", 0, 0,
                RenderTreeFrameType.Region, 1234, 0, 0,
                RenderTreeFrameType.Text, "Some text", 0, 0,
                RenderTreeFrameType.Markup, "Some markup", 0, 0,
                RenderTreeFrameType.Text, "\n\t  ", 0, 0,
                RenderTreeFrameType.Attribute, "Attribute with string value", "String value", 0,
                RenderTreeFrameType.Element, 999, "Some element", 0,
                RenderTreeFrameType.Text, "Some text", 0, 0,
                RenderTreeFrameType.Markup, "Some markup", 0, 0,
                RenderTreeFrameType.Text, "\n\t  ", 0, 0
            );

            Assert.Equal(new[]
            {
                "Attribute with string value",
                "String value",
                "Attribute with nonstring value",
                "Attribute with delegate value",
                "Some element",
                "my unique ID",
                "Some text",
                "Some markup",
                "\n\t  ",
                "String value",
                "Some text",
                "Some markup",
            }, ReadStringTable(bytes));
        }

        private Span<byte> Serialize(RenderBatch renderBatch)
        {
            using (var ms = new MemoryStream())
            using (var writer = new RenderBatchWriter(ms, leaveOpen: false))
            {
                writer.Write(renderBatch);
                return new Span<byte>(ms.ToArray(), 0, (int)ms.Length);
            }
        }

        static string[] ReadStringTable(Span<byte> data)
        {
            var bytes = data.ToArray();

            // The string table position is given by the final int, and continues
            // until we get to the final set of top-level indices
            var stringTableStartPosition = BitConverter.ToInt32(bytes, bytes.Length - 4);
            var stringTableEndPositionExcl = bytes.Length - 20;

            var result = new List<string>();
            for (var entryPosition = stringTableStartPosition;
                entryPosition < stringTableEndPositionExcl;
                entryPosition += 4)
            {
                // The string table entries are all length-prefixed UTF8 blobs
                var tableEntryPos = BitConverter.ToInt32(bytes, entryPosition);
                var length = (int)ReadUnsignedLEB128(bytes, tableEntryPos, out var numLEB128Bytes);
                var value = Encoding.UTF8.GetString(bytes, tableEntryPos + numLEB128Bytes, length);
                result.Add(value);
            }

            return result.ToArray();
        }

        static void AssertBinaryContents(Span<byte> data, int startIndex, params object[] entries)
        {
            var bytes = data.ToArray();
            var stringTableEntries = ReadStringTable(data);

            using (var ms = new MemoryStream(bytes))
            using (var reader = new BinaryReader(ms))
            {
                ms.Seek(startIndex, SeekOrigin.Begin);

                foreach (var expectedEntryIterationVar in entries)
                {
                    // Assume enums are represented as ints
                    var expectedEntry = expectedEntryIterationVar.GetType().IsEnum
                        ? (int)expectedEntryIterationVar
                        : expectedEntryIterationVar;

                    if (expectedEntry is int expectedInt)
                    {
                        Assert.Equal(expectedInt, reader.ReadInt32());
                    }
                    else if (expectedEntry is string || expectedEntry == NullStringMarker)
                    {
                        // For strings, we have to look up the value in the table of strings
                        // that appears at the end of the serialized data
                        var indexIntoStringTable = reader.ReadInt32();
                        var expectedString = expectedEntry as string;
                        if (expectedString == null)
                        {
                            Assert.Equal(-1, indexIntoStringTable);
                        }
                        else
                        {
                            Assert.Equal(expectedString, stringTableEntries[indexIntoStringTable]);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unsupported type: {expectedEntry.GetType().FullName}");
                    }
                }
            }
        }

        static int ReadInt(Span<byte> bytes, int startOffset)
            => BitConverter.ToInt32(bytes.Slice(startOffset, 4).ToArray(), 0);

        public static uint ReadUnsignedLEB128(byte[] bytes, int startOffset, out int numBytesRead)
        {
            var result = (uint)0;
            var shift = 0;
            var currentByte = (byte)128;
            numBytesRead = 0;

            for (var count = 0; count < 4 && currentByte >= 128; count++)
            {
                currentByte = bytes[startOffset + count];
                result += (uint)(currentByte & 0x7f) << shift;
                shift += 7;
                numBytesRead++;
            }
            
            return result;
        }

        class FakeComponent : IComponent
        {
            public void Configure(RenderHandle renderHandle)
                => throw new NotImplementedException();

            public Task SetParametersAsync(ParameterCollection parameters)
                => throw new NotImplementedException();
        }

        class FakeRenderer : Renderer
        {
            public FakeRenderer()
                : base(new ServiceCollection().BuildServiceProvider(), new RendererSynchronizationContext())
            {
            }

            protected override void HandleException(Exception exception)
            {
                throw new NotImplementedException();
            }

            protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
                => throw new NotImplementedException();
        }
    }
}
