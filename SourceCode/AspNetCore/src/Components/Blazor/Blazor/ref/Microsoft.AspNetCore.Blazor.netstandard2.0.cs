// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Blazor.Hosting
{
    public static partial class BlazorWebAssemblyHost
    {
        public static Microsoft.AspNetCore.Blazor.Hosting.IWebAssemblyHostBuilder CreateDefaultBuilder() { throw null; }
    }
    public partial interface IWebAssemblyHost : System.IDisposable
    {
        System.IServiceProvider Services { get; }
        System.Threading.Tasks.Task StartAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        System.Threading.Tasks.Task StopAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
    }
    public partial interface IWebAssemblyHostBuilder
    {
        System.Collections.Generic.IDictionary<object, object> Properties { get; }
        Microsoft.AspNetCore.Blazor.Hosting.IWebAssemblyHost Build();
        Microsoft.AspNetCore.Blazor.Hosting.IWebAssemblyHostBuilder ConfigureServices(System.Action<Microsoft.AspNetCore.Blazor.Hosting.WebAssemblyHostBuilderContext, Microsoft.Extensions.DependencyInjection.IServiceCollection> configureDelegate);
        Microsoft.AspNetCore.Blazor.Hosting.IWebAssemblyHostBuilder UseServiceProviderFactory<TContainerBuilder>(Microsoft.Extensions.DependencyInjection.IServiceProviderFactory<TContainerBuilder> factory);
        Microsoft.AspNetCore.Blazor.Hosting.IWebAssemblyHostBuilder UseServiceProviderFactory<TContainerBuilder>(System.Func<Microsoft.AspNetCore.Blazor.Hosting.WebAssemblyHostBuilderContext, Microsoft.Extensions.DependencyInjection.IServiceProviderFactory<TContainerBuilder>> factory);
    }
    public sealed partial class WebAssemblyHostBuilderContext
    {
        public WebAssemblyHostBuilderContext(System.Collections.Generic.IDictionary<object, object> properties) { }
        public System.Collections.Generic.IDictionary<object, object> Properties { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } }
    }
    public static partial class WebAssemblyHostBuilderExtensions
    {
        public static Microsoft.AspNetCore.Blazor.Hosting.IWebAssemblyHostBuilder ConfigureServices(this Microsoft.AspNetCore.Blazor.Hosting.IWebAssemblyHostBuilder hostBuilder, System.Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> configureDelegate) { throw null; }
        public static Microsoft.AspNetCore.Blazor.Hosting.IWebAssemblyHostBuilder UseBlazorStartup(this Microsoft.AspNetCore.Blazor.Hosting.IWebAssemblyHostBuilder builder, System.Type startupType) { throw null; }
        public static Microsoft.AspNetCore.Blazor.Hosting.IWebAssemblyHostBuilder UseBlazorStartup<TStartup>(this Microsoft.AspNetCore.Blazor.Hosting.IWebAssemblyHostBuilder builder) { throw null; }
    }
    public static partial class WebAssemblyHostExtensions
    {
        public static void Run(this Microsoft.AspNetCore.Blazor.Hosting.IWebAssemblyHost host) { }
    }
}
namespace Microsoft.AspNetCore.Blazor.Http
{
    public enum FetchCredentialsOption
    {
        Include = 2,
        Omit = 0,
        SameOrigin = 1,
    }
    public partial class WebAssemblyHttpMessageHandler : System.Net.Http.HttpMessageHandler
    {
        public const string FetchArgs = "WebAssemblyHttpMessageHandler.FetchArgs";
        public WebAssemblyHttpMessageHandler() { }
        public static Microsoft.AspNetCore.Blazor.Http.FetchCredentialsOption DefaultCredentials { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute]set { } }
        [System.Diagnostics.DebuggerStepThroughAttribute]
        protected override System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) { throw null; }
    }
}
namespace Microsoft.AspNetCore.Blazor.Rendering
{
    public partial class WebAssemblyRenderer : Microsoft.AspNetCore.Components.Rendering.Renderer
    {
        public WebAssemblyRenderer(System.IServiceProvider serviceProvider) : base (default(System.IServiceProvider)) { }
        public System.Threading.Tasks.Task AddComponentAsync(System.Type componentType, string domElementSelector) { throw null; }
        public System.Threading.Tasks.Task AddComponentAsync<TComponent>(string domElementSelector) where TComponent : Microsoft.AspNetCore.Components.IComponent { throw null; }
        protected override void Dispose(bool disposing) { }
        protected override void HandleException(System.Exception exception) { }
        protected override System.Threading.Tasks.Task UpdateDisplayAsync(in Microsoft.AspNetCore.Components.Rendering.RenderBatch batch) { throw null; }
    }
}
namespace Microsoft.AspNetCore.Blazor.Services
{
    public partial class WebAssemblyUriHelper : Microsoft.AspNetCore.Components.Services.UriHelperBase
    {
        internal WebAssemblyUriHelper() { }
        public static readonly Microsoft.AspNetCore.Blazor.Services.WebAssemblyUriHelper Instance;
        protected override void InitializeState() { }
        protected override void NavigateToCore(string uri, bool forceLoad) { }
        [Microsoft.JSInterop.JSInvokableAttribute("NotifyLocationChanged")]
        public static void NotifyLocationChanged(string newAbsoluteUri) { }
    }
}
namespace Microsoft.AspNetCore.Components.Builder
{
    public static partial class ComponentsApplicationBuilderExtensions
    {
        public static void AddComponent<TComponent>(this Microsoft.AspNetCore.Components.Builder.IComponentsApplicationBuilder app, string domElementSelector) where TComponent : Microsoft.AspNetCore.Components.IComponent { }
    }
    public partial interface IComponentsApplicationBuilder
    {
        System.IServiceProvider Services { get; }
        void AddComponent(System.Type componentType, string domElementSelector);
    }
}
