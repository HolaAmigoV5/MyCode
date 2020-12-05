﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Microsoft.AspNetCore.HttpsPolicy.Tests
{
    public class HttpsRedirectionMiddlewareTests
    {
        [Fact]
        public async Task SetOptions_NotEnabledByDefault()
        {
            var sink = new TestSink(
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>,
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>);
            var loggerFactory = new TestLoggerFactory(sink, enabled: true);
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ILoggerFactory>(loggerFactory);
                })
                .Configure(app =>
                {
                    app.UseHttpsRedirection();
                    app.Run(context =>
                    {
                        return context.Response.WriteAsync("Hello world");
                    });
                });

            var server = new TestServer(builder);
            var client = server.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "");

            var response = await client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var logMessages = sink.Writes.ToList();

            Assert.Single(logMessages);
            var message = logMessages.Single();
            Assert.Equal(LogLevel.Warning, message.LogLevel);
            Assert.Equal("Failed to determine the https port for redirect.", message.State.ToString());
        }

        [Theory]
        [InlineData(302, 5001, "https://localhost:5001/")]
        [InlineData(307, 1, "https://localhost:1/")]
        [InlineData(308, 3449, "https://localhost:3449/")]
        [InlineData(301, 5050, "https://localhost:5050/")]
        [InlineData(301, 443, "https://localhost/")]
        public async Task SetOptions_SetStatusCodeHttpsPort(int statusCode, int? httpsPort, string expected)
        {
            var sink = new TestSink(
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>,
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>);
            var loggerFactory = new TestLoggerFactory(sink, enabled: true);
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ILoggerFactory>(loggerFactory);
                    services.Configure<HttpsRedirectionOptions>(options =>
                    {
                        options.RedirectStatusCode = statusCode;
                        options.HttpsPort = httpsPort;
                    });
                })
                .Configure(app =>
                {
                    app.UseHttpsRedirection();
                    app.Run(context =>
                    {
                        return context.Response.WriteAsync("Hello world");
                    });
                });

            var server = new TestServer(builder);
            var client = server.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "");

            var response = await client.SendAsync(request);

            Assert.Equal(statusCode, (int)response.StatusCode);
            Assert.Equal(expected, response.Headers.Location.ToString());

            var logMessages = sink.Writes.ToList();

            Assert.Single(logMessages);
            var message = logMessages.Single();
            Assert.Equal(LogLevel.Debug, message.LogLevel);
            Assert.Equal($"Redirecting to '{expected}'.", message.State.ToString());
        }

        [Theory]
        [InlineData(302, 5001, "https://localhost:5001/")]
        [InlineData(307, 1, "https://localhost:1/")]
        [InlineData(308, 3449, "https://localhost:3449/")]
        [InlineData(301, 5050, "https://localhost:5050/")]
        [InlineData(301, 443, "https://localhost/")]
        public async Task SetOptionsThroughHelperMethod_SetStatusCodeAndHttpsPort(int statusCode, int? httpsPort, string expectedUrl)
        {
            var sink = new TestSink(
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>,
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>);
            var loggerFactory = new TestLoggerFactory(sink, enabled: true);
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ILoggerFactory>(loggerFactory);
                    services.AddHttpsRedirection(options =>
                    {
                        options.RedirectStatusCode = statusCode;
                        options.HttpsPort = httpsPort;
                    });
                })
                .Configure(app =>
                {
                    app.UseHttpsRedirection();
                    app.Run(context =>
                    {
                        return context.Response.WriteAsync("Hello world");
                    });
                });

            var server = new TestServer(builder);
            var client = server.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "");

            var response = await client.SendAsync(request);

            Assert.Equal(statusCode, (int)response.StatusCode);
            Assert.Equal(expectedUrl, response.Headers.Location.ToString());

            var logMessages = sink.Writes.ToList();

            Assert.Single(logMessages);
            var message = logMessages.Single();
            Assert.Equal(LogLevel.Debug, message.LogLevel);
            Assert.Equal($"Redirecting to '{expectedUrl}'.", message.State.ToString());
        }

        [Theory]
        [InlineData(null, null, "https://localhost:4444/", "https://localhost:4444/")]
        [InlineData(null, null, "https://localhost:443/", "https://localhost/")]
        [InlineData(null, null, "https://localhost/", "https://localhost/")]
        [InlineData(null, "5000", "https://localhost:4444/", "https://localhost:5000/")]
        [InlineData(null, "443", "https://localhost:4444/", "https://localhost/")]
        [InlineData(443, "5000", "https://localhost:4444/", "https://localhost/")]
        [InlineData(4000, "5000", "https://localhost:4444/", "https://localhost:4000/")]
        [InlineData(5000, null, "https://localhost:4444/", "https://localhost:5000/")]
        public async Task SetHttpsPortEnvironmentVariableAndServerFeature_ReturnsCorrectStatusCodeOnResponse(
            int? optionsHttpsPort, string configHttpsPort, string serverAddressFeatureUrl, string expectedUrl)
        {
            var builder = new WebHostBuilder()
               .ConfigureServices(services =>
               {
                   services.AddHttpsRedirection(options =>
                   {
                       options.HttpsPort = optionsHttpsPort;
                   });
               })
               .Configure(app =>
               {
                   app.UseHttpsRedirection();
                   app.Run(context =>
                   {
                       return context.Response.WriteAsync("Hello world");
                   });
               });

            builder.UseSetting("HTTPS_PORT", configHttpsPort);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IServerAddressesFeature>(new ServerAddressesFeature());

            var server = new TestServer(builder, featureCollection);
            if (serverAddressFeatureUrl != null)
            {
                server.Features.Get<IServerAddressesFeature>().Addresses.Add(serverAddressFeatureUrl);
            }

            var client = server.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "");

            var response = await client.SendAsync(request);

            Assert.Equal(expectedUrl, response.Headers.Location.ToString());
        }

        [Fact]
        public async Task SetServerAddressesFeature_SingleHttpsAddress_Success()
        {
            var sink = new TestSink(
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>,
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>);
            var loggerFactory = new TestLoggerFactory(sink, enabled: true);
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ILoggerFactory>(loggerFactory);
                })
               .Configure(app =>
               {
                   app.UseHttpsRedirection();
                   app.Run(context =>
                   {
                       return context.Response.WriteAsync("Hello world");
                   });
               });

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IServerAddressesFeature>(new ServerAddressesFeature());
            var server = new TestServer(builder, featureCollection);

            server.Features.Get<IServerAddressesFeature>().Addresses.Add("https://localhost:5050");
            var client = server.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "");

            var response = await client.SendAsync(request);

            Assert.Equal("https://localhost:5050/", response.Headers.Location.ToString());

            var logMessages = sink.Writes.ToList();

            Assert.Equal(2, logMessages.Count);
            var message = logMessages.First();
            Assert.Equal(LogLevel.Debug, message.LogLevel);
            Assert.Equal("Https port '5050' discovered from server endpoints.", message.State.ToString());

            message = logMessages.Skip(1).First();
            Assert.Equal(LogLevel.Debug, message.LogLevel);
            Assert.Equal("Redirecting to 'https://localhost:5050/'.", message.State.ToString());
        }

        [Fact]
        public async Task SetServerAddressesFeature_MultipleHttpsAddresses_LogsAndFailsToRedirect()
        {
            var sink = new TestSink(
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>,
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>);
            var loggerFactory = new TestLoggerFactory(sink, enabled: true);
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ILoggerFactory>(loggerFactory);
                })
               .Configure(app =>
               {
                   app.UseHttpsRedirection();
                   app.Run(context =>
                   {
                       return context.Response.WriteAsync("Hello world");
                   });
               });

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IServerAddressesFeature>(new ServerAddressesFeature());
            var server = new TestServer(builder, featureCollection);

            server.Features.Get<IServerAddressesFeature>().Addresses.Add("https://localhost:5050");
            server.Features.Get<IServerAddressesFeature>().Addresses.Add("https://localhost:5051");

            var client = server.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "");

            var response = await client.SendAsync(request);
            Assert.Equal(200, (int)response.StatusCode);

            var logMessages = sink.Writes.ToList();

            Assert.Single(logMessages);
            var message = logMessages.First();
            Assert.Equal(LogLevel.Warning, message.LogLevel);
            Assert.Equal("Cannot determine the https port from IServerAddressesFeature, multiple values were found. " +
                "Please set the desired port explicitly on HttpsRedirectionOptions.HttpsPort.", message.State.ToString());
        }

        [Fact]
        public async Task SetServerAddressesFeature_MultipleHttpsAddressesWithSamePort_Success()
        {
            var sink = new TestSink(
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>,
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>);
            var loggerFactory = new TestLoggerFactory(sink, enabled: true);
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ILoggerFactory>(loggerFactory);
                })
               .Configure(app =>
               {
                   app.UseHttpsRedirection();
                   app.Run(context =>
                   {
                       return context.Response.WriteAsync("Hello world");
                   });
               });

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IServerAddressesFeature>(new ServerAddressesFeature());
            var server = new TestServer(builder, featureCollection);

            server.Features.Get<IServerAddressesFeature>().Addresses.Add("https://localhost:5050");
            server.Features.Get<IServerAddressesFeature>().Addresses.Add("https://example.com:5050");

            var client = server.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "");

            var response = await client.SendAsync(request);

            Assert.Equal("https://localhost:5050/", response.Headers.Location.ToString());

            var logMessages = sink.Writes.ToList();

            Assert.Equal(2, logMessages.Count);
            var message = logMessages.First();
            Assert.Equal(LogLevel.Debug, message.LogLevel);
            Assert.Equal("Https port '5050' discovered from server endpoints.", message.State.ToString());

            message = logMessages.Skip(1).First();
            Assert.Equal(LogLevel.Debug, message.LogLevel);
            Assert.Equal("Redirecting to 'https://localhost:5050/'.", message.State.ToString());
        }

        [Fact]
        public async Task NoServerAddressFeature_DoesNotThrow_DoesNotRedirect()
        {
            var sink = new TestSink(
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>,
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>);
            var loggerFactory = new TestLoggerFactory(sink, enabled: true);
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ILoggerFactory>(loggerFactory);
                })
                .Configure(app =>
                {
                    app.UseHttpsRedirection();
                    app.Run(context =>
                    {
                        return context.Response.WriteAsync("Hello world");
                    });
                });

            var server = new TestServer(builder);
            var client = server.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "");
            var response = await client.SendAsync(request);
            Assert.Equal(200, (int)response.StatusCode);

            var logMessages = sink.Writes.ToList();

            Assert.Single(logMessages);
            var message = logMessages.First();
            Assert.Equal(LogLevel.Warning, message.LogLevel);
            Assert.Equal("Failed to determine the https port for redirect.", message.State.ToString());
        }

        [Fact]
        public async Task SetNullAddressFeature_DoesNotThrow()
        {
            var sink = new TestSink(
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>,
                TestSink.EnableWithTypeName<HttpsRedirectionMiddleware>);
            var loggerFactory = new TestLoggerFactory(sink, enabled: true);
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ILoggerFactory>(loggerFactory);
                })
                .Configure(app =>
                {
                    app.UseHttpsRedirection();
                    app.Run(context =>
                    {
                        return context.Response.WriteAsync("Hello world");
                    });
                });

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IServerAddressesFeature>(null);
            var server = new TestServer(builder, featureCollection);

            var client = server.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "");
            var response = await client.SendAsync(request);
            Assert.Equal(200, (int)response.StatusCode);

            var logMessages = sink.Writes.ToList();

            Assert.Single(logMessages);
            var message = logMessages.First();
            Assert.Equal(LogLevel.Warning, message.LogLevel);
            Assert.Equal("Failed to determine the https port for redirect.", message.State.ToString());
        }
    }
}
