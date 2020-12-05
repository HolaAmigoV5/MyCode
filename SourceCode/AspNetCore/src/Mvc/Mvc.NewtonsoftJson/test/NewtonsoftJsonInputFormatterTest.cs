// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.ObjectPool;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Formatters
{
    public class NewtonsoftJsonInputFormatterTest : JsonInputFormatterTestBase
    {
        private static readonly ObjectPoolProvider _objectPoolProvider = new DefaultObjectPoolProvider();
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings();

        [Fact]
        public async Task Constructor_BuffersRequestBody_UsingDefaultOptions()
        {
            // Arrange
            var formatter = new NewtonsoftJsonInputFormatter(
                GetLogger(),
                _serializerSettings,
                ArrayPool<char>.Shared,
                _objectPoolProvider,
                new MvcOptions(),
                new MvcNewtonsoftJsonOptions());

            var content = "{name: 'Person Name', Age: '30'}";
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<IHttpResponseFeature>(new TestResponseFeature());
            httpContext.Request.Body = new NonSeekableReadStream(contentBytes);
            httpContext.Request.ContentType = "application/json";

            var formatterContext = CreateInputFormatterContext(typeof(User), httpContext);

            // Act
            var result = await formatter.ReadAsync(formatterContext);

            // Assert
            Assert.False(result.HasError);

            var userModel = Assert.IsType<User>(result.Model);
            Assert.Equal("Person Name", userModel.Name);
            Assert.Equal(30, userModel.Age);

            Assert.True(httpContext.Request.Body.CanSeek);
            httpContext.Request.Body.Seek(0L, SeekOrigin.Begin);

            result = await formatter.ReadAsync(formatterContext);

            // Assert
            Assert.False(result.HasError);

            userModel = Assert.IsType<User>(result.Model);
            Assert.Equal("Person Name", userModel.Name);
            Assert.Equal(30, userModel.Age);
        }

        [Fact]
        public async Task Constructor_SuppressInputFormatterBuffering_UsingMvcOptions_DoesNotBufferRequestBody()
        {
            // Arrange
            var mvcOptions = new MvcOptions()
            {
                SuppressInputFormatterBuffering = true,
            };
            var formatter = new NewtonsoftJsonInputFormatter(
                GetLogger(),
                _serializerSettings,
                ArrayPool<char>.Shared,
                _objectPoolProvider,
                mvcOptions,
                new MvcNewtonsoftJsonOptions());

            var content = "{name: 'Person Name', Age: '30'}";
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<IHttpResponseFeature>(new TestResponseFeature());
            httpContext.Request.Body = new NonSeekableReadStream(contentBytes);
            httpContext.Request.ContentType = "application/json";

            var formatterContext = CreateInputFormatterContext(typeof(User), httpContext);

            // Act
            var result = await formatter.ReadAsync(formatterContext);

            // Assert
            Assert.False(result.HasError);

            var userModel = Assert.IsType<User>(result.Model);
            Assert.Equal("Person Name", userModel.Name);
            Assert.Equal(30, userModel.Age);

            Assert.False(httpContext.Request.Body.CanSeek);
            result = await formatter.ReadAsync(formatterContext);

            // Assert
            Assert.False(result.HasError);
            Assert.Null(result.Model);
        }

        [Fact]
        public async Task Version_2_1_Constructor_SuppressInputFormatterBufferingSetToTrue_UsingMutatedOptions()
        {
            // Arrange
            var mvcOptions = new MvcOptions()
            {
                SuppressInputFormatterBuffering = false,
            };
            var formatter = new NewtonsoftJsonInputFormatter(
                GetLogger(),
                _serializerSettings,
                ArrayPool<char>.Shared,
                _objectPoolProvider,
                mvcOptions,
                new MvcNewtonsoftJsonOptions());

            var content = "{name: 'Person Name', Age: '30'}";
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<IHttpResponseFeature>(new TestResponseFeature());
            httpContext.Request.Body = new NonSeekableReadStream(contentBytes);
            httpContext.Request.ContentType = "application/json";

            var formatterContext = CreateInputFormatterContext(typeof(User), httpContext);

            // Act
            // Mutate options after passing into the constructor to make sure that the value type is not store in the constructor
            mvcOptions.SuppressInputFormatterBuffering = true;
            var result = await formatter.ReadAsync(formatterContext);

            // Assert
            Assert.False(result.HasError);

            var userModel = Assert.IsType<User>(result.Model);
            Assert.Equal("Person Name", userModel.Name);
            Assert.Equal(30, userModel.Age);

            Assert.False(httpContext.Request.Body.CanSeek);
            result = await formatter.ReadAsync(formatterContext);

            // Assert
            Assert.False(result.HasError);
            Assert.Null(result.Model);
        }

        [Fact]
        public void Constructor_UsesSerializerSettings()
        {
            // Arrange
            var serializerSettings = new JsonSerializerSettings();

            // Act
            var formatter = new TestableJsonInputFormatter(serializerSettings);

            // Assert
            Assert.Same(serializerSettings, formatter.SerializerSettings);
        }

        [Fact]
        public async Task CustomSerializerSettingsObject_TakesEffect()
        {
            // Arrange
            // by default we ignore missing members, so here explicitly changing it
            var serializerSettings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };
            var formatter = CreateFormatter(serializerSettings, allowInputFormatterExceptionMessages: true);

            // missing password property here
            var contentBytes = Encoding.UTF8.GetBytes("{ \"UserName\" : \"John\"}");
            var httpContext = GetHttpContext(contentBytes, "application/json;charset=utf-8");

            var formatterContext = CreateInputFormatterContext(typeof(UserLogin), httpContext);

            // Act
            var result = await formatter.ReadAsync(formatterContext);

            // Assert
            Assert.True(result.HasError);
            Assert.False(formatterContext.ModelState.IsValid);

            var message = formatterContext.ModelState.Values.First().Errors[0].ErrorMessage;
            Assert.Contains("Required property 'Password' not found in JSON", message);
        }

        [Fact]
        public void CreateJsonSerializer_UsesJsonSerializerSettings()
        {
            // Arrange
            var settings = new JsonSerializerSettings
            {
                ContractResolver = Mock.Of<IContractResolver>(),
                MaxDepth = 2,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
            };
            var formatter = new TestableJsonInputFormatter(settings);

            // Act
            var actual = formatter.CreateJsonSerializer(null);

            // Assert
            Assert.Same(settings.ContractResolver, actual.ContractResolver);
            Assert.Equal(settings.MaxDepth, actual.MaxDepth);
            Assert.Equal(settings.DateTimeZoneHandling, actual.DateTimeZoneHandling);
        }

        [Theory]
        [InlineData(" ", true, true)]
        [InlineData(" ", false, false)]
        public Task ReadAsync_WithInputThatDeserializesToNull_SetsModelOnlyIfAllowingEmptyInput_WhenValueIsWhitespaceString(string content, bool treatEmptyInputAsDefaultValue, bool expectedIsModelSet)
        {
            return base.ReadAsync_WithInputThatDeserializesToNull_SetsModelOnlyIfAllowingEmptyInput(content, treatEmptyInputAsDefaultValue, expectedIsModelSet);
        }

        [Theory]
        [InlineData("{", "", "Unexpected end when reading JSON. Path '', line 1, position 1.")]
        [InlineData("{\"a\":{\"b\"}}", "a", "Invalid character after parsing property name. Expected ':' but got: }. Path 'a', line 1, position 9.")]
        [InlineData("{\"age\":\"x\"}", "age", "Could not convert string to decimal: x. Path 'age', line 1, position 10.")]
        [InlineData("{\"login\":1}", "login", "Error converting value 1 to type 'Microsoft.AspNetCore.Mvc.Formatters.NewtonsoftJsonInputFormatterTest+UserLogin'. Path 'login', line 1, position 10.")]
        [InlineData("{\"login\":{\"username\":\"somevalue\"}}", "login.Password", "Required property 'Password' not found in JSON. Path 'login', line 1, position 33.")]
        public async Task ReadAsync_WithAllowInputFormatterExceptionMessages_RegistersJsonInputExceptionsAsInputFormatterException(
            string content,
            string modelStateKey,
            string expectedMessage)
        {
            // Arrange
            var formatter = CreateFormatter(allowInputFormatterExceptionMessages: true);

            var contentBytes = Encoding.UTF8.GetBytes(content);
            var httpContext = GetHttpContext(contentBytes);

            var formatterContext = CreateInputFormatterContext(typeof(User), httpContext);

            // Act
            var result = await formatter.ReadAsync(formatterContext);

            // Assert
            Assert.True(result.HasError);
            Assert.True(!formatterContext.ModelState.IsValid);
            Assert.True(formatterContext.ModelState.ContainsKey(modelStateKey));

            var modelError = formatterContext.ModelState[modelStateKey].Errors.Single();
            Assert.Equal(expectedMessage, modelError.ErrorMessage);
        }

        [Fact]
        public async Task ReadAsync_DoNotAllowInputFormatterExceptionMessages_DoesNotWrapJsonInputExceptions()
        {
            // Arrange
            var formatter = CreateFormatter(allowInputFormatterExceptionMessages: false);
            var contentBytes = Encoding.UTF8.GetBytes("{");
            var httpContext = GetHttpContext(contentBytes);

            var formatterContext = CreateInputFormatterContext(typeof(User), httpContext);

            // Act
            var result = await formatter.ReadAsync(formatterContext);

            // Assert
            Assert.True(result.HasError);
            Assert.True(!formatterContext.ModelState.IsValid);
            Assert.True(formatterContext.ModelState.ContainsKey(string.Empty));

            var modelError = formatterContext.ModelState[string.Empty].Errors.Single();
            Assert.IsNotType<InputFormatterException>(modelError.Exception);
            Assert.Empty(modelError.ErrorMessage);
        }

        [Fact]
        public async Task ReadAsync_lowInputFormatterExceptionMessages_DoesNotWrapJsonInputExceptions()
        {
            // Arrange
            var formatter = new NewtonsoftJsonInputFormatter(
                GetLogger(),
                _serializerSettings,
                ArrayPool<char>.Shared,
                _objectPoolProvider,
                new MvcOptions(),
                new MvcNewtonsoftJsonOptions()
                {
                    AllowInputFormatterExceptionMessages = true,
                });

            var contentBytes = Encoding.UTF8.GetBytes("{");
            var httpContext = GetHttpContext(contentBytes);

            var formatterContext = CreateInputFormatterContext(typeof(User), httpContext);

            // Act
            var result = await formatter.ReadAsync(formatterContext);

            // Assert
            Assert.True(result.HasError);
            Assert.True(!formatterContext.ModelState.IsValid);
            Assert.True(formatterContext.ModelState.ContainsKey(string.Empty));

            var modelError = formatterContext.ModelState[string.Empty].Errors.Single();
            Assert.Null(modelError.Exception);
            Assert.NotEmpty(modelError.ErrorMessage);
        }

        private class TestableJsonInputFormatter : NewtonsoftJsonInputFormatter
        {
            public TestableJsonInputFormatter(JsonSerializerSettings settings)
                : base(GetLogger(), settings, ArrayPool<char>.Shared, _objectPoolProvider, new MvcOptions(), new MvcNewtonsoftJsonOptions())
            {
            }

            public new JsonSerializerSettings SerializerSettings => base.SerializerSettings;

            public new JsonSerializer CreateJsonSerializer(InputFormatterContext _) => base.CreateJsonSerializer(null);
        }

        private static ILogger GetLogger()
        {
            return NullLogger.Instance;
        }

        protected override TextInputFormatter GetInputFormatter()
            => CreateFormatter(allowInputFormatterExceptionMessages: true);

        private NewtonsoftJsonInputFormatter CreateFormatter(JsonSerializerSettings serializerSettings = null, bool allowInputFormatterExceptionMessages = false)
        {
            return new NewtonsoftJsonInputFormatter(
                GetLogger(),
                serializerSettings ?? _serializerSettings,
                ArrayPool<char>.Shared,
                _objectPoolProvider,
                new MvcOptions(),
                new MvcNewtonsoftJsonOptions()
                {
                    AllowInputFormatterExceptionMessages = allowInputFormatterExceptionMessages,
                });
        }

        private class Location
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        private class TestResponseFeature : HttpResponseFeature
        {
            public override void OnCompleted(Func<object, Task> callback, object state)
            {
                // do not do anything
            }
        }

        private sealed class User
        {
            public string Name { get; set; }

            public decimal Age { get; set; }

            public byte Small { get; set; }

            public UserLogin Login { get; set; }
        }

        private sealed class UserLogin
        {
            [JsonProperty(Required = Required.Always)]
            public string UserName { get; set; }

            [JsonProperty(Required = Required.Always)]
            public string Password { get; set; }
        }
    }
}
