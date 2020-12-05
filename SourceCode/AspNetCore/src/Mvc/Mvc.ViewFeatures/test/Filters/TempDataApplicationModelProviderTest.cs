﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Filters
{
    public class TempDataApplicationModelProviderTest
    {
        [Fact]
        public void OnProvidersExecuting_DoesNotAddFilter_IfTypeHasNoTempDataProperties()
        {
            // Arrange
            var type = typeof(TestController_NoTempDataProperties);
            var provider = CreateProvider();

            var context = GetContext(type);

            // Act
            provider.OnProvidersExecuting(context);

            // Assert
            var controller = Assert.Single(context.Result.Controllers);
            Assert.Empty(controller.Filters);
        }

        [Fact]
        public void OnProvidersExecuting_ValidatesTempDataProperties()
        {
            // Arrange
            var type = typeof(TestController_PrivateSet);
            var provider = CreateProvider();
            var expected = $"The '{type.FullName}.Test' property with TempDataAttribute is invalid. A property using TempDataAttribute must have a public getter and setter.";

            var context = GetContext(type);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => provider.OnProvidersExecuting(context));
            Assert.Equal(expected, ex.Message);
        }

        [Fact]
        public void AddsTempDataPropertyFilter_ForTempDataAttributeProperties()
        {
            // Arrange
            var type = typeof(TestController_NullableNonPrimitiveTempDataProperty);
            var provider = CreateProvider();

            var context = GetContext(type);

            // Act
            provider.OnProvidersExecuting(context);

            // Assert
            var controller = Assert.Single(context.Result.Controllers);
            Assert.IsType<ControllerSaveTempDataPropertyFilterFactory>(Assert.Single(controller.Filters));
        }

        [Fact]
        public void InitializeFilterFactory_WithExpectedPropertyHelpers_ForTempDataAttributeProperties()
        {
            // Arrange
            var type = typeof(TestController_OneTempDataProperty);
            var expected = type.GetProperty(nameof(TestController_OneTempDataProperty.Test2));
            var provider = CreateProvider();

            var context = GetContext(type);

            // Act
            provider.OnProvidersExecuting(context);
            var controller = context.Result.Controllers.SingleOrDefault();
            var filter = Assert.IsType<ControllerSaveTempDataPropertyFilterFactory>(Assert.Single(controller.Filters));

            // Assert
            Assert.NotNull(filter);
            var property = Assert.Single(filter.TempDataProperties);
            Assert.Same(expected, property.PropertyInfo);
            Assert.Equal("Test2", property.Key);
        }

        [Fact]
        public void OnProvidersExecuting_SetsKeyPrefixToEmptyString()
        {
            // Arrange
            var expected = typeof(TestController_OneTempDataProperty).GetProperty(nameof(TestController_OneTempDataProperty.Test2));
            var type = typeof(TestController_OneTempDataProperty);
            var provider = CreateProvider();
            var context = GetContext(type);

            // Act
            provider.OnProvidersExecuting(context);
            var controller = context.Result.Controllers.SingleOrDefault();
            var filter = Assert.IsType<ControllerSaveTempDataPropertyFilterFactory>(Assert.Single(controller.Filters));

            // Assert
            Assert.NotNull(filter);
            var property = Assert.Single(filter.TempDataProperties);
            Assert.Same(expected, property.PropertyInfo);
            Assert.Equal("Test2", property.Key);
        }

        private static TempDataApplicationModelProvider CreateProvider()
        {
            var tempDataSerializer = Mock.Of<TempDataSerializer>(s => s.CanSerializeType(It.IsAny<Type>()) == true);
            return new TempDataApplicationModelProvider(tempDataSerializer);
        }

        private static ApplicationModelProviderContext GetContext(Type type)
        {
            var defaultProvider = new DefaultApplicationModelProvider(
                Options.Create(new MvcOptions()),
                new EmptyModelMetadataProvider());

            var context = new ApplicationModelProviderContext(new[] { type.GetTypeInfo() });
            defaultProvider.OnProvidersExecuting(context);
            return context;
        }

        public class TestController_NoTempDataProperties
        {
            public DateTime? DateTime { get; set; }
        }

        public class TestController_NullableNonPrimitiveTempDataProperty
        {
            [TempData]
            public DateTime? DateTime { get; set; }
        }

        public class TestController_OneTempDataProperty
        {
            public string Test { get; set; }

            [TempData]
            public string Test2 { get; set; }
        }

        public class TestController_PrivateSet
        {
            [TempData]
            public string Test { get; private set; }
        }
    }
}
