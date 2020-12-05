// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Test.Helpers;
using Microsoft.AspNetCore.Testing;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;

namespace Microsoft.AspNetCore.Components.Test
{
    public class RendererTest
    {
        private const string EventActionsName = nameof(NestedAsyncComponent.EventActions);
        private const string WhatToRenderName = nameof(NestedAsyncComponent.WhatToRender);
        private const string LogName = nameof(NestedAsyncComponent.Log);

        [Fact]
        public void CanRenderTopLevelComponents()
        {
            // Arrange
            var renderer = new TestRenderer();
            var component = new TestComponent(builder =>
            {
                builder.OpenElement(0, "my element");
                builder.AddContent(1, "some text");
                builder.CloseElement();
            });

            // Act
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();

            // Assert
            var batch = renderer.Batches.Single();
            var diff = batch.DiffsByComponentId[componentId].Single();
            Assert.Collection(diff.Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.PrependFrame, edit.Type);
                    Assert.Equal(0, edit.ReferenceFrameIndex);
                });
            AssertFrame.Element(batch.ReferenceFrames[0], "my element", 2);
            AssertFrame.Text(batch.ReferenceFrames[1], "some text");
        }

        [Fact]
        public void CanRenderNestedComponents()
        {
            // Arrange
            var renderer = new TestRenderer();
            var component = new TestComponent(builder =>
            {
                builder.AddContent(0, "Hello");
                builder.OpenComponent<MessageComponent>(1);
                builder.AddAttribute(2, nameof(MessageComponent.Message), "Nested component output");
                builder.CloseComponent();
            });

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();
            var batch = renderer.Batches.Single();
            var componentFrame = batch.ReferenceFrames
                .Single(frame => frame.FrameType == RenderTreeFrameType.Component);
            var nestedComponentId = componentFrame.ComponentId;
            var nestedComponentDiff = batch.DiffsByComponentId[nestedComponentId].Single();

            // We rendered both components
            Assert.Equal(2, batch.DiffsByComponentId.Count);

            // The nested component exists
            Assert.IsType<MessageComponent>(componentFrame.Component);

            // The nested component was rendered as part of the batch
            Assert.Collection(nestedComponentDiff.Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.PrependFrame, edit.Type);
                    AssertFrame.Text(
                        batch.ReferenceFrames[edit.ReferenceFrameIndex],
                        "Nested component output");
                });
        }

        [Fact]
        public void CanReRenderTopLevelComponents()
        {
            // Arrange
            var renderer = new TestRenderer();
            var component = new MessageComponent { Message = "Initial message" };
            var componentId = renderer.AssignRootComponentId(component);

            // Act/Assert: first render
            component.TriggerRender();
            var batch = renderer.Batches.Single();
            var firstDiff = batch.DiffsByComponentId[componentId].Single();
            Assert.Collection(firstDiff.Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.PrependFrame, edit.Type);
                    Assert.Equal(0, edit.ReferenceFrameIndex);
                    AssertFrame.Text(batch.ReferenceFrames[0], "Initial message");
                });

            // Act/Assert: second render
            component.Message = "Modified message";
            component.TriggerRender();
            var secondBatch = renderer.Batches.Skip(1).Single();
            var secondDiff = secondBatch.DiffsByComponentId[componentId].Single();
            Assert.Collection(secondDiff.Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.UpdateText, edit.Type);
                    Assert.Equal(0, edit.ReferenceFrameIndex);
                    AssertFrame.Text(secondBatch.ReferenceFrames[0], "Modified message");
                });
        }

        [Fact]
        public void CanReRenderNestedComponents()
        {
            // Arrange: parent component already rendered
            var renderer = new TestRenderer();
            var parentComponent = new TestComponent(builder =>
            {
                builder.OpenComponent<MessageComponent>(0);
                builder.CloseComponent();
            });
            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            parentComponent.TriggerRender();
            var nestedComponentFrame = renderer.Batches.Single()
                .ReferenceFrames
                .Single(frame => frame.FrameType == RenderTreeFrameType.Component);
            var nestedComponent = (MessageComponent)nestedComponentFrame.Component;
            var nestedComponentId = nestedComponentFrame.ComponentId;

            // Assert: initial render
            nestedComponent.Message = "Render 1";
            nestedComponent.TriggerRender();
            var batch = renderer.Batches[1];
            var firstDiff = batch.DiffsByComponentId[nestedComponentId].Single();
            Assert.Collection(firstDiff.Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.UpdateText, edit.Type);
                    Assert.Equal(0, edit.ReferenceFrameIndex);
                    AssertFrame.Text(batch.ReferenceFrames[0], "Render 1");
                });

            // Act/Assert: re-render
            nestedComponent.Message = "Render 2";
            nestedComponent.TriggerRender();
            var secondBatch = renderer.Batches[2];
            var secondDiff = secondBatch.DiffsByComponentId[nestedComponentId].Single();
            Assert.Collection(secondDiff.Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.UpdateText, edit.Type);
                    Assert.Equal(0, edit.ReferenceFrameIndex);
                    AssertFrame.Text(secondBatch.ReferenceFrames[0], "Render 2");
                });
        }

        [Fact]
        public async Task CanRenderAsyncTopLevelComponents()
        {
            // Arrange
            var renderer = new TestRenderer();
            var tcs = new TaskCompletionSource<int>();
            var component = new AsyncComponent(tcs.Task, 5); // Triggers n renders, the first one creating <p>n</p> and the n-1 renders asynchronously update the value.

            // Act
            var componentId = renderer.AssignRootComponentId(component);
            var renderTask = renderer.InvokeAsync(() => renderer.RenderRootComponentAsync(componentId));

            // Assert
            Assert.False(renderTask.IsCompleted);
            tcs.SetResult(0);
            await renderTask;
            Assert.Equal(5, renderer.Batches.Count);

            // First render
            var create = renderer.Batches[0];
            var diff = create.DiffsByComponentId[componentId].Single();
            Assert.Collection(diff.Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.PrependFrame, edit.Type);
                    Assert.Equal(0, edit.ReferenceFrameIndex);
                });
            AssertFrame.Element(create.ReferenceFrames[0], "p", 2);
            AssertFrame.Text(create.ReferenceFrames[1], "5");

            // Second render
            for (var i = 1; i < 5; i++)
            {

                var update = renderer.Batches[i];
                var updateDiff = update.DiffsByComponentId[componentId].Single();
                Assert.Collection(updateDiff.Edits,
                    edit =>
                    {
                        Assert.Equal(RenderTreeEditType.StepIn, edit.Type);
                    },
                    edit =>
                    {
                        Assert.Equal(RenderTreeEditType.UpdateText, edit.Type);
                    },
                    edit =>
                    {
                        Assert.Equal(RenderTreeEditType.StepOut, edit.Type);
                    });
                AssertFrame.Text(update.ReferenceFrames[0], (5 - i).ToString());
            }
        }

        [Fact]
        public async Task CanRenderAsyncNestedComponents()
        {
            // Arrange
            var renderer = new TestRenderer();
            var component = new NestedAsyncComponent();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            var log = new ConcurrentQueue<(int id, NestedAsyncComponent.EventType @event)>();
            await renderer.InvokeAsync(() => renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [EventActionsName] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnInit),
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnInitAsyncAsync, async:true),
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnParametersSet),
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnParametersSetAsyncAsync, async: true),
                    },
                    [1] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnInit),
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnInitAsyncAsync, async:true),
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnParametersSet),
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnParametersSetAsyncAsync, async: true),
                    }
                },
                [WhatToRenderName] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(new[] { 1 }),
                    [1] = CreateRenderFactory(Array.Empty<int>())
                },
                [LogName] = log
            })));

            var logForParent = log.Where(l => l.id == 0).ToArray();
            var logForChild = log.Where(l => l.id == 1).ToArray();

            AssertStream(0, logForParent);
            AssertStream(1, logForChild);
        }

        [Fact]
        public async Task CanRenderAsyncComponentsWithSyncChildComponents()
        {
            // Arrange
            var renderer = new TestRenderer();
            var component = new NestedAsyncComponent();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            var log = new ConcurrentQueue<(int id, NestedAsyncComponent.EventType @event)>();
            await renderer.InvokeAsync(() => renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [EventActionsName] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnInit),
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnInitAsyncAsync, async:true),
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnParametersSet),
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnParametersSetAsyncAsync, async: true),
                    },
                    [1] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnInit),
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnInitAsyncAsync),
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnParametersSet),
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnParametersSetAsyncAsync),
                    }
                },
                [WhatToRenderName] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(new[] { 1 }),
                    [1] = CreateRenderFactory(Array.Empty<int>())
                },
                [LogName] = log
            })));

            var logForParent = log.Where(l => l.id == 0).ToArray();
            var logForChild = log.Where(l => l.id == 1).ToArray();

            AssertStream(0, logForParent);
            AssertStream(1, logForChild);
        }

        [Fact]
        public async Task CanRenderAsyncComponentsWithAsyncChildInit()
        {
            // Arrange
            var renderer = new TestRenderer();
            var component = new NestedAsyncComponent();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            var log = new ConcurrentQueue<(int id, NestedAsyncComponent.EventType @event)>();
            await renderer.InvokeAsync(() => renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [EventActionsName] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnInit),
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnInitAsyncAsync, async:true),
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnParametersSet),
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnParametersSetAsyncAsync, async: true),
                    },
                    [1] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnInit),
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnInitAsyncAsync, async:true),
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnParametersSet),
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnParametersSetAsyncAsync),
                    }
                },
                [WhatToRenderName] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(new[] { 1 }),
                    [1] = CreateRenderFactory(Array.Empty<int>())
                },
                [LogName] = log
            })));

            var logForParent = log.Where(l => l.id == 0).ToArray();
            var logForChild = log.Where(l => l.id == 1).ToArray();

            AssertStream(0, logForParent);
            AssertStream(1, logForChild);
        }

        [Fact]
        public async Task CanRenderAsyncComponentsWithMultipleAsyncChildren()
        {
            // Arrange
            var renderer = new TestRenderer();
            var component = new NestedAsyncComponent();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            var log = new ConcurrentQueue<(int id, NestedAsyncComponent.EventType @event)>();
            await renderer.InvokeAsync(() => renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [EventActionsName] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnInit),
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnInitAsyncAsync, async:true),
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnParametersSet),
                        NestedAsyncComponent.ExecutionAction.On(0, NestedAsyncComponent.EventType.OnParametersSetAsyncAsync, async: true),
                    },
                    [1] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnInit),
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnInitAsyncAsync, async:true),
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnParametersSet),
                        NestedAsyncComponent.ExecutionAction.On(1, NestedAsyncComponent.EventType.OnParametersSetAsyncAsync, async:true),
                    },
                    [2] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        NestedAsyncComponent.ExecutionAction.On(2, NestedAsyncComponent.EventType.OnInit),
                        NestedAsyncComponent.ExecutionAction.On(2, NestedAsyncComponent.EventType.OnInitAsyncAsync, async:true),
                        NestedAsyncComponent.ExecutionAction.On(2, NestedAsyncComponent.EventType.OnParametersSet),
                        NestedAsyncComponent.ExecutionAction.On(2, NestedAsyncComponent.EventType.OnParametersSetAsyncAsync, async:true),
                    },
                    [3] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        NestedAsyncComponent.ExecutionAction.On(3, NestedAsyncComponent.EventType.OnInit),
                        NestedAsyncComponent.ExecutionAction.On(3, NestedAsyncComponent.EventType.OnInitAsyncAsync, async:true),
                        NestedAsyncComponent.ExecutionAction.On(3, NestedAsyncComponent.EventType.OnParametersSet),
                        NestedAsyncComponent.ExecutionAction.On(3, NestedAsyncComponent.EventType.OnParametersSetAsyncAsync, async:true),
                    }
                },
                [WhatToRenderName] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(new[] { 1, 2 }),
                    [1] = CreateRenderFactory(new[] { 3 }),
                    [2] = CreateRenderFactory(Array.Empty<int>()),
                    [3] = CreateRenderFactory(Array.Empty<int>())
                },
                [LogName] = log
            })));

            var logForParent = log.Where(l => l.id == 0).ToArray();
            var logForFirstChild = log.Where(l => l.id == 1).ToArray();
            var logForSecondChild = log.Where(l => l.id == 2).ToArray();
            var logForThirdChild = log.Where(l => l.id == 3).ToArray();

            AssertStream(0, logForParent);
            AssertStream(1, logForFirstChild);
            AssertStream(2, logForSecondChild);
            AssertStream(3, logForThirdChild);
        }

        [Fact]
        public void DispatchingEventsWithoutAsyncWorkShouldCompleteSynchronously()
        {
            // Arrange: Render a component with an event handler
            var renderer = new TestRenderer();
            UIEventArgs receivedArgs = null;

            var component = new EventComponent
            {
                OnTest = args => { receivedArgs = args; }
            };
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();

            var eventHandlerId = renderer.Batches.Single()
                .ReferenceFrames
                .First(frame => frame.AttributeValue != null)
                .AttributeEventHandlerId;

            // Assert: Event not yet fired
            Assert.Null(receivedArgs);

            // Act/Assert: Event can be fired
            var eventArgs = new UIEventArgs();
            var task = renderer.DispatchEventAsync(eventHandlerId, eventArgs);

            // This should always be run synchronously
            Assert.True(task.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task CanDispatchEventsToTopLevelComponents()
        {
            // Arrange: Render a component with an event handler
            var renderer = new TestRenderer();
            UIEventArgs receivedArgs = null;

            var component = new EventComponent
            {
                OnTest = args => { receivedArgs = args; }
            };
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();

            var eventHandlerId = renderer.Batches.Single()
                .ReferenceFrames
                .First(frame => frame.AttributeValue != null)
                .AttributeEventHandlerId;

            // Assert: Event not yet fired
            Assert.Null(receivedArgs);

            // Act/Assert: Event can be fired
            var eventArgs = new UIEventArgs();
            var renderTask = renderer.DispatchEventAsync(eventHandlerId, eventArgs);
            Assert.True(renderTask.IsCompletedSuccessfully);
            Assert.Same(eventArgs, receivedArgs);

            await renderTask; // Does not throw
        }

        [Fact]
        public async Task CanDispatchTypedEventsToTopLevelComponents()
        {
            // Arrange: Render a component with an event handler
            var renderer = new TestRenderer();
            UIMouseEventArgs receivedArgs = null;

            var component = new EventComponent
            {
                OnClick = args => { receivedArgs = args; }
            };
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();

            var eventHandlerId = renderer.Batches.Single()
                .ReferenceFrames
                .First(frame => frame.AttributeValue != null)
                .AttributeEventHandlerId;

            // Assert: Event not yet fired
            Assert.Null(receivedArgs);

            // Act/Assert: Event can be fired
            var eventArgs = new UIMouseEventArgs();
            var renderTask = renderer.DispatchEventAsync(eventHandlerId, eventArgs);
            Assert.True(renderTask.IsCompletedSuccessfully);
            Assert.Same(eventArgs, receivedArgs);

            await renderTask; // does not throw
        }

        [Fact]
        public async Task CanDispatchActionEventsToTopLevelComponents()
        {
            // Arrange: Render a component with an event handler
            var renderer = new TestRenderer();
            object receivedArgs = null;

            var component = new EventComponent
            {
                OnClickAction = () => { receivedArgs = new object(); }
            };
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();

            var eventHandlerId = renderer.Batches.Single()
                .ReferenceFrames
                .First(frame => frame.AttributeValue != null)
                .AttributeEventHandlerId;

            // Assert: Event not yet fired
            Assert.Null(receivedArgs);

            // Act/Assert: Event can be fired
            var eventArgs = new UIMouseEventArgs();
            var renderTask = renderer.DispatchEventAsync(eventHandlerId, eventArgs);
            Assert.True(renderTask.IsCompletedSuccessfully);
            Assert.NotNull(receivedArgs);

            await renderTask; // does not throw
        }

        [Fact]
        public async Task CanDispatchEventsToNestedComponents()
        {
            UIEventArgs receivedArgs = null;

            // Arrange: Render parent component
            var renderer = new TestRenderer();
            var parentComponent = new TestComponent(builder =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.CloseComponent();
            });
            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            parentComponent.TriggerRender();

            // Arrange: Render nested component
            var nestedComponentFrame = renderer.Batches.Single()
                .ReferenceFrames
                .Single(frame => frame.FrameType == RenderTreeFrameType.Component);
            var nestedComponent = (EventComponent)nestedComponentFrame.Component;
            nestedComponent.OnTest = args => { receivedArgs = args; };
            var nestedComponentId = nestedComponentFrame.ComponentId;
            nestedComponent.TriggerRender();

            // Find nested component's event handler ID
            var eventHandlerId = renderer.Batches[1]
                .ReferenceFrames
                .First(frame => frame.AttributeValue != null)
                .AttributeEventHandlerId;

            // Assert: Event not yet fired
            Assert.Null(receivedArgs);

            // Act/Assert: Event can be fired
            var eventArgs = new UIEventArgs();
            var renderTask = renderer.DispatchEventAsync(eventHandlerId, eventArgs);
            Assert.True(renderTask.IsCompletedSuccessfully);
            Assert.Same(eventArgs, receivedArgs);

            await renderTask; // does not throw
        }

        [Fact]
        public async Task CanAsyncDispatchEventsToTopLevelComponents()
        {
            // Arrange: Render a component with an event handler
            var renderer = new TestRenderer();
            UIEventArgs receivedArgs = null;

            var state = 0;
            var tcs = new TaskCompletionSource<object>();

            var component = new EventComponent
            {
                OnTestAsync = async (args) =>
                {
                    receivedArgs = args;
                    state = 1;
                    await tcs.Task;
                    state = 2;
                },
            };
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();

            var eventHandlerId = renderer.Batches.Single()
                .ReferenceFrames
                .First(frame => frame.AttributeValue != null)
                .AttributeEventHandlerId;

            // Assert: Event not yet fired
            Assert.Null(receivedArgs);

            // Act/Assert: Event can be fired
            var eventArgs = new UIEventArgs();
            var task = renderer.DispatchEventAsync(eventHandlerId, eventArgs);
            Assert.Equal(1, state);
            Assert.Same(eventArgs, receivedArgs);

            tcs.SetResult(null);
            await task;

            Assert.Equal(2, state);
        }

        [Fact]
        public async Task CanAsyncDispatchTypedEventsToTopLevelComponents()
        {
            // Arrange: Render a component with an event handler
            var renderer = new TestRenderer();
            UIMouseEventArgs receivedArgs = null;

            var state = 0;
            var tcs = new TaskCompletionSource<object>();

            var component = new EventComponent
            {
                OnClickAsync = async (args) =>
                {
                    receivedArgs = args;
                    state = 1;
                    await tcs.Task;
                    state = 2;
                }
            };
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();

            var eventHandlerId = renderer.Batches.Single()
                .ReferenceFrames
                .First(frame => frame.AttributeValue != null)
                .AttributeEventHandlerId;

            // Assert: Event not yet fired
            Assert.Null(receivedArgs);

            // Act/Assert: Event can be fired
            var eventArgs = new UIMouseEventArgs();
            var task = renderer.DispatchEventAsync(eventHandlerId, eventArgs);
            Assert.Equal(1, state);
            Assert.Same(eventArgs, receivedArgs);

            tcs.SetResult(null);
            await task;

            Assert.Equal(2, state);
        }

        [Fact]
        public async Task CanAsyncDispatchActionEventsToTopLevelComponents()
        {
            // Arrange: Render a component with an event handler
            var renderer = new TestRenderer();
            object receivedArgs = null;

            var state = 0;
            var tcs = new TaskCompletionSource<object>();

            var component = new EventComponent
            {
                OnClickAsyncAction = async () =>
                {
                    receivedArgs = new object();
                    state = 1;
                    await tcs.Task;
                    state = 2;
                }
            };
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();

            var eventHandlerId = renderer.Batches.Single()
                .ReferenceFrames
                .First(frame => frame.AttributeValue != null)
                .AttributeEventHandlerId;

            // Assert: Event not yet fired
            Assert.Null(receivedArgs);

            // Act/Assert: Event can be fired
            var eventArgs = new UIMouseEventArgs();
            var task = renderer.DispatchEventAsync(eventHandlerId, eventArgs);
            Assert.Equal(1, state);
            Assert.NotNull(receivedArgs);

            tcs.SetResult(null);
            await task;

            Assert.Equal(2, state);
        }

        [Fact]
        public async Task CanAsyncDispatchEventsToNestedComponents()
        {
            UIEventArgs receivedArgs = null;

            var state = 0;
            var tcs = new TaskCompletionSource<object>();

            // Arrange: Render parent component
            var renderer = new TestRenderer();
            var parentComponent = new TestComponent(builder =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.CloseComponent();
            });
            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            parentComponent.TriggerRender();

            // Arrange: Render nested component
            var nestedComponentFrame = renderer.Batches.Single()
                .ReferenceFrames
                .Single(frame => frame.FrameType == RenderTreeFrameType.Component);
            var nestedComponent = (EventComponent)nestedComponentFrame.Component;
            nestedComponent.OnTestAsync = async (args) =>
            {
                receivedArgs = args;
                state = 1;
                await tcs.Task;
                state = 2;
            };
            var nestedComponentId = nestedComponentFrame.ComponentId;
            nestedComponent.TriggerRender();

            // Find nested component's event handler ID
            var eventHandlerId = renderer.Batches[1]
                .ReferenceFrames
                .First(frame => frame.AttributeValue != null)
                .AttributeEventHandlerId;

            // Assert: Event not yet fired
            Assert.Null(receivedArgs);

            // Act/Assert: Event can be fired
            var eventArgs = new UIEventArgs();
            var task = renderer.DispatchEventAsync(eventHandlerId, eventArgs);
            Assert.Equal(1, state);
            Assert.Same(eventArgs, receivedArgs);

            tcs.SetResult(null);
            await task;

            Assert.Equal(2, state);
        }

        // This tests the behaviour of dispatching an event when the event-handler
        // delegate is a bound-delegate with a target that points to the parent component.
        //
        // This is a very common case when a component accepts a delegate parameter that
        // will be hooked up to a DOM event handler. It's essential that this will dispatch
        // to the parent component so that manual StateHasChanged calls are not necessary.
        [Fact]
        public async Task EventDispatching_DelegateParameter_MethodToDelegateConversion()
        {
            // Arrange
            var outerStateChangeCount = 0;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickAction), parentComponent.SomeMethod);
                builder.CloseComponent();
            };
            parentComponent.OnEvent = () =>
            {
                outerStateChangeCount++;
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclickaction")
                .AttributeEventHandlerId;

            // Act
            var eventArgs = new UIMouseEventArgs();
            await renderer.DispatchEventAsync(eventHandlerId, eventArgs);

            // Assert
            Assert.Equal(1, parentComponent.SomeMethodCallCount);
            Assert.Equal(1, outerStateChangeCount);
        }

        // This is the inverse case of EventDispatching_DelegateParameter_MethodToDelegateConversion
        // where the event-handling delegate has a target that is not a component.
        //
        // This is a degenerate case that we don't expect to occur in applications often,
        // but it's important to verify the semantics.
        [Fact]
        public async Task EventDispatching_DelegateParameter_NoTargetLambda()
        {
            // Arrange
            var outerStateChangeCount = 0;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickAction), () =>
                {
                    parentComponent.SomeMethod();
                });
                builder.CloseComponent();
            };
            parentComponent.OnEvent = () =>
            {
                outerStateChangeCount++;
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclickaction")
                .AttributeEventHandlerId;

            // Act
            var eventArgs = new UIMouseEventArgs();
            await renderer.DispatchEventAsync(eventHandlerId, eventArgs);

            // Assert
            Assert.Equal(1, parentComponent.SomeMethodCallCount);
            Assert.Equal(0, outerStateChangeCount);
        }

        // This is a similar case to EventDispatching_DelegateParameter_MethodToDelegateConversion
        // but uses our event handling infrastructure to achieve the same effect. The call to CreateDelegate
        // is not necessary for correctness in this case - it should just no op.
        [Fact]
        public async Task EventDispatching_EventCallback_MethodToDelegateConversion()
        {
            // Arrange
            var outerStateChangeCount = 0;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallback), EventCallback.Factory.Create(parentComponent, (Action)parentComponent.SomeMethod));
                builder.CloseComponent();
            };
            parentComponent.OnEvent = () =>
            {
                outerStateChangeCount++;
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var eventArgs = new UIMouseEventArgs();
            await renderer.DispatchEventAsync(eventHandlerId, eventArgs);

            // Assert
            Assert.Equal(1, parentComponent.SomeMethodCallCount);
            Assert.Equal(1, outerStateChangeCount);
        }

        // This is a similar case to EventDispatching_DelegateParameter_NoTargetLambda but it uses
        // our event-handling infrastructure to avoid the need for a manual StateHasChanged()
        [Fact]
        public async Task EventDispatching_EventCallback_NoTargetLambda()
        {
            // Arrange
            var outerStateChangeCount = 0;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallback), EventCallback.Factory.Create(parentComponent, (Action)(() =>
                {
                    parentComponent.SomeMethod();
                })));
                builder.CloseComponent();
            };
            parentComponent.OnEvent = () =>
            {
                outerStateChangeCount++;
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var eventArgs = new UIMouseEventArgs();
            await renderer.DispatchEventAsync(eventHandlerId, eventArgs);

            // Assert
            Assert.Equal(1, parentComponent.SomeMethodCallCount);
            Assert.Equal(1, outerStateChangeCount);
        }

        // This is a similar case to EventDispatching_DelegateParameter_NoTargetLambda but it uses
        // our event-handling infrastructure to avoid the need for a manual StateHasChanged()
        [Fact]
        public async Task EventDispatching_EventCallback_AsyncNoTargetLambda()
        {
            // Arrange
            var outerStateChangeCount = 0;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallback), EventCallback.Factory.Create(parentComponent, (Func<Task>)(() =>
                {
                    parentComponent.SomeMethod();
                    return Task.CompletedTask;
                })));
                builder.CloseComponent();
            };
            parentComponent.OnEvent = () =>
            {
                outerStateChangeCount++;
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var eventArgs = new UIMouseEventArgs();
            await renderer.DispatchEventAsync(eventHandlerId, eventArgs);

            // Assert
            Assert.Equal(1, parentComponent.SomeMethodCallCount);
            Assert.Equal(1, outerStateChangeCount);
        }

        [Fact]
        public async Task EventDispatching_EventCallbackOfT_MethodToDelegateConversion()
        {
            // Arrange
            var outerStateChangeCount = 0;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallbackOfT), EventCallback.Factory.Create<UIMouseEventArgs>(parentComponent, (Action)parentComponent.SomeMethod));
                builder.CloseComponent();
            };
            parentComponent.OnEvent = () =>
            {
                outerStateChangeCount++;
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var eventArgs = new UIMouseEventArgs();
            await renderer.DispatchEventAsync(eventHandlerId, eventArgs);

            // Assert
            Assert.Equal(1, parentComponent.SomeMethodCallCount);
            Assert.Equal(1, outerStateChangeCount);
        }

        // This is a similar case to EventDispatching_DelegateParameter_NoTargetLambda but it uses
        // our event-handling infrastructure to avoid the need for a manual StateHasChanged()
        [Fact]
        public async Task EventDispatching_EventCallbackOfT_NoTargetLambda()
        {
            // Arrange
            var outerStateChangeCount = 0;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallbackOfT), EventCallback.Factory.Create<UIMouseEventArgs>(parentComponent, (Action)(() =>
                {
                    parentComponent.SomeMethod();
                })));
                builder.CloseComponent();
            };
            parentComponent.OnEvent = () =>
            {
                outerStateChangeCount++;
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var eventArgs = new UIMouseEventArgs();
            await renderer.DispatchEventAsync(eventHandlerId, eventArgs);

            // Assert
            Assert.Equal(1, parentComponent.SomeMethodCallCount);
            Assert.Equal(1, outerStateChangeCount);
        }

        // This is a similar case to EventDispatching_DelegateParameter_NoTargetLambda but it uses
        // our event-handling infrastructure to avoid the need for a manual StateHasChanged()
        [Fact]
        public async Task EventDispatching_EventCallbackOfT_AsyncNoTargetLambda()
        {
            // Arrange
            var outerStateChangeCount = 0;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallbackOfT), EventCallback.Factory.Create<UIMouseEventArgs>(parentComponent, (Func<Task>)(() =>
                {
                    parentComponent.SomeMethod();
                    return Task.CompletedTask;
                })));
                builder.CloseComponent();
            };
            parentComponent.OnEvent = () =>
            {
                outerStateChangeCount++;
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var eventArgs = new UIMouseEventArgs();
            await renderer.DispatchEventAsync(eventHandlerId, eventArgs);

            // Assert
            Assert.Equal(1, parentComponent.SomeMethodCallCount);
            Assert.Equal(1, outerStateChangeCount);
        }

        [Fact]
        public async Task DispatchEventAsync_Delegate_SynchronousCompletion()
        {
            // Arrange
            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickAction), () =>
                {
                    // Do nothing.
                });
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclickaction")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            await task; // Does not throw
        }

        [Fact]
        public async Task DispatchEventAsync_EventCallback_SynchronousCompletion()
        {
            // Arrange
            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallback), EventCallback.Factory.Create(parentComponent, (Action)(() =>
                {
                    // Do nothing.
                })));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            await task; // Does not throw
        }

        [Fact]
        public async Task DispatchEventAsync_EventCallbackOfT_SynchronousCompletion()
        {
            // Arrange
            UIMouseEventArgs arg = null;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallbackOfT), EventCallback.Factory.Create(parentComponent, (Action<UIMouseEventArgs>)((e) =>
                {
                    arg = e;
                })));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.NotNull(arg);
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            await task; // Does not throw
        }

        [Fact]
        public async Task DispatchEventAsync_Delegate_SynchronousCancellation()
        {
            // Arrange
            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickAction), (Action)(() =>
                {
                    throw new OperationCanceledException();
                }));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclickaction")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.Equal(TaskStatus.Canceled, task.Status);
            await Assert.ThrowsAsync<TaskCanceledException>(() => task);
        }

        [Fact]
        public async Task DispatchEventAsync_EventCallback_SynchronousCancellation()
        {
            // Arrange
            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallback), EventCallback.Factory.Create(parentComponent, (Action)(() =>
                {
                    throw new OperationCanceledException();
                })));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.Equal(TaskStatus.Canceled, task.Status);
            await Assert.ThrowsAsync<TaskCanceledException>(() => task);
        }

        [Fact]
        public async Task DispatchEventAsync_EventCallbackOfT_SynchronousCancellation()
        {
            // Arrange
            UIMouseEventArgs arg = null;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallbackOfT), EventCallback.Factory.Create(parentComponent, (Action<UIMouseEventArgs>)((e) =>
                {
                    arg = e;
                    throw new OperationCanceledException();
                })));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.NotNull(arg);
            Assert.Equal(TaskStatus.Canceled, task.Status);
            await Assert.ThrowsAsync<TaskCanceledException>(() => task);
        }

        [Fact]
        public async Task DispatchEventAsync_Delegate_SynchronousException()
        {
            // Arrange
            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickAction), (Action)(() =>
                {
                    throw new InvalidTimeZoneException();
                }));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclickaction")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.Equal(TaskStatus.Faulted, task.Status);
            await Assert.ThrowsAsync<InvalidTimeZoneException>(() => task);
        }

        [Fact]
        public async Task DispatchEventAsync_EventCallback_SynchronousException()
        {
            // Arrange
            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallback), EventCallback.Factory.Create(parentComponent, (Action)(() =>
                {
                    throw new InvalidTimeZoneException();
                })));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.Equal(TaskStatus.Faulted, task.Status);
            await Assert.ThrowsAsync<InvalidTimeZoneException>(() => task);
        }

        [Fact]
        public async Task DispatchEventAsync_EventCallbackOfT_SynchronousException()
        {
            // Arrange
            UIMouseEventArgs arg = null;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallbackOfT), EventCallback.Factory.Create<UIMouseEventArgs>(parentComponent, (Action<UIMouseEventArgs>)((e) =>
                {
                    arg = e;
                    throw new InvalidTimeZoneException();
                })));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.NotNull(arg);
            Assert.Equal(TaskStatus.Faulted, task.Status);
            await Assert.ThrowsAsync<InvalidTimeZoneException>(() => task);
        }

        [Fact]
        public async Task DispatchEventAsync_Delegate_AsynchronousCompletion()
        {
            // Arrange
            var tcs = new TaskCompletionSource<object>();

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickAsyncAction), async () =>
                {
                    await tcs.Task;
                });
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclickaction")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
            tcs.SetResult(null);
            await task; // Does not throw
        }

        [Fact]
        public async Task DispatchEventAsync_EventCallback_AsynchronousCompletion()
        {
            // Arrange
            var tcs = new TaskCompletionSource<object>();

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallback), EventCallback.Factory.Create(parentComponent, async () =>
                {
                    await tcs.Task;
                }));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
            tcs.SetResult(null);
            await task; // Does not throw
        }

        [Fact]
        public async Task DispatchEventAsync_EventCallbackOfT_AsynchronousCompletion()
        {
            // Arrange
            var tcs = new TaskCompletionSource<object>();

            UIMouseEventArgs arg = null;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallbackOfT), EventCallback.Factory.Create<UIMouseEventArgs>(parentComponent, async (e) =>
                {
                    arg = e;
                    await tcs.Task;
                }));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.NotNull(arg);
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
            tcs.SetResult(null);
            await task; // Does not throw
        }

        [Fact]
        public async Task DispatchEventAsync_Delegate_AsynchronousCancellation()
        {
            // Arrange
            var tcs = new TaskCompletionSource<object>();

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickAsyncAction), async () =>
                {
                    await tcs.Task;
                    throw new TaskCanceledException();
                });
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclickaction")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
            tcs.SetResult(null);

            await task; // Does not throw
            Assert.Empty(renderer.HandledExceptions);
        }

        [Fact]
        public async Task DispatchEventAsync_EventCallback_AsynchronousCancellation()
        {
            // Arrange
            var tcs = new TaskCompletionSource<object>();

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallback), EventCallback.Factory.Create(parentComponent, async () =>
                {
                    await tcs.Task;
                    throw new TaskCanceledException();
                }));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
            tcs.SetResult(null);

            await task; // Does not throw
            Assert.Empty(renderer.HandledExceptions);
        }

        [Fact]
        public async Task DispatchEventAsync_EventCallbackOfT_AsynchronousCancellation()
        {
            // Arrange
            var tcs = new TaskCompletionSource<object>();

            UIMouseEventArgs arg = null;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallbackOfT), EventCallback.Factory.Create<UIMouseEventArgs>(parentComponent, async (e) =>
                {
                    arg = e;
                    await tcs.Task;
                    throw new TaskCanceledException();
                }));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.NotNull(arg);
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
            tcs.SetResult(null);

            await task; // Does not throw
            Assert.Empty(renderer.HandledExceptions);
        }

        [Fact]
        public async Task DispatchEventAsync_Delegate_AsynchronousException()
        {
            // Arrange
            var tcs = new TaskCompletionSource<object>();

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickAsyncAction), async () =>
                {
                    await tcs.Task;
                    throw new InvalidTimeZoneException();
                });
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclickaction")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
            tcs.SetResult(null);

            await Assert.ThrowsAsync<InvalidTimeZoneException>(() => task);
        }

        [Fact]
        public async Task DispatchEventAsync_EventCallback_AsynchronousException()
        {
            // Arrange
            var tcs = new TaskCompletionSource<object>();

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallback), EventCallback.Factory.Create(parentComponent, async () =>
                {
                    await tcs.Task;
                    throw new InvalidTimeZoneException();
                }));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
            tcs.SetResult(null);

            await Assert.ThrowsAsync<InvalidTimeZoneException>(() => task);
        }

        [Fact]
        public async Task DispatchEventAsync_EventCallbackOfT_AsynchronousException()
        {
            // Arrange
            var tcs = new TaskCompletionSource<object>();

            UIMouseEventArgs arg = null;

            var renderer = new TestRenderer();
            var parentComponent = new OuterEventComponent();
            parentComponent.RenderFragment = (builder) =>
            {
                builder.OpenComponent<EventComponent>(0);
                builder.AddAttribute(1, nameof(EventComponent.OnClickEventCallbackOfT), EventCallback.Factory.Create<UIMouseEventArgs>(parentComponent, async (e) =>
                {
                    arg = e;
                    await tcs.Task;
                    throw new InvalidTimeZoneException();
                }));
                builder.CloseComponent();
            };

            var parentComponentId = renderer.AssignRootComponentId(parentComponent);
            await parentComponent.TriggerRenderAsync();

            var eventHandlerId = renderer.Batches[0]
                .ReferenceFrames
                .First(frame => frame.AttributeName == "onclick")
                .AttributeEventHandlerId;

            // Act
            var task = renderer.DispatchEventAsync(eventHandlerId, new UIMouseEventArgs());

            // Assert
            Assert.NotNull(arg);
            Assert.Equal(TaskStatus.WaitingForActivation, task.Status);
            tcs.SetResult(null);

            await Assert.ThrowsAsync<InvalidTimeZoneException>(() => task);
        }

        [Fact]
        public async Task CannotDispatchEventsWithUnknownEventHandlers()
        {
            // Arrange
            var renderer = new TestRenderer();

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                return renderer.DispatchEventAsync(0, new UIEventArgs());
            });
        }

        [Fact]
        public void ComponentsCanBeAssociatedWithMultipleRenderers()
        {
            // Arrange
            var renderer1 = new TestRenderer();
            var renderer2 = new TestRenderer();
            var component = new MultiRendererComponent();
            var renderer1ComponentId = renderer1.AssignRootComponentId(component);
            renderer2.AssignRootComponentId(new TestComponent(null)); // Just so they don't get the same IDs
            var renderer2ComponentId = renderer2.AssignRootComponentId(component);

            // Act/Assert
            component.TriggerRender();
            var renderer1Batch = renderer1.Batches.Single();
            var renderer1Diff = renderer1Batch.DiffsByComponentId[renderer1ComponentId].Single();
            Assert.Collection(renderer1Diff.Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.PrependFrame, edit.Type);
                    AssertFrame.Text(renderer1Batch.ReferenceFrames[edit.ReferenceFrameIndex],
                        $"Hello from {nameof(MultiRendererComponent)}", 0);
                });

            var renderer2Batch = renderer2.Batches.Single();
            var renderer2Diff = renderer2Batch.DiffsByComponentId[renderer2ComponentId].Single();
            Assert.Collection(renderer2Diff.Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.PrependFrame, edit.Type);
                    AssertFrame.Text(renderer2Batch.ReferenceFrames[edit.ReferenceFrameIndex],
                        $"Hello from {nameof(MultiRendererComponent)}", 0);
                });
        }

        [Fact]
        public void PreservesChildComponentInstancesWithNoAttributes()
        {
            // Arrange: First render, capturing child component instance
            var renderer = new TestRenderer();
            var message = "Hello";
            var component = new TestComponent(builder =>
            {
                builder.AddContent(0, message);
                builder.OpenComponent<MessageComponent>(1);
                builder.CloseComponent();
            });

            var rootComponentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();

            var nestedComponentFrame = renderer.Batches.Single()
                .ReferenceFrames
                .Single(frame => frame.FrameType == RenderTreeFrameType.Component);
            var nestedComponentInstance = (MessageComponent)nestedComponentFrame.Component;

            // Act: Second render
            message = "Modified message";
            component.TriggerRender();

            // Assert
            var batch = renderer.Batches[1];
            var diff = batch.DiffsByComponentId[rootComponentId].Single();
            Assert.Collection(diff.Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.UpdateText, edit.Type);
                    Assert.Equal(0, edit.ReferenceFrameIndex);
                });
            AssertFrame.Text(batch.ReferenceFrames[0], "Modified message");
            Assert.False(batch.DiffsByComponentId.ContainsKey(nestedComponentFrame.ComponentId));
        }

        [Fact]
        public void UpdatesPropertiesOnRetainedChildComponentInstances()
        {
            // Arrange: First render, capturing child component instance
            var renderer = new TestRenderer();
            var objectThatWillNotChange = new object();
            var firstRender = true;
            var component = new TestComponent(builder =>
            {
                builder.OpenComponent<FakeComponent>(1);
                builder.AddAttribute(2, nameof(FakeComponent.IntProperty), firstRender ? 123 : 256);
                builder.AddAttribute(3, nameof(FakeComponent.ObjectProperty), objectThatWillNotChange);
                builder.AddAttribute(4, nameof(FakeComponent.StringProperty), firstRender ? "String that will change" : "String that did change");
                builder.CloseComponent();
            });

            var rootComponentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();

            var originalComponentFrame = renderer.Batches.Single()
                .ReferenceFrames
                .Single(frame => frame.FrameType == RenderTreeFrameType.Component);
            var childComponentInstance = (FakeComponent)originalComponentFrame.Component;

            // Assert 1: properties were assigned
            Assert.Equal(123, childComponentInstance.IntProperty);
            Assert.Equal("String that will change", childComponentInstance.StringProperty);
            Assert.Same(objectThatWillNotChange, childComponentInstance.ObjectProperty);

            // Act: Second render
            firstRender = false;
            component.TriggerRender();

            // Assert
            Assert.Equal(256, childComponentInstance.IntProperty);
            Assert.Equal("String that did change", childComponentInstance.StringProperty);
            Assert.Same(objectThatWillNotChange, childComponentInstance.ObjectProperty);
        }

        [Fact]
        public void ReRendersChildComponentsWhenPropertiesChange()
        {
            // Arrange: First render
            var renderer = new TestRenderer();
            var firstRender = true;
            var component = new TestComponent(builder =>
            {
                builder.OpenComponent<MessageComponent>(1);
                builder.AddAttribute(2, nameof(MessageComponent.Message), firstRender ? "first" : "second");
                builder.CloseComponent();
            });

            var rootComponentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();

            var childComponentId = renderer.Batches.Single()
                .ReferenceFrames
                .Single(frame => frame.FrameType == RenderTreeFrameType.Component)
                .ComponentId;

            // Act: Second render
            firstRender = false;
            component.TriggerRender();
            var diff = renderer.Batches[1].DiffsByComponentId[childComponentId].Single();

            // Assert
            Assert.Collection(diff.Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.UpdateText, edit.Type);
                    Assert.Equal(0, edit.ReferenceFrameIndex);
                });
            AssertFrame.Text(renderer.Batches[1].ReferenceFrames[0], "second");
        }

        [Fact]
        public void RenderBatchIncludesListOfDisposedComponents()
        {
            // Arrange
            var renderer = new TestRenderer();
            var firstRender = true;
            var component = new TestComponent(builder =>
            {
                if (firstRender)
                {
                    // Nested descendants
                    builder.OpenComponent<ConditionalParentComponent<FakeComponent>>(100);
                    builder.AddAttribute(101, nameof(ConditionalParentComponent<FakeComponent>.IncludeChild), true);
                    builder.CloseComponent();
                }
                builder.OpenComponent<FakeComponent>(200);
                builder.CloseComponent();
            });

            var rootComponentId = renderer.AssignRootComponentId(component);

            // Act/Assert 1: First render, capturing child component IDs
            component.TriggerRender();
            var batch = renderer.Batches.Single();
            var rootComponentDiff = batch.DiffsByComponentId[rootComponentId].Single();
            var childComponentIds = rootComponentDiff
                .Edits
                .Select(edit => batch.ReferenceFrames[edit.ReferenceFrameIndex])
                .Where(frame => frame.FrameType == RenderTreeFrameType.Component)
                .Select(frame => frame.ComponentId)
                .ToList();
            var childComponent3 = batch.ReferenceFrames.Where(f => f.ComponentId == 3)
                .Single().Component;
            Assert.Equal(new[] { 1, 2 }, childComponentIds);
            Assert.IsType<FakeComponent>(childComponent3);

            // Act: Second render
            firstRender = false;
            component.TriggerRender();

            // Assert: Applicable children are included in disposal list
            Assert.Equal(2, renderer.Batches.Count);
            Assert.Equal(new[] { 1, 3 }, renderer.Batches[1].DisposedComponentIDs);

            // Act/Assert: If a disposed component requests a render, it's a no-op
            var renderHandle = ((FakeComponent)childComponent3).RenderHandle;
            renderHandle.Invoke(() => renderHandle.Render(builder
                => throw new NotImplementedException("Should not be invoked")));
            Assert.Equal(2, renderer.Batches.Count);
        }

        [Fact]
        public async Task DisposesEventHandlersWhenAttributeValueChanged()
        {
            // Arrange
            var renderer = new TestRenderer();
            var eventCount = 0;
            Action<UIEventArgs> origEventHandler = args => { eventCount++; };
            var component = new EventComponent { OnTest = origEventHandler };
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();
            var origEventHandlerId = renderer.Batches.Single()
                .ReferenceFrames
                .Where(f => f.FrameType == RenderTreeFrameType.Attribute)
                .Single(f => f.AttributeEventHandlerId != 0)
                .AttributeEventHandlerId;

            // Act/Assert 1: Event handler fires when we trigger it
            Assert.Equal(0, eventCount);
            var renderTask = renderer.DispatchEventAsync(origEventHandlerId, args: null);
            Assert.True(renderTask.IsCompletedSuccessfully);
            Assert.Equal(1, eventCount);
            await renderTask;

            // Now change the attribute value
            var newEventCount = 0;
            component.OnTest = args => { newEventCount++; };
            component.TriggerRender();

            // Act/Assert 2: Can no longer fire the original event, but can fire the new event
            await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                return renderer.DispatchEventAsync(origEventHandlerId, args: null);
            });

            Assert.Equal(1, eventCount);
            Assert.Equal(0, newEventCount);
            renderTask = renderer.DispatchEventAsync(origEventHandlerId + 1, args: null);
            Assert.True(renderTask.IsCompletedSuccessfully);
            Assert.Equal(1, newEventCount);
            await renderTask;
        }

        [Fact]
        public async Task DisposesEventHandlersWhenAttributeRemoved()
        {
            // Arrange
            var renderer = new TestRenderer();
            var eventCount = 0;
            Action<UIEventArgs> origEventHandler = args => { eventCount++; };
            var component = new EventComponent { OnTest = origEventHandler };
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();
            var origEventHandlerId = renderer.Batches.Single()
                .ReferenceFrames
                .Where(f => f.FrameType == RenderTreeFrameType.Attribute)
                .Single(f => f.AttributeEventHandlerId != 0)
                .AttributeEventHandlerId;

            // Act/Assert 1: Event handler fires when we trigger it
            Assert.Equal(0, eventCount);
            var renderTask = renderer.DispatchEventAsync(origEventHandlerId, args: null);
            Assert.True(renderTask.IsCompletedSuccessfully);
            Assert.Equal(1, eventCount);
            await renderTask;

            // Now remove the event attribute
            component.OnTest = null;
            component.TriggerRender();

            // Act/Assert 2: Can no longer fire the original event
            await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                return renderer.DispatchEventAsync(origEventHandlerId, args: null);
            });
            Assert.Equal(1, eventCount);
        }

        [Fact]
        public async Task DisposesEventHandlersWhenOwnerComponentRemoved()
        {
            // Arrange
            var renderer = new TestRenderer();
            var eventCount = 0;
            Action<UIEventArgs> origEventHandler = args => { eventCount++; };
            var component = new ConditionalParentComponent<EventComponent>
            {
                IncludeChild = true,
                ChildParameters = new Dictionary<string, object>
                {
                    { nameof(EventComponent.OnTest), origEventHandler }
                }
            };
            var rootComponentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();
            var batch = renderer.Batches.Single();
            var rootComponentDiff = batch.DiffsByComponentId[rootComponentId].Single();
            var rootComponentFrame = batch.ReferenceFrames[0];
            var childComponentFrame = rootComponentDiff.Edits
                .Select(e => batch.ReferenceFrames[e.ReferenceFrameIndex])
                .Where(f => f.FrameType == RenderTreeFrameType.Component)
                .Single();
            var childComponentId = childComponentFrame.ComponentId;
            var childComponentDiff = batch.DiffsByComponentId[childComponentFrame.ComponentId].Single();
            var eventHandlerId = batch.ReferenceFrames
                .Skip(childComponentDiff.Edits[0].ReferenceFrameIndex) // Search from where the child component frames start
                .Where(f => f.FrameType == RenderTreeFrameType.Attribute)
                .Single(f => f.AttributeEventHandlerId != 0)
                .AttributeEventHandlerId;

            // Act/Assert 1: Event handler fires when we trigger it
            Assert.Equal(0, eventCount);
            var renderTask = renderer.DispatchEventAsync(eventHandlerId, args: null);
            Assert.True(renderTask.IsCompletedSuccessfully);
            Assert.Equal(1, eventCount);
            await renderTask;

            // Now remove the EventComponent
            component.IncludeChild = false;
            component.TriggerRender();

            // Act/Assert 2: Can no longer fire the original event
            await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                return renderer.DispatchEventAsync(eventHandlerId, args: null);
            });
            Assert.Equal(1, eventCount);
        }

        [Fact]
        public async Task DisposesEventHandlersWhenAncestorElementRemoved()
        {
            // Arrange
            var renderer = new TestRenderer();
            var eventCount = 0;
            Action<UIEventArgs> origEventHandler = args => { eventCount++; };
            var component = new EventComponent { OnTest = origEventHandler };
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();
            var origEventHandlerId = renderer.Batches.Single()
                .ReferenceFrames
                .Where(f => f.FrameType == RenderTreeFrameType.Attribute)
                .Single(f => f.AttributeEventHandlerId != 0)
                .AttributeEventHandlerId;

            // Act/Assert 1: Event handler fires when we trigger it
            Assert.Equal(0, eventCount);
            var renderTask = renderer.DispatchEventAsync(origEventHandlerId, args: null);
            Assert.True(renderTask.IsCompletedSuccessfully);
            Assert.Equal(1, eventCount);
            await renderTask;

            // Now remove the ancestor element
            component.SkipElement = true;
            component.TriggerRender();

            // Act/Assert 2: Can no longer fire the original event
            await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                return renderer.DispatchEventAsync(origEventHandlerId, args: null);
            });
            Assert.Equal(1, eventCount);
        }

        [Fact]
        public async Task AllRendersTriggeredSynchronouslyDuringEventHandlerAreHandledAsSingleBatch()
        {
            // Arrange: A root component with a child whose event handler explicitly queues
            // a re-render of both the root component and the child
            var renderer = new TestRenderer();
            var eventCount = 0;
            TestComponent rootComponent = null;
            EventComponent childComponent = null;
            rootComponent = new TestComponent(builder =>
            {
                builder.AddContent(0, "Child event count: " + eventCount);
                builder.OpenComponent<EventComponent>(1);
                builder.AddAttribute(2, nameof(EventComponent.OnTest), args =>
                {
                    eventCount++;
                    rootComponent.TriggerRender();
                    childComponent.TriggerRender();
                });
                builder.CloseComponent();
            });
            var rootComponentId = renderer.AssignRootComponentId(rootComponent);
            rootComponent.TriggerRender();
            var origBatchReferenceFrames = renderer.Batches.Single().ReferenceFrames;
            var childComponentFrame = origBatchReferenceFrames
                .Single(f => f.Component is EventComponent);
            var childComponentId = childComponentFrame.ComponentId;
            childComponent = (EventComponent)childComponentFrame.Component;
            var origEventHandlerId = origBatchReferenceFrames
                .Where(f => f.FrameType == RenderTreeFrameType.Attribute)
                .Last(f => f.AttributeEventHandlerId != 0)
                .AttributeEventHandlerId;
            Assert.Single(renderer.Batches);

            // Act
            var renderTask = renderer.DispatchEventAsync(origEventHandlerId, args: null);

            // Assert
            Assert.True(renderTask.IsCompletedSuccessfully);
            await renderTask;

            Assert.Equal(2, renderer.Batches.Count);
            var batch = renderer.Batches.Last();
            Assert.Collection(batch.DiffsInOrder,
                diff =>
                {
                    // First we triggered the root component to re-render
                    Assert.Equal(rootComponentId, diff.ComponentId);
                    Assert.Collection(diff.Edits, edit =>
                    {
                        Assert.Equal(RenderTreeEditType.UpdateText, edit.Type);
                        AssertFrame.Text(
                            batch.ReferenceFrames[edit.ReferenceFrameIndex],
                            "Child event count: 1");
                    });
                },
                diff =>
                {
                    // Then the root re-render will have triggered an update to the child
                    Assert.Equal(childComponentId, diff.ComponentId);
                    Assert.Collection(diff.Edits, edit =>
                    {
                        Assert.Equal(RenderTreeEditType.UpdateText, edit.Type);
                        AssertFrame.Text(
                            batch.ReferenceFrames[edit.ReferenceFrameIndex],
                            "Render count: 2");
                    });
                },
                diff =>
                {
                    // Finally we explicitly requested a re-render of the child
                    Assert.Equal(childComponentId, diff.ComponentId);
                    Assert.Collection(diff.Edits, edit =>
                    {
                        Assert.Equal(RenderTreeEditType.UpdateText, edit.Type);
                        AssertFrame.Text(
                            batch.ReferenceFrames[edit.ReferenceFrameIndex],
                            "Render count: 3");
                    });
                });
        }

        [Fact]
        public void ComponentCannotTriggerRenderBeforeRenderHandleAssigned()
        {
            // Arrange
            var component = new TestComponent(builder => { });

            // Act/Assert
            var ex = Assert.Throws<InvalidOperationException>(() => component.TriggerRender());
            Assert.Equal("The render handle is not yet assigned.", ex.Message);
        }

        [Fact]
        public void ComponentCanTriggerRenderWhenNoBatchIsInProgress()
        {
            // Arrange
            var renderer = new TestRenderer();
            var renderCount = 0;
            var component = new TestComponent(builder =>
            {
                builder.AddContent(0, $"Render count: {++renderCount}");
            });
            var componentId = renderer.AssignRootComponentId(component);

            // Act/Assert: Can trigger initial render
            Assert.Equal(0, renderCount);
            component.TriggerRender();
            Assert.Equal(1, renderCount);
            var batch1 = renderer.Batches.Single();
            var edit1 = batch1.DiffsByComponentId[componentId].Single().Edits.Single();
            Assert.Equal(RenderTreeEditType.PrependFrame, edit1.Type);
            AssertFrame.Text(batch1.ReferenceFrames[edit1.ReferenceFrameIndex],
                "Render count: 1", 0);

            // Act/Assert: Can trigger subsequent render
            component.TriggerRender();
            Assert.Equal(2, renderCount);
            var batch2 = renderer.Batches.Skip(1).Single();
            var edit2 = batch2.DiffsByComponentId[componentId].Single().Edits.Single();
            Assert.Equal(RenderTreeEditType.UpdateText, edit2.Type);
            AssertFrame.Text(batch2.ReferenceFrames[edit2.ReferenceFrameIndex],
                "Render count: 2", 0);
        }

        [Fact]
        public void ComponentCanTriggerRenderWhenExistingBatchIsInProgress()
        {
            // Arrange
            var renderer = new TestRenderer();
            TestComponent parent = null;
            var parentRenderCount = 0;
            parent = new TestComponent(builder =>
            {
                builder.OpenComponent<ReRendersParentComponent>(0);
                builder.AddAttribute(1, nameof(ReRendersParentComponent.Parent), parent);
                builder.CloseComponent();
                builder.AddContent(2, $"Parent render count: {++parentRenderCount}");
            });
            var parentComponentId = renderer.AssignRootComponentId(parent);

            // Act
            parent.TriggerRender();

            // Assert
            var batch = renderer.Batches.Single();
            Assert.Equal(4, batch.DiffsInOrder.Count);

            // First is the parent component's initial render
            var diff1 = batch.DiffsInOrder[0];
            Assert.Equal(parentComponentId, diff1.ComponentId);
            Assert.Collection(diff1.Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.PrependFrame, edit.Type);
                    AssertFrame.Component<ReRendersParentComponent>(
                        batch.ReferenceFrames[edit.ReferenceFrameIndex]);
                },
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.PrependFrame, edit.Type);
                    AssertFrame.Text(
                        batch.ReferenceFrames[edit.ReferenceFrameIndex],
                        "Parent render count: 1");
                });

            // Second is the child component's single render
            var diff2 = batch.DiffsInOrder[1];
            Assert.NotEqual(parentComponentId, diff2.ComponentId);
            var diff2edit = diff2.Edits.Single();
            Assert.Equal(RenderTreeEditType.PrependFrame, diff2edit.Type);
            AssertFrame.Text(batch.ReferenceFrames[diff2edit.ReferenceFrameIndex],
                "Child is here");

            // Third is the parent's triggered render
            var diff3 = batch.DiffsInOrder[2];
            Assert.Equal(parentComponentId, diff3.ComponentId);
            var diff3edit = diff3.Edits.Single();
            Assert.Equal(RenderTreeEditType.UpdateText, diff3edit.Type);
            AssertFrame.Text(batch.ReferenceFrames[diff3edit.ReferenceFrameIndex],
                "Parent render count: 2");

            // Fourth is child's rerender due to parent rendering
            var diff4 = batch.DiffsInOrder[3];
            Assert.NotEqual(parentComponentId, diff4.ComponentId);
            Assert.Empty(diff4.Edits);
        }

        [Fact]
        public void QueuedRenderIsSkippedIfComponentWasAlreadyDisposedInSameBatch()
        {
            // Arrange
            var renderer = new TestRenderer();
            var shouldRenderChild = true;
            TestComponent component = null;
            component = new TestComponent(builder =>
            {
                builder.AddContent(0, "Some frame so the child isn't at position zero");
                if (shouldRenderChild)
                {
                    builder.OpenComponent<RendersSelfAfterEventComponent>(1);
                    builder.AddAttribute(2, "onclick", (Action<object>)((object obj) =>
                    {
                        // First we queue (1) a re-render of the root component, then the child component
                        // will queue (2) its own re-render. But by the time (1) completes, the child will
                        // have been disposed, even though (2) is still in the queue
                        shouldRenderChild = false;
                        component.TriggerRender();
                    }));
                    builder.CloseComponent();
                }
            });

            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();
            var childComponentId = renderer.Batches.Single()
                .ReferenceFrames
                .Where(f => f.ComponentId != 0)
                .Single()
                .ComponentId;
            var origEventHandlerId = renderer.Batches.Single()
                .ReferenceFrames
                .Where(f => f.FrameType == RenderTreeFrameType.Attribute && f.AttributeName == "onclick")
                .Single(f => f.AttributeEventHandlerId != 0)
                .AttributeEventHandlerId;

            // Act
            // The fact that there's no error here is the main thing we're testing
            var renderTask = renderer.DispatchEventAsync(origEventHandlerId, args: null);

            // Assert: correct render result
            Assert.True(renderTask.IsCompletedSuccessfully);
            var newBatch = renderer.Batches.Skip(1).Single();
            Assert.Equal(1, newBatch.DisposedComponentIDs.Count);
            Assert.Equal(1, newBatch.DiffsByComponentId.Count);
            Assert.Collection(newBatch.DiffsByComponentId[componentId].Single().Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.RemoveFrame, edit.Type);
                    Assert.Equal(1, edit.SiblingIndex);
                });
        }

        [Fact]
        public async Task CanCombineBindAndConditionalAttribute()
        {
            // This test represents https://github.com/aspnet/Blazor/issues/624

            // Arrange: Rendered with textbox enabled
            var renderer = new TestRenderer();
            var component = new BindPlusConditionalAttributeComponent();
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();
            var checkboxChangeEventHandlerId = renderer.Batches.Single()
                .ReferenceFrames
                .First(frame => frame.FrameType == RenderTreeFrameType.Attribute && frame.AttributeEventHandlerId != 0)
                .AttributeEventHandlerId;

            // Act: Toggle the checkbox
            var eventArgs = new UIChangeEventArgs { Value = true };
            var renderTask =  renderer.DispatchEventAsync(checkboxChangeEventHandlerId, eventArgs);

            Assert.True(renderTask.IsCompletedSuccessfully);
            var latestBatch = renderer.Batches.Last();
            var latestDiff = latestBatch.DiffsInOrder.Single();
            var referenceFrames = latestBatch.ReferenceFrames;

            // Assert: Textbox's "disabled" attribute was removed
            Assert.Equal(2, renderer.Batches.Count);
            Assert.Equal(componentId, latestDiff.ComponentId);
            Assert.Contains(latestDiff.Edits, edit =>
                edit.SiblingIndex == 1
                && edit.RemovedAttributeName == "disabled");

            await renderTask;
        }

        [Fact]
        public void HandlesNestedElementCapturesDuringRefresh()
        {
            // This may seem like a very arbitrary test case, but at once stage there was a bug
            // whereby the diff output was incorrect given a ref capture on an element whose
            // parent element also had a ref capture

            // Arrange
            var attrValue = 0;
            var component = new TestComponent(builder =>
            {
                builder.OpenElement(0, "parent elem");
                builder.AddAttribute(1, "parent elem attr", attrValue);
                builder.AddElementReferenceCapture(2, _ => { });
                builder.OpenElement(3, "child elem");
                builder.AddElementReferenceCapture(4, _ => { });
                builder.AddContent(5, "child text");
                builder.CloseElement();
                builder.CloseElement();
            });
            var renderer = new TestRenderer();
            renderer.AssignRootComponentId(component);

            // Act: Update the attribute value on the parent
            component.TriggerRender();
            attrValue++;
            component.TriggerRender();

            // Assert
            var latestBatch = renderer.Batches.Skip(1).Single();
            var latestDiff = latestBatch.DiffsInOrder.Single();
            Assert.Collection(latestDiff.Edits,
                edit =>
                {
                    Assert.Equal(RenderTreeEditType.SetAttribute, edit.Type);
                    Assert.Equal(0, edit.SiblingIndex);
                    AssertFrame.Attribute(latestBatch.ReferenceFrames[edit.ReferenceFrameIndex],
                        "parent elem attr", 1);
                });
        }

        [Fact]
        public void CallsAfterRenderOnEachRender()
        {
            // Arrange
            var onAfterRenderCallCountLog = new List<int>();
            var component = new AfterRenderCaptureComponent();
            var renderer = new TestRenderer
            {
                OnUpdateDisplay = _ => onAfterRenderCallCountLog.Add(component.OnAfterRenderCallCount)
            };
            renderer.AssignRootComponentId(component);

            // Act
            component.TriggerRender();

            // Assert
            // When the display was first updated, OnAfterRender had not yet been called
            Assert.Equal(new[] { 0 }, onAfterRenderCallCountLog);
            // But OnAfterRender was called since then
            Assert.Equal(1, component.OnAfterRenderCallCount);

            // Act/Assert 2: On a subsequent render, the same happens again
            component.TriggerRender();
            Assert.Equal(new[] { 0, 1 }, onAfterRenderCallCountLog);
            Assert.Equal(2, component.OnAfterRenderCallCount);
        }

        [Fact]
        public void DoesNotCallOnAfterRenderForComponentsNotRendered()
        {
            // Arrange
            var showComponent3 = true;
            var parentComponent = new TestComponent(builder =>
            {
                // First child will be re-rendered because we'll change its param
                builder.OpenComponent<AfterRenderCaptureComponent>(0);
                builder.AddAttribute(1, "some param", showComponent3);
                builder.CloseComponent();

                // Second child will not be re-rendered because nothing changes
                builder.OpenComponent<AfterRenderCaptureComponent>(2);
                builder.CloseComponent();

                // Third component will be disposed
                if (showComponent3)
                {
                    builder.OpenComponent<AfterRenderCaptureComponent>(3);
                    builder.CloseComponent();
                }
            });
            var renderer = new TestRenderer();
            var parentComponentId = renderer.AssignRootComponentId(parentComponent);

            // Act: First render
            parentComponent.TriggerRender();

            // Assert: All child components were notified of "after render"
            var batch1 = renderer.Batches.Single();
            var parentComponentEdits1 = batch1.DiffsByComponentId[parentComponentId].Single().Edits;
            var childComponents = parentComponentEdits1
                .Select(
                    edit => (AfterRenderCaptureComponent)batch1.ReferenceFrames[edit.ReferenceFrameIndex].Component)
                .ToArray();
            Assert.Equal(1, childComponents[0].OnAfterRenderCallCount);
            Assert.Equal(1, childComponents[1].OnAfterRenderCallCount);
            Assert.Equal(1, childComponents[2].OnAfterRenderCallCount);

            // Act: Second render
            showComponent3 = false;
            parentComponent.TriggerRender();

            // Assert: Only the re-rendered component was notified of "after render"
            var batch2 = renderer.Batches.Skip(1).Single();
            Assert.Equal(2, batch2.DiffsInOrder.Count); // Parent and first child
            Assert.Equal(1, batch2.DisposedComponentIDs.Count); // Third child
            Assert.Equal(2, childComponents[0].OnAfterRenderCallCount); // Retained and re-rendered
            Assert.Equal(1, childComponents[1].OnAfterRenderCallCount); // Retained and not re-rendered
            Assert.Equal(1, childComponents[2].OnAfterRenderCallCount); // Disposed
        }

        [ConditionalFact]
        [SkipOnHelix] // https://github.com/aspnet/AspNetCore/issues/7487
        public async Task CanTriggerEventHandlerDisposedInEarlierPendingBatchAsync()
        {
            // This represents the scenario where the same event handler is being triggered
            // rapidly, such as an input event while typing. It only applies to asynchronous
            // batch updates, i.e., server-side Components.
            // Sequence:
            // 1. The client dispatches event X twice (say) in quick succession
            // 2. The server receives the first instance, handles the event, and re-renders
            //    some component. The act of re-rendering causes the old event handler to be
            //    replaced by a new one, so the old one is flagged to be disposed.
            // 3. The server receives the second instance. Even though the corresponding event
            //    handler is flagged to be disposed, we have to still be able to find and
            //    execute it without errors.

            // Arrange
            var renderer = new TestAsyncRenderer
            {
                NextUpdateDisplayReturnTask = Task.CompletedTask
            };
            var numEventsFired = 0;
            EventComponent component = null;
            Action<UIEventArgs> eventHandler = null;

            eventHandler = _ =>
            {
                numEventsFired++;

                // Replace the old event handler with a different one,
                // (old the old handler ID will be disposed) then re-render.
                component.OnTest = args => eventHandler(args);
                component.TriggerRender();
            };

            component = new EventComponent { OnTest = eventHandler };
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();

            var eventHandlerId = renderer.Batches.Single()
                .ReferenceFrames
                .First(frame => frame.AttributeValue != null)
                .AttributeEventHandlerId;

            // Act/Assert 1: Event can be fired for the first time
            var render1TCS = new TaskCompletionSource<object>();
            renderer.NextUpdateDisplayReturnTask = render1TCS.Task;
            await renderer.DispatchEventAsync(eventHandlerId, new UIEventArgs());
            Assert.Equal(1, numEventsFired);

            // Act/Assert 2: *Same* event handler ID can be reused prior to completion of
            // preceding UI update
            var render2TCS = new TaskCompletionSource<object>();
            renderer.NextUpdateDisplayReturnTask = render2TCS.Task;
            await renderer.DispatchEventAsync(eventHandlerId, new UIEventArgs());
            Assert.Equal(2, numEventsFired);

            // Act/Assert 3: After we complete the first UI update in which a given
            // event handler ID is disposed, we can no longer reuse that event handler ID

            // From here we can't see when the async disposal is completed. Just give it plenty of time (Task.Yield isn't enough).
            // There is a small chance in which the continuations from TaskCompletionSource run asynchronously.
            // In that case we might not be able to see the results from RemoveEventHandlerIds as they might run asynchronously.
            // For that case, we are going to queue a continuation on render1TCS.Task, include a 1s delay and await the resulting
            // task to offer the best chance that we get to see the error in all cases.
            var awaitableTask = render1TCS.Task.ContinueWith(_ => Task.Delay(1000)).Unwrap();
            render1TCS.SetResult(null);
            await awaitableTask;
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                return renderer.DispatchEventAsync(eventHandlerId, new UIEventArgs());
            });
            Assert.Equal($"There is no event handler with ID {eventHandlerId}", ex.Message);
            Assert.Equal(2, numEventsFired);
        }

        [Fact]
        public void ExceptionsThrownSynchronouslyCanBeHandledSynchronously()
        {
            // Arrange
            var renderer = new TestRenderer { ShouldHandleExceptions = true };
            var component = new NestedAsyncComponent();
            var exception = new InvalidTimeZoneException();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            var task = renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [nameof(NestedAsyncComponent.EventActions)] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = new[]
                    {
                        new NestedAsyncComponent.ExecutionAction
                        {
                            Event = NestedAsyncComponent.EventType.OnInitAsyncAsync,
                            EventAction = () => throw exception,
                        },
                    }
                },
                [nameof(NestedAsyncComponent.WhatToRender)] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(Array.Empty<int>()),
                },
            }));

            Assert.True(task.IsCompletedSuccessfully);
            Assert.Equal(new[] { exception }, renderer.HandledExceptions);
        }

        [Fact]
        public void ExceptionsThrownSynchronouslyCanBeHandled()
        {
            // Arrange
            var renderer = new TestRenderer { ShouldHandleExceptions = true };
            var component = new NestedAsyncComponent();
            var exception = new InvalidTimeZoneException();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            var renderTask = renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [nameof(NestedAsyncComponent.EventActions)] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = new[]
                    {
                        new NestedAsyncComponent.ExecutionAction
                        {
                            Event = NestedAsyncComponent.EventType.OnInitAsyncAsync,
                            EventAction = () => throw exception,
                        },
                    }
                },
                [nameof(NestedAsyncComponent.WhatToRender)] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(Array.Empty<int>()),
                },
            }));

            Assert.True(renderTask.IsCompletedSuccessfully);
            Assert.Equal(new[] { exception }, renderer.HandledExceptions);
        }

        [Fact]
        public void ExceptionsReturnedUsingTaskFromExceptionCanBeHandled()
        {
            // Arrange
            var renderer = new TestRenderer { ShouldHandleExceptions = true };
            var component = new NestedAsyncComponent();
            var exception = new InvalidTimeZoneException();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            var renderTask = renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [nameof(NestedAsyncComponent.EventActions)] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = new[]
                    {
                        new NestedAsyncComponent.ExecutionAction
                        {
                            Event = NestedAsyncComponent.EventType.OnInitAsyncAsync,
                            EventAction = () => Task.FromException<(int, NestedAsyncComponent.EventType)>(exception),
                        },
                    }
                },
                [nameof(NestedAsyncComponent.WhatToRender)] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(Array.Empty<int>()),
                },
            }));

            Assert.True(renderTask.IsCompletedSuccessfully);
            Assert.Equal(new[] { exception }, renderer.HandledExceptions);
        }

        [Fact]
        public async Task ExceptionsThrownAsynchronouslyDuringFirstRenderCanBeHandled()
        {
            // Arrange
            var renderer = new TestRenderer { ShouldHandleExceptions = true };
            var component = new NestedAsyncComponent();
            var tcs = new TaskCompletionSource<int>();
            var exception = new InvalidTimeZoneException();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            var renderTask = renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [nameof(NestedAsyncComponent.EventActions)] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = new[]
                    {
                        new NestedAsyncComponent.ExecutionAction
                        {
                            Event = NestedAsyncComponent.EventType.OnInitAsyncAsync,
                            EventAction = async () =>
                            {
                                await tcs.Task;
                                throw exception;
                            }
                        },
                    }
                },
                [nameof(NestedAsyncComponent.WhatToRender)] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(Array.Empty<int>()),
                },
            }));

            Assert.False(renderTask.IsCompleted);
            tcs.SetResult(0);
            await renderTask;
            Assert.Same(exception, Assert.Single(renderer.HandledExceptions).GetBaseException());
        }

        [Fact]
        public async Task ExceptionsThrownAsynchronouslyAfterFirstRenderCanBeHandled()
        {
            // This differs from the "during first render" case, because some aspects of the rendering
            // code paths are special cased for the first render because of prerendering.

            // Arrange
            var renderer = new TestRenderer { ShouldHandleExceptions = true };
            var taskToAwait = Task.CompletedTask;
            var component = new TestComponent(builder =>
            {
                builder.OpenComponent<ComponentThatAwaitsTask>(0);
                builder.AddAttribute(1, nameof(ComponentThatAwaitsTask.TaskToAwait), taskToAwait);
                builder.CloseComponent();
            });
            var componentId = renderer.AssignRootComponentId(component);
            await renderer.RenderRootComponentAsync(componentId); // Not throwing on first render

            var asyncExceptionTcs = new TaskCompletionSource<object>();
            taskToAwait = asyncExceptionTcs.Task;
            await renderer.Invoke(component.TriggerRender);

            // Act
            var exception = new InvalidOperationException();
            asyncExceptionTcs.SetException(exception);

            // Assert
            Assert.Same(exception, Assert.Single(renderer.HandledExceptions).GetBaseException());
        }

        [Fact]
        public async Task ExceptionsThrownAsynchronouslyFromMultipleComponentsCanBeHandled()
        {
            // Arrange
            var renderer = new TestRenderer { ShouldHandleExceptions = true };
            var component = new NestedAsyncComponent();
            var exception1 = new InvalidTimeZoneException();
            var exception2 = new UriFormatException();
            var tcs = new TaskCompletionSource<int>();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            var renderTask = renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [nameof(NestedAsyncComponent.EventActions)] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = Array.Empty<NestedAsyncComponent.ExecutionAction>(),
                    [1] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        new NestedAsyncComponent.ExecutionAction
                        {
                            Event = NestedAsyncComponent.EventType.OnInitAsyncAsync,
                            EventAction = async () =>
                            {
                                await tcs.Task;
                                throw exception1;
                            }
                        },
                    },
                    [2] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        new NestedAsyncComponent.ExecutionAction
                        {
                            Event = NestedAsyncComponent.EventType.OnInitAsyncAsync,
                            EventAction = async () =>
                            {
                                await tcs.Task;
                                throw exception2;
                            }
                        },
                    },
                },
                [nameof(NestedAsyncComponent.WhatToRender)] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(new[] { 1, 2, }),
                    [1] = CreateRenderFactory(Array.Empty<int>()),
                    [2] = CreateRenderFactory(Array.Empty<int>()),
                },
            }));

            Assert.False(renderTask.IsCompleted);
            tcs.SetResult(0);

            await renderTask;
            Assert.Equal(2, renderer.HandledExceptions.Count);
            Assert.Contains(exception1, renderer.HandledExceptions);
            Assert.Contains(exception2, renderer.HandledExceptions);
        }

        [Fact]
        public void ExceptionsThrownSynchronouslyFromMultipleComponentsCanBeHandled()
        {
            // Arrange
            var renderer = new TestRenderer { ShouldHandleExceptions = true };
            var component = new NestedAsyncComponent();
            var exception1 = new InvalidTimeZoneException();
            var exception2 = new UriFormatException();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            var renderTask = renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [nameof(NestedAsyncComponent.EventActions)] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = Array.Empty<NestedAsyncComponent.ExecutionAction>(),
                    [1] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        new NestedAsyncComponent.ExecutionAction
                        {
                            Event = NestedAsyncComponent.EventType.OnInitAsyncAsync,
                            EventAction = () =>
                            {
                                throw exception1;
                            }
                        },
                    },
                    [2] = new List<NestedAsyncComponent.ExecutionAction>
                    {
                        new NestedAsyncComponent.ExecutionAction
                        {
                            Event = NestedAsyncComponent.EventType.OnInitAsyncAsync,
                            EventAction = () =>
                            {
                                throw exception2;
                            }
                        },
                    },
                },
                [nameof(NestedAsyncComponent.WhatToRender)] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(new[] { 1, 2, }),
                    [1] = CreateRenderFactory(Array.Empty<int>()),
                    [2] = CreateRenderFactory(Array.Empty<int>()),
                },
            }));

            Assert.True(renderTask.IsCompletedSuccessfully);

            Assert.Equal(2, renderer.HandledExceptions.Count);
            Assert.Contains(exception1, renderer.HandledExceptions);
            Assert.Contains(exception2, renderer.HandledExceptions);
        }

        [Fact]
        public async Task ExceptionsThrownFromHandleAfterRender_AreHandled()
        {
            // Arrange
            var renderer = new TestRenderer { ShouldHandleExceptions = true };
            var component = new NestedAsyncComponent();
            var exception = new InvalidTimeZoneException();

            var taskCompletionSource = new TaskCompletionSource<int>();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            var renderTask = renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [nameof(NestedAsyncComponent.EventActions)] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = new[]
                    {
                        new NestedAsyncComponent.ExecutionAction
                        {
                            Event = NestedAsyncComponent.EventType.OnAfterRenderAsync,
                            EventAction = () =>
                            {
                                throw exception;
                            },
                        }
                    },
                    [1] = new[]
                    {
                        new NestedAsyncComponent.ExecutionAction
                        {
                            Event = NestedAsyncComponent.EventType.OnAfterRenderAsync,
                            EventAction = async () =>
                            {
                                await Task.Yield();
                                taskCompletionSource.TrySetResult(0);
                                return (1, NestedAsyncComponent.EventType.OnAfterRenderAsync);
                            },
                        }
                    }
                },
                [nameof(NestedAsyncComponent.WhatToRender)] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(new[] { 1 }),
                    [1] = CreateRenderFactory(Array.Empty<int>()),
                },
            }));

            Assert.True(renderTask.IsCompletedSuccessfully);

            // OnAfterRenderAsync happens in the background. Make it more predictable, by gating it until we're ready to capture exceptions.
            await taskCompletionSource.Task.TimeoutAfter(TimeSpan.FromSeconds(10));
            Assert.Same(exception, Assert.Single(renderer.HandledExceptions).GetBaseException());
        }

        [Fact]
        public void SynchronousCancelledTasks_HandleAfterRender_Works()
        {
            // Arrange
            var renderer = new TestRenderer { ShouldHandleExceptions = true };
            var component = new NestedAsyncComponent();
            var tcs = new TaskCompletionSource<(int, NestedAsyncComponent.EventType)>();
            tcs.TrySetCanceled();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            var renderTask = renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [nameof(NestedAsyncComponent.EventActions)] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = new[]
                    {
                        new NestedAsyncComponent.ExecutionAction
                        {
                            Event = NestedAsyncComponent.EventType.OnAfterRenderAsync,
                            EventAction = () => tcs.Task,
                        }
                    },
                },
                [nameof(NestedAsyncComponent.WhatToRender)] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(Array.Empty<int>()),
                },
            }));

            // Rendering should finish synchronously
            Assert.True(renderTask.IsCompletedSuccessfully);
            Assert.Empty(renderer.HandledExceptions);
        }

        [Fact]
        public void AsynchronousCancelledTasks_HandleAfterRender_Works()
        {
            // Arrange
            var renderer = new TestRenderer { ShouldHandleExceptions = true };
            var component = new NestedAsyncComponent();
            var tcs = new TaskCompletionSource<(int, NestedAsyncComponent.EventType)>();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            var renderTask = renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [nameof(NestedAsyncComponent.EventActions)] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = new[]
                    {
                        new NestedAsyncComponent.ExecutionAction
                        {
                            Event = NestedAsyncComponent.EventType.OnAfterRenderAsync,
                            EventAction = () => tcs.Task,
                        }
                    },
                },
                [nameof(NestedAsyncComponent.WhatToRender)] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(Array.Empty<int>()),
                },
            }));

            // Rendering should be complete.
            Assert.True(renderTask.IsCompletedSuccessfully);
            tcs.TrySetCanceled();
            Assert.Empty(renderer.HandledExceptions);
        }

        [Fact]
        public async Task CanceledTasksInHandleAfterRender_AreIgnored()
        {
            // Arrange
            var renderer = new TestRenderer { ShouldHandleExceptions = true };
            var component = new NestedAsyncComponent();
            var taskCompletionSource = new TaskCompletionSource<int>();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act/Assert
            var componentId = renderer.AssignRootComponentId(component);
            await renderer.RenderRootComponentAsync(componentId, ParameterCollection.FromDictionary(new Dictionary<string, object>
            {
                [nameof(NestedAsyncComponent.EventActions)] = new Dictionary<int, IList<NestedAsyncComponent.ExecutionAction>>
                {
                    [0] = new[]
                    {
                        new NestedAsyncComponent.ExecutionAction
                        {
                            Event = NestedAsyncComponent.EventType.OnAfterRenderAsync,
                            EventAction = () =>
                            {
                                taskCompletionSource.TrySetResult(0);
                                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                                return default;
                            },
                        }
                    },
                },
                [nameof(NestedAsyncComponent.WhatToRender)] = new Dictionary<int, Func<NestedAsyncComponent, RenderFragment>>
                {
                    [0] = CreateRenderFactory(Array.Empty<int>()),
                },
            }));

            await taskCompletionSource.Task.TimeoutAfter(TimeSpan.FromSeconds(10));

            Assert.Empty(renderer.HandledExceptions);
        }

        [Fact]
        public void DisposingRenderer_DisposesTopLevelComponents()
        {
            // Arrange
            var renderer = new TestRenderer();
            var component = new DisposableComponent();
            renderer.AssignRootComponentId(component);

            // Act
            renderer.Dispose();

            // Assert
            Assert.True(component.Disposed);
        }

        [Fact]
        public void DisposingRenderer_DisposesNestedComponents()
        {
            // Arrange
            var renderer = new TestRenderer();
            var component = new TestComponent(builder =>
            {
                builder.AddContent(0, "Hello");
                builder.OpenComponent<DisposableComponent>(1);
                builder.CloseComponent();
            });
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();
            var batch = renderer.Batches.Single();
            var componentFrame = batch.ReferenceFrames
                .Single(frame => frame.FrameType == RenderTreeFrameType.Component);
            var nestedComponent = Assert.IsType<DisposableComponent>(componentFrame.Component);

            // Act
            renderer.Dispose();

            // Assert
            Assert.True(component.Disposed);
            Assert.True(nestedComponent.Disposed);
        }

        [Fact]
        public void DisposingRenderer_CapturesExceptionsFromAllRegisteredComponents()
        {
            // Arrange
            var renderer = new TestRenderer { ShouldHandleExceptions = true };
            var exception1 = new Exception();
            var exception2 = new Exception();
            var component = new TestComponent(builder =>
            {
                builder.AddContent(0, "Hello");
                builder.OpenComponent<DisposableComponent>(1);
                builder.AddAttribute(1, nameof(DisposableComponent.DisposeAction), (Action)(() => throw exception1));
                builder.CloseComponent();

                builder.OpenComponent<DisposableComponent>(2);
                builder.AddAttribute(1, nameof(DisposableComponent.DisposeAction), (Action)(() => throw exception2));
                builder.CloseComponent();
            });
            var componentId = renderer.AssignRootComponentId(component);
            component.TriggerRender();

            // Act &A Assert
            renderer.Dispose();

            // All components must be disposed even if some throw as part of being diposed.
            Assert.True(component.Disposed);
            Assert.Equal(2, renderer.HandledExceptions.Count);
            Assert.Contains(exception1, renderer.HandledExceptions);
            Assert.Contains(exception2, renderer.HandledExceptions);
        }

        private class NoOpRenderer : Renderer
        {
            public NoOpRenderer() : base(new TestServiceProvider(), new RendererSynchronizationContext())
            {
            }

            public new int AssignRootComponentId(IComponent component)
                => base.AssignRootComponentId(component);

            protected override void HandleException(Exception exception)
                => throw new NotImplementedException();

            protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
                => Task.CompletedTask;
        }

        private class TestComponent : IComponent, IDisposable
        {
            private RenderHandle _renderHandle;
            private RenderFragment _renderFragment;

            public TestComponent(RenderFragment renderFragment)
            {
                _renderFragment = renderFragment;
            }

            public void Configure(RenderHandle renderHandle)
            {
                _renderHandle = renderHandle;
            }

            public Task SetParametersAsync(ParameterCollection parameters)
            {
                TriggerRender();
                return Task.CompletedTask;
            }

            public void TriggerRender()
            {
                var t = _renderHandle.Invoke(() => _renderHandle.Render(_renderFragment));
                // This should always be run synchronously
                Assert.True(t.IsCompleted);
                if (t.IsFaulted)
                {
                    var exception = t.Exception.Flatten().InnerException;
                    while (exception is AggregateException e)
                    {
                        exception = e.InnerException;
                    }
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
            }

            public bool Disposed { get; private set; }

            void IDisposable.Dispose() => Disposed = true;
        }

        private class MessageComponent : AutoRenderComponent
        {
            [Parameter]
            internal string Message { get; set; }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.AddContent(0, Message);
            }
        }

        private class FakeComponent : IComponent
        {
            [Parameter]
            internal int IntProperty { get; private set; }

            [Parameter]
            internal string StringProperty { get; private set; }

            [Parameter]
            internal object ObjectProperty { get; set; }

            public RenderHandle RenderHandle { get; private set; }

            public void Configure(RenderHandle renderHandle)
                => RenderHandle = renderHandle;

            public Task SetParametersAsync(ParameterCollection parameters)
            {
                parameters.SetParameterProperties(this);
                return Task.CompletedTask;
            }
        }

        private class EventComponent : AutoRenderComponent, IComponent, IHandleEvent
        {
            [Parameter]
            internal Action<UIEventArgs> OnTest { get; set; }

            [Parameter]
            internal Func<UIEventArgs, Task> OnTestAsync { get; set; }

            [Parameter]
            internal Action<UIMouseEventArgs> OnClick { get; set; }

            [Parameter]
            internal Func<UIMouseEventArgs, Task> OnClickAsync { get; set; }

            [Parameter]
            internal Action OnClickAction { get; set; }

            [Parameter]
            internal Func<Task> OnClickAsyncAction { get; set; }

            [Parameter]
            internal EventCallback OnClickEventCallback { get; set; }

            [Parameter]
            internal EventCallback<UIMouseEventArgs> OnClickEventCallbackOfT { get; set; }

            public bool SkipElement { get; set; }
            private int renderCount = 0;

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenElement(0, "grandparent");
                if (!SkipElement)
                {
                    builder.OpenElement(1, "parent");
                    builder.OpenElement(2, "some element");

                    if (OnTest != null)
                    {
                        builder.AddAttribute(3, "ontest", OnTest);
                    }
                    else if (OnTestAsync != null)
                    {
                        builder.AddAttribute(3, "ontest", OnTestAsync);
                    }

                    if (OnClick != null)
                    {
                        builder.AddAttribute(4, "onclick", OnClick);
                    }
                    else if (OnClickAsync != null)
                    {
                        builder.AddAttribute(4, "onclick", OnClickAsync);
                    }
                    else if (OnClickEventCallback.HasDelegate)
                    {
                        builder.AddAttribute(4, "onclick", OnClickEventCallback);
                    }
                    else if (OnClickEventCallbackOfT.HasDelegate)
                    {
                        builder.AddAttribute(4, "onclick", OnClickEventCallbackOfT);
                    }

                    if (OnClickAction != null)
                    {
                        builder.AddAttribute(5, "onclickaction", OnClickAction);
                    }
                    else if (OnClickAsyncAction != null)
                    {
                        builder.AddAttribute(5, "onclickaction", OnClickAsyncAction);
                    }
                    builder.CloseElement();
                    builder.CloseElement();
                }
                builder.CloseElement();
                builder.AddContent(6, $"Render count: {++renderCount}");
            }

            public Task HandleEventAsync(EventCallbackWorkItem callback, object arg)
            {
                // Notice, we don't re-render.
                return callback.InvokeAsync(arg);
            }
        }

        private class ConditionalParentComponent<T> : AutoRenderComponent where T : IComponent
        {
            [Parameter]
            internal bool IncludeChild { get; set; }

            [Parameter]
            internal IDictionary<string, object> ChildParameters { get; set; }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.AddContent(0, "Parent here");

                if (IncludeChild)
                {
                    builder.OpenComponent<T>(1);
                    if (ChildParameters != null)
                    {
                        var sequence = 2;
                        foreach (var kvp in ChildParameters)
                        {
                            builder.AddAttribute(sequence++, kvp.Key, kvp.Value);
                        }
                    }
                    builder.CloseComponent();
                }
            }
        }

        private class ReRendersParentComponent : AutoRenderComponent
        {
            [Parameter]
            internal TestComponent Parent { get; private set; }

            private bool _isFirstTime = true;

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                if (_isFirstTime) // Don't want an infinite loop
                {
                    _isFirstTime = false;
                    Parent.TriggerRender();
                }

                builder.AddContent(0, "Child is here");
            }
        }

        private class RendersSelfAfterEventComponent : IComponent, IHandleEvent
        {
            [Parameter]
            Action<object> OnClick { get; set; }

            private RenderHandle _renderHandle;

            public void Configure(RenderHandle renderHandle)
                => _renderHandle = renderHandle;

            public Task SetParametersAsync(ParameterCollection parameters)
            {
                parameters.SetParameterProperties(this);
                Render();
                return Task.CompletedTask;
            }

            public Task HandleEventAsync(EventCallbackWorkItem callback, object arg)
            {
                var task = callback.InvokeAsync(arg);
                Render();
                return task;
            }

            private void Render()
                => _renderHandle.Render(builder =>
                {
                    builder.OpenElement(0, "my button");
                    builder.AddAttribute(1, "my click handler", eventArgs => OnClick(eventArgs));
                    builder.CloseElement();
                });
        }

        private class MultiRendererComponent : IComponent
        {
            private readonly List<RenderHandle> _renderHandles
                = new List<RenderHandle>();

            public void Configure(RenderHandle renderHandle)
                => _renderHandles.Add(renderHandle);

            public Task SetParametersAsync(ParameterCollection parameters)
            {
                return Task.CompletedTask;
            }

            public void TriggerRender()
            {
                foreach (var renderHandle in _renderHandles)
                {
                    renderHandle.Invoke(() => renderHandle.Render(builder =>
                    {
                        builder.AddContent(0, $"Hello from {nameof(MultiRendererComponent)}");
                    }));
                }
            }
        }

        private class BindPlusConditionalAttributeComponent : AutoRenderComponent, IHandleEvent
        {
            public bool CheckboxEnabled;
            public string SomeStringProperty;

            public Task HandleEventAsync(EventCallbackWorkItem callback, object arg)
            {
                var task = callback.InvokeAsync(arg);
                TriggerRender();
                return task;
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenElement(0, "input");
                builder.AddAttribute(1, "type", "checkbox");
                builder.AddAttribute(2, "value", BindMethods.GetValue(CheckboxEnabled));
                builder.AddAttribute(3, "onchange", BindMethods.SetValueHandler(__value => CheckboxEnabled = __value, CheckboxEnabled));
                builder.CloseElement();
                builder.OpenElement(4, "input");
                builder.AddAttribute(5, "value", BindMethods.GetValue(SomeStringProperty));
                builder.AddAttribute(6, "onchange", BindMethods.SetValueHandler(__value => SomeStringProperty = __value, SomeStringProperty));
                builder.AddAttribute(7, "disabled", !CheckboxEnabled);
                builder.CloseElement();
            }
        }

        private class AfterRenderCaptureComponent : AutoRenderComponent, IComponent, IHandleAfterRender
        {
            public int OnAfterRenderCallCount { get; private set; }

            public Task OnAfterRenderAsync()
            {
                OnAfterRenderCallCount++;
                return Task.CompletedTask;
            }

            Task IComponent.SetParametersAsync(ParameterCollection parameters)
            {
                TriggerRender();
                return Task.CompletedTask;
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
            }
        }

        private class DisposableComponent : AutoRenderComponent, IDisposable
        {
            public bool Disposed { get; private set; }

            [Parameter]
            public Action DisposeAction { get; private set; }

            public void Dispose()
            {
                Disposed = true;
                DisposeAction?.Invoke();
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
            }
        }

        class TestAsyncRenderer : TestRenderer
        {
            public Task NextUpdateDisplayReturnTask { get; set; }

            protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
            {
                base.UpdateDisplayAsync(renderBatch);
                return NextUpdateDisplayReturnTask;
            }
        }

        private class AsyncComponent : IComponent
        {
            private RenderHandle _renderHandler;

            public AsyncComponent(Task taskToAwait, int number)
            {
                _taskToAwait = taskToAwait;
                Number = number;
            }

            private readonly Task _taskToAwait;

            public int Number { get; set; }

            public void Configure(RenderHandle renderHandle)
            {
                _renderHandler = renderHandle;
            }

            public async Task SetParametersAsync(ParameterCollection parameters)
            {
                int n;
                while (Number > 0)
                {
                    n = Number;
                    _renderHandler.Render(CreateFragment);
                    Number--;
                    await _taskToAwait;
                };

                // Cheap closure
                void CreateFragment(RenderTreeBuilder builder)
                {
                    var s = 0;
                    builder.OpenElement(s++, "p");
                    builder.AddContent(s++, n);
                    builder.CloseElement();
                }
            }
        }

        private class OuterEventComponent : IComponent, IHandleEvent
        {
            private RenderHandle _renderHandle;

            public RenderFragment RenderFragment { get; set; }

            public Action OnEvent { get; set; }

            public int SomeMethodCallCount { get; set; }

            public void SomeMethod()
            {
                SomeMethodCallCount++;
            }

            public void Configure(RenderHandle renderHandle)
            {
                _renderHandle = renderHandle;
            }

            public Task HandleEventAsync(EventCallbackWorkItem callback, object arg)
            {
                var task = callback.InvokeAsync(arg);
                OnEvent?.Invoke();
                return task;
            }

            public Task SetParametersAsync(ParameterCollection parameters)
            {
                return TriggerRenderAsync();
            }

            public Task TriggerRenderAsync() => _renderHandle.Invoke(() => _renderHandle.Render(RenderFragment));
        }

        private void AssertStream(int expectedId, (int id, NestedAsyncComponent.EventType @event)[] logStream)
        {
            // OnInit runs first
            Assert.Equal((expectedId, NestedAsyncComponent.EventType.OnInit), logStream[0]);

            // OnInit async completes
            Assert.Single(logStream.Skip(1),
                e => e == (expectedId, NestedAsyncComponent.EventType.OnInitAsyncAsync) || e == (expectedId, NestedAsyncComponent.EventType.OnInitAsyncSync));

            var parametersSetEvent = logStream.Where(le => le == (expectedId, NestedAsyncComponent.EventType.OnParametersSet)).ToArray();
            // OnParametersSet gets called at least once
            Assert.NotEmpty(parametersSetEvent);

            var parametersSetAsyncEvent = logStream
                .Where(le => le == (expectedId, NestedAsyncComponent.EventType.OnParametersSetAsyncAsync) ||
                       le == (expectedId, NestedAsyncComponent.EventType.OnParametersSetAsyncSync))
                .ToArray();
            // OnParametersSetAsync async gets called at least once
            Assert.NotEmpty(parametersSetAsyncEvent);

            // The same number of OnParametersSet and OnParametersSetAsync get produced
            Assert.Equal(parametersSetEvent.Length, parametersSetAsyncEvent.Length);

            // The log ends with an OnParametersSetAsync event
            Assert.True(logStream.Last() == (expectedId, NestedAsyncComponent.EventType.OnParametersSetAsyncSync) ||
                logStream.Last() == (expectedId, NestedAsyncComponent.EventType.OnParametersSetAsyncAsync));
        }

        private Func<NestedAsyncComponent, RenderFragment> CreateRenderFactory(int[] childrenToRender)
        {
            // For some reason nameof doesn't work inside a nested lambda, so capturing the value here.
            var eventActionsName = nameof(NestedAsyncComponent.EventActions);
            var whatToRenderName = nameof(NestedAsyncComponent.WhatToRender);
            var testIdName = nameof(NestedAsyncComponent.TestId);
            var logName = nameof(NestedAsyncComponent.Log);

            return component => builder =>
            {
                var s = 0;
                builder.OpenElement(s++, "div");
                builder.AddContent(s++, $"Id: {component.TestId} BuildRenderTree, {Guid.NewGuid()}");
                foreach (var child in childrenToRender)
                {
                    builder.OpenComponent<NestedAsyncComponent>(s++);
                    builder.AddAttribute(s++, eventActionsName, component.EventActions);
                    builder.AddAttribute(s++, whatToRenderName, component.WhatToRender);
                    builder.AddAttribute(s++, testIdName, child);
                    builder.AddAttribute(s++, logName, component.Log);
                    builder.CloseComponent();
                }

                builder.CloseElement();
            };
        }

        private class NestedAsyncComponent : ComponentBase
        {
            [Parameter] public IDictionary<int, IList<ExecutionAction>> EventActions { get; set; }

            [Parameter] public IDictionary<int, Func<NestedAsyncComponent, RenderFragment>> WhatToRender { get; set; }

            [Parameter] public int TestId { get; set; }

            [Parameter] public ConcurrentQueue<(int testId, EventType @event)> Log { get; set; }

            protected override void OnInit()
            {
                if (TryGetEntry(EventType.OnInit, out var entry))
                {
                    var result = entry.EventAction();
                    LogResult(result.Result);
                }
                base.OnInit();
            }

            protected override async Task OnInitAsync()
            {
                if (TryGetEntry(EventType.OnInitAsyncSync, out var entrySync))
                {
                    var result = await entrySync.EventAction();
                    LogResult(result);
                }
                else if (TryGetEntry(EventType.OnInitAsyncAsync, out var entryAsync))
                {
                    var result = await entryAsync.EventAction();
                    LogResult(result);
                }
            }

            protected override void OnParametersSet()
            {
                if (TryGetEntry(EventType.OnParametersSet, out var entry))
                {
                    var result = entry.EventAction();
                    LogResult(result.Result);
                }
                base.OnParametersSet();
            }

            protected override async Task OnParametersSetAsync()
            {
                if (TryGetEntry(EventType.OnParametersSetAsyncSync, out var entrySync))
                {
                    var result = await entrySync.EventAction();
                    LogResult(result);

                    await entrySync.EventAction();
                }
                else if (TryGetEntry(EventType.OnParametersSetAsyncAsync, out var entryAsync))
                {
                    var result = await entryAsync.EventAction();
                    LogResult(result);
                }
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                var renderFactory = WhatToRender[TestId];
                renderFactory(this)(builder);
            }

            protected override async Task OnAfterRenderAsync()
            {
                if (TryGetEntry(EventType.OnAfterRenderAsync, out var entry))
                {
                    var result = await entry.EventAction();
                    LogResult(result);
                }
            }

            private bool TryGetEntry(EventType eventType, out ExecutionAction entry)
            {
                var entries = EventActions[TestId];
                if (entries == null)
                {
                    throw new InvalidOperationException("Failed to find entries for component with Id: " + TestId);
                }
                entry = entries.FirstOrDefault(e => e.Event == eventType);
                return entry != null;
            }

            private void LogResult((int, EventType) entry)
            {
                Log?.Enqueue(entry);
            }

            public class ExecutionAction
            {
                public EventType Event { get; set; }
                public Func<Task<(int id, EventType @event)>> EventAction { get; set; }

                public static ExecutionAction On(int id, EventType @event, bool async = false)
                {
                    if (!async)
                    {
                        return new ExecutionAction
                        {
                            Event = @event,
                            EventAction = () => Task.FromResult((id, @event))
                        };
                    }
                    else
                    {
                        return new ExecutionAction
                        {
                            Event = @event,
                            EventAction = async () =>
                            {
                                await Task.Yield();
                                return (id, @event);
                            }
                        };
                    }
                }
            }

            public enum EventType
            {
                OnInit,
                OnInitAsyncSync,
                OnInitAsyncAsync,
                OnParametersSet,
                OnParametersSetAsyncSync,
                OnParametersSetAsyncAsync,
                OnAfterRenderAsync,
            }
        }

        private class ComponentThatAwaitsTask : ComponentBase
        {
            [Parameter] public Task TaskToAwait { get; set; }

            protected override async Task OnParametersSetAsync()
            {
                await TaskToAwait;
            }
        }
    }
}
