﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DaJuTestDemo.Core
{
    /// <summary>
    /// Loads XAML files using Application.LoadComponent.
    /// </summary>
    public class DefaultContentLoader : IContentLoader
    {
        /// <summary>
        /// Asynchronously loads content from specified uri.
        /// </summary>
        /// <param name="uri">The content uri.</param>
        /// <param name="cancellationToken">The token used to cancel the load content task.</param>
        /// <returns>The loaded content.</returns>
        public Task<object> LoadContentAsync(Uri uri, CancellationToken cancellationToken)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                throw new InvalidOperationException("Operation requires the UI thread");
            }

            // scheduler ensures LoadContent is executed on the current UI thread
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return Task.Factory.StartNew(() => LoadContent(uri), cancellationToken, TaskCreationOptions.None, scheduler);
        }

        /// <summary>
        /// Loads the content from specified uri.
        /// </summary>
        /// <param name="uri">The content uri</param>
        /// <returns>The loaded content.</returns>
        protected virtual object LoadContent(Uri uri)
        {
            // don't do anything in design mode
            if (ModernUIHelper.IsInDesignMode)
                return null;
            return Application.LoadComponent(uri);
        }
    }
}
