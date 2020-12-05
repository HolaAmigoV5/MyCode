// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Resources = Microsoft.AspNetCore.Mvc.RazorPages.Resources;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcRazorPagesMvcCoreBuilderExtensions
    {
        public static IMvcCoreBuilder AddRazorPages(this IMvcCoreBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddRazorViewEngine();

            AddRazorPagesServices(builder.Services);

            return builder;
        }

        public static IMvcCoreBuilder AddRazorPages(
            this IMvcCoreBuilder builder,
            Action<RazorPagesOptions> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            builder.AddRazorViewEngine();

            AddRazorPagesServices(builder.Services);

            builder.Services.Configure(setupAction);

            return builder;
        }

        /// <summary>
        /// Configures Razor Pages to use the specified <paramref name="rootDirectory"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder"/>.</param>
        /// <param name="rootDirectory">The application relative path to use as the root directory.</param>
        /// <returns></returns>
        public static IMvcCoreBuilder WithRazorPagesRoot(this IMvcCoreBuilder builder, string rootDirectory)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (string.IsNullOrEmpty(rootDirectory))
            {
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpty, nameof(rootDirectory));
            }

            builder.Services.Configure<RazorPagesOptions>(options => options.RootDirectory = rootDirectory);
            return builder;
        }

        // Internal for testing.
        internal static void AddRazorPagesServices(IServiceCollection services)
        {
            // Options
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<RazorViewEngineOptions>, RazorPagesRazorViewEngineOptionsSetup>());

            // Routing
            services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, PageLoaderMatcherPolicy>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, DynamicPageEndpointMatcherPolicy>());
            services.TryAddSingleton<DynamicPageEndpointSelector>();

            // Action description and invocation
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IActionDescriptorProvider, PageActionDescriptorProvider>());
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IPageRouteModelProvider, CompiledPageRouteModelProvider>());
            services.TryAddSingleton<PageActionEndpointDataSource>();
            services.TryAddSingleton<DynamicPageEndpointSelector>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, DynamicPageEndpointMatcherPolicy>());

            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IPageApplicationModelProvider, DefaultPageApplicationModelProvider>());
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IPageApplicationModelProvider, AutoValidateAntiforgeryPageApplicationModelProvider>());
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IPageApplicationModelProvider, AuthorizationPageApplicationModelProvider>());
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IPageApplicationModelProvider, TempDataFilterPageApplicationModelProvider>());
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IPageApplicationModelProvider, ViewDataAttributePageApplicationModelProvider>());
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IPageApplicationModelProvider, ResponseCacheFilterApplicationModelProvider>());

            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IActionInvokerProvider, PageActionInvokerProvider>());

            // Page and Page model creation and activation
            services.TryAddSingleton<IPageModelActivatorProvider, DefaultPageModelActivatorProvider>();
            services.TryAddSingleton<IPageModelFactoryProvider, DefaultPageModelFactoryProvider>();

            services.TryAddSingleton<IPageActivatorProvider, DefaultPageActivatorProvider>();
            services.TryAddSingleton<IPageFactoryProvider, DefaultPageFactoryProvider>();

#pragma warning disable CS0618 // Type or member is obsolete
            services.TryAddSingleton<IPageLoader>(s => s.GetRequiredService<PageLoader>());
#pragma warning restore CS0618 // Type or member is obsolete
            services.TryAddSingleton<PageLoader, DefaultPageLoader>();
            services.TryAddSingleton<IPageHandlerMethodSelector, DefaultPageHandlerMethodSelector>();

            // Action executors
            services.TryAddSingleton<PageResultExecutor>();

            services.TryAddTransient<PageSaveTempDataPropertyFilter>();
        }
    }
}
