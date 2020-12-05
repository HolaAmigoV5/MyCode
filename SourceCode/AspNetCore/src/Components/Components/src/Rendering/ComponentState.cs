// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.RenderTree;

namespace Microsoft.AspNetCore.Components.Rendering
{
    /// <summary>
    /// Tracks the rendering state associated with an <see cref="IComponent"/> instance
    /// within the context of a <see cref="Renderer"/>. This is an internal implementation
    /// detail of <see cref="Renderer"/>.
    /// </summary>
    internal class ComponentState
    {
        private readonly Renderer _renderer;
        private readonly IReadOnlyList<CascadingParameterState> _cascadingParameters;
        private readonly bool _hasAnyCascadingParameterSubscriptions;
        private RenderTreeBuilder _renderTreeBuilderPrevious;
        private ArrayBuilder<RenderTreeFrame> _latestDirectParametersSnapshot; // Lazily instantiated
        private bool _componentWasDisposed;

        /// <summary>
        /// Constructs an instance of <see cref="ComponentState"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="Renderer"/> with which the new instance should be associated.</param>
        /// <param name="componentId">The externally visible identifier for the <see cref="IComponent"/>. The identifier must be unique in the context of the <see cref="Renderer"/>.</param>
        /// <param name="component">The <see cref="IComponent"/> whose state is being tracked.</param>
        /// <param name="parentComponentState">The <see cref="ComponentState"/> for the parent component, or null if this is a root component.</param>
        public ComponentState(Renderer renderer, int componentId, IComponent component, ComponentState parentComponentState)
        {
            ComponentId = componentId;
            ParentComponentState = parentComponentState;
            Component = component ?? throw new ArgumentNullException(nameof(component));
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            _cascadingParameters = CascadingParameterState.FindCascadingParameters(this);
            CurrrentRenderTree = new RenderTreeBuilder(renderer);
            _renderTreeBuilderPrevious = new RenderTreeBuilder(renderer);

            if (_cascadingParameters != null)
            {
                _hasAnyCascadingParameterSubscriptions = AddCascadingParameterSubscriptions();
            }
        }

        // TODO: Change the type to 'long' when the Mono runtime has more complete support for passing longs in .NET->JS calls
        public int ComponentId { get; }
        public IComponent Component { get; }
        public ComponentState ParentComponentState { get; }
        public RenderTreeBuilder CurrrentRenderTree { get; private set; }

        public void RenderIntoBatch(RenderBatchBuilder batchBuilder, RenderFragment renderFragment)
        {
            // A component might be in the render queue already before getting disposed by an
            // earlier entry in the render queue. In that case, rendering is a no-op.
            if (_componentWasDisposed)
            {
                return;
            }

            // Swap the old and new tree builders
            (CurrrentRenderTree, _renderTreeBuilderPrevious) = (_renderTreeBuilderPrevious, CurrrentRenderTree);

            CurrrentRenderTree.Clear();
            renderFragment(CurrrentRenderTree);

            var diff = RenderTreeDiffBuilder.ComputeDiff(
                _renderer,
                batchBuilder,
                ComponentId,
                _renderTreeBuilderPrevious.GetFrames(),
                CurrrentRenderTree.GetFrames());
            batchBuilder.UpdatedComponentDiffs.Append(diff);
        }

        public void DisposeInBatch(RenderBatchBuilder batchBuilder)
        {
            _componentWasDisposed = true;

            // TODO: Handle components throwing during dispose. Shouldn't break the whole render batch.
            if (Component is IDisposable disposable)
            {
                disposable.Dispose();
            }

            RenderTreeDiffBuilder.DisposeFrames(batchBuilder, CurrrentRenderTree.GetFrames());

            if (_hasAnyCascadingParameterSubscriptions)
            {
                RemoveCascadingParameterSubscriptions();
            }
        }

        public Task NotifyRenderCompletedAsync()
        {
            if (Component is IHandleAfterRender handlerAfterRender)
            {
                return handlerAfterRender.OnAfterRenderAsync();
            }

            return Task.CompletedTask;
        }

        public void SetDirectParameters(ParameterCollection parameters)
        {
            // Note: We should be careful to ensure that the framework never calls
            // IComponent.SetParameters directly elsewhere. We should only call it
            // via ComponentState.SetParameters (or NotifyCascadingValueChanged below).
            // If we bypass this, the component won't receive the cascading parameters nor
            // will it update its snapshot of direct parameters.

            if (_hasAnyCascadingParameterSubscriptions)
            {
                // We may need to replay these direct parameters later (in NotifyCascadingValueChanged),
                // but we can't guarantee that the original underlying data won't have mutated in the
                // meantime, since it's just an index into the parent's RenderTreeFrames buffer.
                if (_latestDirectParametersSnapshot == null)
                {
                    _latestDirectParametersSnapshot = new ArrayBuilder<RenderTreeFrame>();
                }

                parameters.CaptureSnapshot(_latestDirectParametersSnapshot);
            }

            if (_cascadingParameters != null)
            {
                parameters = parameters.WithCascadingParameters(_cascadingParameters);
            }

            _renderer.AddToPendingTasks(Component.SetParametersAsync(parameters));
        }

        public void NotifyCascadingValueChanged()
        {
            var directParams = _latestDirectParametersSnapshot != null
                ? new ParameterCollection(_latestDirectParametersSnapshot.Buffer, 0)
                : ParameterCollection.Empty;
            var allParams = directParams.WithCascadingParameters(_cascadingParameters);
            var task = Component.SetParametersAsync(allParams);
            _renderer.AddToPendingTasks(task);
        }

        private bool AddCascadingParameterSubscriptions()
        {
            var hasSubscription = false;
            var numCascadingParameters = _cascadingParameters.Count;

            for (var i = 0; i < numCascadingParameters; i++)
            {
                var valueSupplier = _cascadingParameters[i].ValueSupplier;
                if (!valueSupplier.CurrentValueIsFixed)
                {
                    valueSupplier.Subscribe(this);
                    hasSubscription = true;
                }
            }

            return hasSubscription;
        }

        private void RemoveCascadingParameterSubscriptions()
        {
            var numCascadingParameters = _cascadingParameters.Count;
            for (var i = 0; i < numCascadingParameters; i++)
            {
                _cascadingParameters[i].ValueSupplier.Unsubscribe(this);
            }
        }
    }
}
