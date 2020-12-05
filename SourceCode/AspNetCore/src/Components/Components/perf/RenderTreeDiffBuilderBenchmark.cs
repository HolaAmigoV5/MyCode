// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;

namespace Microsoft.AspNetCore.Components.Performance
{
    public class RenderTreeDiffBuilderBenchmark
    {
        private readonly Renderer renderer;
        private readonly RenderTreeBuilder original;
        private readonly RenderTreeBuilder modified;
        private readonly RenderBatchBuilder builder;

        public RenderTreeDiffBuilderBenchmark()
        {
            builder = new RenderBatchBuilder();
            renderer = new FakeRenderer();

            // A simple component for basic tests -- this is similar to what MVC scaffolding generates
            // for bootstrap3 form fields, but modified to be more Component-like.
            original = new RenderTreeBuilder(renderer);
            original.OpenElement(0, "div");
            original.AddAttribute(1, "class", "form-group");

            original.OpenElement(2, "label");
            original.AddAttribute(3, "class", "control-label");
            original.AddAttribute(4, "for", "name");
            original.AddAttribute(5, "data-unvalidated", true);
            original.AddContent(6, "Car");
            original.CloseElement();

            original.OpenElement(7, "input");
            original.AddAttribute(8, "class", "form-control");
            original.AddAttribute(9, "type", "text");
            original.AddAttribute(10, "name", "name"); // Notice the gap in sequence numbers
            original.AddAttribute(12, "value", "");
            original.CloseElement();

            original.OpenElement(13, "span");
            original.AddAttribute(14, "class", "text-danger field-validation-valid");
            original.AddContent(15, "");
            original.CloseElement();

            original.CloseElement();

            // Now simulate some input
            modified = new RenderTreeBuilder(renderer);
            modified.OpenElement(0, "div");
            modified.AddAttribute(1, "class", "form-group");

            modified.OpenElement(2, "label");
            modified.AddAttribute(3, "class", "control-label");
            modified.AddAttribute(4, "for", "name");
            modified.AddAttribute(5, "data-unvalidated", false);
            modified.AddContent(6, "Car");
            modified.CloseElement();

            modified.OpenElement(7, "input");
            modified.AddAttribute(8, "class", "form-control");
            modified.AddAttribute(9, "type", "text");
            modified.AddAttribute(10, "name", "name");
            modified.AddAttribute(11, "data-validation-state", "invalid");
            modified.AddAttribute(12, "value", "Lamborghini");
            modified.CloseElement();

            modified.OpenElement(13, "span");
            modified.AddAttribute(14, "class", "text-danger field-validation-invalid"); // changed
            modified.AddContent(15, "No, you can't afford that.");
            modified.CloseElement();

            modified.CloseElement();
        }

        [Benchmark(Description = "RenderTreeDiffBuilder: Input and validation on a single form field.", Baseline = true)]
        public void ComputeDiff_SingleFormField()
        {
            builder.Clear();
            var diff = RenderTreeDiffBuilder.ComputeDiff(renderer, builder, 0, original.GetFrames(), modified.GetFrames());
            GC.KeepAlive(diff);
        }

        private class FakeRenderer : Renderer
        {
            public FakeRenderer()
                : base(new TestServiceProvider(), new RendererSynchronizationContext())
            {
            }

            protected override void HandleException(Exception exception)
            {
                throw new NotImplementedException();
            }

            protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
                => Task.CompletedTask;
        }

        private class TestServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                return null;
            }
        }
    }
}
