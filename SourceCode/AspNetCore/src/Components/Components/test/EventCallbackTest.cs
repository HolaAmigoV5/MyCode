// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNetCore.Components
{
    public class EventCallbackTest
    {
        [Fact]
        public async Task EventCallback_Default()
        {
            // Arrange
            var callback = default(EventCallback);

            // Act & Assert (Does not throw)
            await callback.InvokeAsync(null);
        }

        [Fact]
        public async Task EventCallbackOfT_Default()
        {
            // Arrange
            var callback = default(EventCallback<UIEventArgs>);

            // Act & Assert (Does not throw)
            await callback.InvokeAsync(null);
        }


        [Fact]
        public async Task EventCallback_NullReceiver()
        {
            // Arrange
            int runCount = 0;
            var callback = new EventCallback(null, (Action)(() => runCount++));

            // Act
            await callback.InvokeAsync(null);


            // Assert
            Assert.Equal(1, runCount);
        }

        [Fact]
        public async Task EventCallbackOfT_NullReceiver()
        {
            // Arrange
            int runCount = 0;
            var callback = new EventCallback<UIEventArgs>(null, (Action)(() => runCount++));

            // Act
            await callback.InvokeAsync(null);


            // Assert
            Assert.Equal(1, runCount);
        }

        [Fact]
        public async Task EventCallback_Action_Null()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            var callback = new EventCallback(component, (Action)(() => runCount++));

            // Act
            await callback.InvokeAsync(null);


            // Assert
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallback_Action_IgnoresArg()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            var callback = new EventCallback(component, (Action)(() => runCount++));

            // Act
            await callback.InvokeAsync(new UIEventArgs());


            // Assert
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallback_ActionT_Null()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            UIEventArgs arg = null;
            var callback = new EventCallback(component, (Action<UIEventArgs>)((e) => { arg = e; runCount++; }));

            // Act
            await callback.InvokeAsync(null);


            // Assert
            Assert.Null(arg);
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallback_ActionT_Arg()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            UIEventArgs arg = null;
            var callback = new EventCallback(component, (Action<UIEventArgs>)((e) => { arg = e; runCount++; }));

            // Act
            await callback.InvokeAsync(new UIEventArgs());


            // Assert
            Assert.NotNull(arg);
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallback_ActionT_Arg_ValueType()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            int arg = -1;
            var callback = new EventCallback(component, (Action<int>)((e) => { arg = e; runCount++; }));

            // Act
            await callback.InvokeAsync(17);


            // Assert
            Assert.Equal(17, arg);
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallback_ActionT_ArgMismatch()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            UIEventArgs arg = null;
            var callback = new EventCallback(component, (Action<UIEventArgs>)((e) => { arg = e; runCount++; }));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                return callback.InvokeAsync(new StringBuilder());
            });
        }

        [Fact]
        public async Task EventCallback_FuncTask_Null()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            var callback = new EventCallback(component, (Func<Task>)(() => { runCount++; return Task.CompletedTask; }));

            // Act
            await callback.InvokeAsync(null);


            // Assert
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallback_FuncTask_IgnoresArg()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            var callback = new EventCallback(component, (Func<Task>)(() => { runCount++; return Task.CompletedTask; }));

            // Act
            await callback.InvokeAsync(new UIEventArgs());


            // Assert
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallback_FuncTTask_Null()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            UIEventArgs arg = null;
            var callback = new EventCallback(component, (Func<UIEventArgs, Task>)((e) => { arg = e; runCount++; return Task.CompletedTask; }));

            // Act
            await callback.InvokeAsync(null);


            // Assert
            Assert.Null(arg);
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallback_FuncTTask_Arg()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            UIEventArgs arg = null;
            var callback = new EventCallback(component, (Func<UIEventArgs, Task>)((e) => { arg = e; runCount++; return Task.CompletedTask; }));

            // Act
            await callback.InvokeAsync(new UIEventArgs());


            // Assert
            Assert.NotNull(arg);
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallback_FuncTTask_Arg_ValueType()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            int arg = -1;
            var callback = new EventCallback(component, (Func<int, Task>)((e) => { arg = e; runCount++; return Task.CompletedTask; }));

            // Act
            await callback.InvokeAsync(17);


            // Assert
            Assert.Equal(17, arg);
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallback_FuncTTask_ArgMismatch()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            UIEventArgs arg = null;
            var callback = new EventCallback(component, (Func<UIEventArgs, Task>)((e) => { arg = e; runCount++; return Task.CompletedTask; }));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                return callback.InvokeAsync(new StringBuilder());
            });
        }

        [Fact]
        public async Task EventCallbackOfT_Action_Null()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            var callback = new EventCallback<UIEventArgs>(component, (Action)(() => runCount++));

            // Act
            await callback.InvokeAsync(null);


            // Assert
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallbackOfT_Action_IgnoresArg()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            var callback = new EventCallback<UIEventArgs>(component, (Action)(() => runCount++));

            // Act
            await callback.InvokeAsync(new UIEventArgs());


            // Assert
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallbackOfT_ActionT_Null()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            UIEventArgs arg = null;
            var callback = new EventCallback<UIEventArgs>(component, (Action<UIEventArgs>)((e) => { arg = e; runCount++; }));

            // Act
            await callback.InvokeAsync(null);


            // Assert
            Assert.Null(arg);
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallbackOfT_ActionT_Arg()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            UIEventArgs arg = null;
            var callback = new EventCallback<UIEventArgs>(component, (Action<UIEventArgs>)((e) => { arg = e; runCount++; }));

            // Act
            await callback.InvokeAsync(new UIEventArgs());


            // Assert
            Assert.NotNull(arg);
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallbackOfT_FuncTask_Null()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            var callback = new EventCallback<UIEventArgs>(component, (Func<Task>)(() => { runCount++; return Task.CompletedTask; }));

            // Act
            await callback.InvokeAsync(null);


            // Assert
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallbackOfT_FuncTask_IgnoresArg()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            var callback = new EventCallback<UIEventArgs>(component, (Func<Task>)(() => { runCount++; return Task.CompletedTask; }));

            // Act
            await callback.InvokeAsync(new UIEventArgs());


            // Assert
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallbackOfT_FuncTTask_Null()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            UIEventArgs arg = null;
            var callback = new EventCallback<UIEventArgs>(component, (Func<UIEventArgs, Task>)((e) => { arg = e; runCount++; return Task.CompletedTask; }));

            // Act
            await callback.InvokeAsync(null);


            // Assert
            Assert.Null(arg);
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        [Fact]
        public async Task EventCallbackOfT_FuncTTask_Arg()
        {
            // Arrange
            var component = new EventCountingComponent();

            int runCount = 0;
            UIEventArgs arg = null;
            var callback = new EventCallback<UIEventArgs>(component, (Func<UIEventArgs, Task>)((e) => { arg = e; runCount++; return Task.CompletedTask; }));

            // Act
            await callback.InvokeAsync(new UIEventArgs());


            // Assert
            Assert.NotNull(arg);
            Assert.Equal(1, runCount);
            Assert.Equal(1, component.Count);
        }

        private class EventCountingComponent : IComponent, IHandleEvent
        {
            public int Count;

            public Task HandleEventAsync(EventCallbackWorkItem item, object arg)
            {
                Count++;
                return item.InvokeAsync(arg);
            }

            public void Configure(RenderHandle renderHandle) => throw new NotImplementedException();

            public Task SetParametersAsync(ParameterCollection parameters) => throw new NotImplementedException();
        }
    }
}
