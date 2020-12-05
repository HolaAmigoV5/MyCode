// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.RenderTree;

namespace Microsoft.AspNetCore.Components
{
    // IMPORTANT
    //
    // Many of these names are used in code generation. Keep these in sync with the code generation code
    // See: src/Microsoft.AspNetCore.Components.Razor.Extensions/ComponentsApi.cs

    // Most of the developer-facing component lifecycle concepts are encapsulated in this
    // base class. The core components rendering system doesn't know about them (it only knows
    // about IComponent). This gives us flexibility to change the lifecycle concepts easily,
    // or for developers to design their own lifecycles as different base classes.

    // TODO: When the component lifecycle design stabilises, add proper unit tests for ComponentBase.

    /// <summary>
    /// Optional base class for components. Alternatively, components may
    /// implement <see cref="IComponent"/> directly.
    /// </summary>
    public abstract class ComponentBase : IComponent, IHandleEvent, IHandleAfterRender
    {
        private readonly RenderFragment _renderFragment;
        private RenderHandle _renderHandle;
        private bool _initialized;
        private bool _hasNeverRendered = true;
        private bool _hasPendingQueuedRender;

        /// <summary>
        /// Constructs an instance of <see cref="ComponentBase"/>.
        /// </summary>
        public ComponentBase()
        {
            _renderFragment = builder =>
            {
                _hasPendingQueuedRender = false;
                _hasNeverRendered = false;
                BuildRenderTree(builder);
            };
        }

        /// <summary>
        /// Renders the component to the supplied <see cref="RenderTreeBuilder"/>.
        /// </summary>
        /// <param name="builder">A <see cref="RenderTreeBuilder"/> that will receive the render output.</param>
        protected virtual void BuildRenderTree(RenderTreeBuilder builder)
        {
            // Developers can either override this method in derived classes, or can use Razor
            // syntax to define a derived class and have the compiler generate the method.

            // Other code within this class should *not* invoke BuildRenderTree directly,
            // but instead should invoke the _renderFragment field.
        }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// </summary>
        protected virtual void OnInit()
        {
        }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        ///
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        protected virtual Task OnInitAsync()
            => Task.CompletedTask;

        /// <summary>
        /// Method invoked when the component has received parameters from its parent in
        /// the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        protected virtual void OnParametersSet()
        {
        }

        /// <summary>
        /// Method invoked when the component has received parameters from its parent in
        /// the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        protected virtual Task OnParametersSetAsync()
            => Task.CompletedTask;

        /// <summary>
        /// Notifies the component that its state has changed. When applicable, this will
        /// cause the component to be re-rendered.
        /// </summary>
        protected void StateHasChanged()
        {
            if (_hasPendingQueuedRender)
            {
                return;
            }

            if (_hasNeverRendered || ShouldRender())
            {
                _hasPendingQueuedRender = true;

                try
                {
                    _renderHandle.Render(_renderFragment);
                }
                catch
                {
                    _hasPendingQueuedRender = false;
                    throw;
                }
            }
        }

        /// <summary>
        /// Returns a flag to indicate whether the component should render.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ShouldRender()
            => true;

        /// <summary>
        /// Method invoked after each time the component has been rendered.
        /// </summary>
        protected virtual void OnAfterRender()
        {
        }

        /// <summary>
        /// Method invoked after each time the component has been rendered. Note that the component does
        /// not automatically re-render after the completion of any returned <see cref="Task"/>, because
        /// that would cause an infinite render loop.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        protected virtual Task OnAfterRenderAsync()
            => Task.CompletedTask;

        /// <summary>
        /// Executes the supplied work item on the associated renderer's
        /// synchronization context.
        /// </summary>
        /// <param name="workItem">The work item to execute.</param>
        protected Task Invoke(Action workItem)
            => _renderHandle.Invoke(workItem);

        /// <summary>
        /// Executes the supplied work item on the associated renderer's
        /// synchronization context.
        /// </summary>
        /// <param name="workItem">The work item to execute.</param>
        protected Task InvokeAsync(Func<Task> workItem)
            => _renderHandle.InvokeAsync(workItem);

        void IComponent.Configure(RenderHandle renderHandle)
        {
            // This implicitly means a ComponentBase can only be associated with a single
            // renderer. That's the only use case we have right now. If there was ever a need,
            // a component could hold a collection of render handles.
            if (_renderHandle.IsInitialized)
            {
                throw new InvalidOperationException($"The render handle is already set. Cannot initialize a {nameof(ComponentBase)} more than once.");
            }

            _renderHandle = renderHandle;
        }

        /// <summary>
        /// Method invoked to apply initial or updated parameters to the component.
        /// </summary>
        /// <param name="parameters">The parameters to apply.</param>
        public virtual Task SetParametersAsync(ParameterCollection parameters)
        {
            parameters.SetParameterProperties(this);
            if (!_initialized)
            {
                _initialized = true;

                return RunInitAndSetParametersAsync();
            }
            else
            {
                return CallOnParametersSetAsync();
            }
        }

        private async Task RunInitAndSetParametersAsync()
        {
            OnInit();
            var task = OnInitAsync();

            if (task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Canceled)
            {
                // Call state has changed here so that we render after the sync part of OnInitAsync has run
                // and wait for it to finish before we continue. If no async work has been done yet, we want
                // to defer calling StateHasChanged up until the first bit of async code happens or until
                // the end. Additionally, we want to avoid calling StateHasChanged if no
                // async work is to be performed.
                StateHasChanged();

                try
                {
                    await task;
                }
                catch when (task.IsCanceled)
                {
                    // Ignore exceptions from task cancelletions.
                    // Awaiting a canceled task may produce either an OperationCanceledException (if produced as a consequence of
                    // CancellationToken.ThrowIfCancellationRequested()) or a TaskCanceledException (produced as a consequence of awaiting Task.FromCanceled).
                    // It's much easier to check the state of the Task (i.e. Task.IsCanceled) rather than catch two distinct exceptions.
                }

                // Don't call StateHasChanged here. CallOnParametersSetAsync should handle that for us.
            }

            await CallOnParametersSetAsync();
        }

        private Task CallOnParametersSetAsync()
        {
            OnParametersSet();
            var task = OnParametersSetAsync();
            // If no async work is to be performed, i.e. the task has already ran to completion
            // or was canceled by the time we got to inspect it, avoid going async and re-invoking
            // StateHasChanged at the culmination of the async work.
            var shouldAwaitTask = task.Status != TaskStatus.RanToCompletion &&
                task.Status != TaskStatus.Canceled;

            // We always call StateHasChanged here as we want to trigger a rerender after OnParametersSet and
            // the synchronous part of OnParametersSetAsync has run.
            StateHasChanged();

            return shouldAwaitTask ?
                CallStateHasChangedOnAsyncCompletion(task) :
                Task.CompletedTask;
        }

        private async Task CallStateHasChangedOnAsyncCompletion(Task task)
        {
            try
            {
                await task;
            }
            catch when (task.IsCanceled)
            {
                // Ignore exceptions from task cancelletions, but don't bother issuing a state change.
                return;
            }

            StateHasChanged();
        }

        Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object arg)
        {
            var task = callback.InvokeAsync(arg);
            var shouldAwaitTask = task.Status != TaskStatus.RanToCompletion &&
                task.Status != TaskStatus.Canceled;

            // After each event, we synchronously re-render (unless !ShouldRender())
            // This just saves the developer the trouble of putting "StateHasChanged();"
            // at the end of every event callback.
            StateHasChanged();

            return shouldAwaitTask ?
                CallStateHasChangedOnAsyncCompletion(task) :
                Task.CompletedTask;
        }

        Task IHandleAfterRender.OnAfterRenderAsync()
        {
            OnAfterRender();

            return OnAfterRenderAsync();

            // Note that we don't call StateHasChanged to trigger a render after
            // handling this, because that would be an infinite loop. The only
            // reason we have OnAfterRenderAsync is so that the developer doesn't
            // have to use "async void" and do their own exception handling in
            // the case where they want to start an async task.
        }
    }
}
