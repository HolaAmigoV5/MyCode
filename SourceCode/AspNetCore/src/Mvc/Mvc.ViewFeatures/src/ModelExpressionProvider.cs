﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures
{
    /// <summary>
    /// Provides <see cref="ModelExpression"/> for expressions.
    /// </summary>
    public class ModelExpressionProvider : IModelExpressionProvider
    {
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly ConcurrentDictionary<LambdaExpression, string> _expressionTextCache;

        /// <summary>
        /// Creates a new <see cref="ModelExpressionProvider"/>.
        /// </summary>
        /// <param name="modelMetadataProvider">The <see cref="IModelMetadataProvider"/>.</param>
        public ModelExpressionProvider(IModelMetadataProvider modelMetadataProvider)
        {
            if (modelMetadataProvider == null)
            {
                throw new ArgumentNullException(nameof(modelMetadataProvider));
            }

            _modelMetadataProvider = modelMetadataProvider;
            _expressionTextCache = new ConcurrentDictionary<LambdaExpression, string>(LambdaExpressionComparer.Instance);
        }

        /// <summary>
        /// Gets the name for <paramref name="expression"/>.
        /// </summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <typeparam name="TValue">The type of the <paramref name="expression"/> result.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>The expression name.</returns>
        public string GetExpressionText<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return ExpressionHelper.GetExpressionText(expression, _expressionTextCache);
        }

        /// <inheritdoc />
        public ModelExpression CreateModelExpression<TModel, TValue>(
            ViewDataDictionary<TModel> viewData,
            Expression<Func<TModel, TValue>> expression)
        {
            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(viewData));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var name = GetExpressionText(expression);
            var modelExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, viewData, _modelMetadataProvider);
            if (modelExplorer == null)
            {
                throw new InvalidOperationException(
                    Resources.FormatCreateModelExpression_NullModelMetadata(nameof(IModelMetadataProvider), name));
            }

            return new ModelExpression(name, modelExplorer);
        }

        /// <summary>
        /// Returns a <see cref="ModelExpression"/> instance describing the given <paramref name="expression"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of the <paramref name="viewData"/>'s <see cref="ViewDataDictionary{T}.Model"/>.</typeparam>
        /// <param name="viewData">The <see cref="ViewDataDictionary{TModel}"/> containing the <see cref="ViewDataDictionary{T}.Model"/>
        /// against which <paramref name="expression"/> is evaluated. </param>
        /// <param name="expression">Expression name, relative to <c>viewData.Model</c>.</param>
        /// <returns>A new <see cref="ModelExpression"/> instance describing the given <paramref name="expression"/>.</returns>
        public ModelExpression CreateModelExpression<TModel>(
            ViewDataDictionary<TModel> viewData,
            string expression)
        {
            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(viewData));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var modelExplorer = ExpressionMetadataProvider.FromStringExpression(expression, viewData, _modelMetadataProvider);
            if (modelExplorer == null)
            {
                throw new InvalidOperationException(
                    Resources.FormatCreateModelExpression_NullModelMetadata(nameof(IModelMetadataProvider), expression));
            }

            return new ModelExpression(expression, modelExplorer);
        }
    }
}
