﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Microsoft.AspNetCore.HttpOverrides
{
    public class ForwardedHeadersMiddlewareTests
    {
        [Fact]
        public async Task XForwardedForDefaultSettingsChangeRemoteIpAndPort()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedFor
                    });
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-For"] = "11.111.111.11:9090";
            });

            Assert.Equal("11.111.111.11", context.Connection.RemoteIpAddress.ToString());
            Assert.Equal(9090, context.Connection.RemotePort);
            // No Original set if RemoteIpAddress started null.
            Assert.False(context.Request.Headers.ContainsKey("X-Original-For"));
            // Should have been consumed and removed
            Assert.False(context.Request.Headers.ContainsKey("X-Forwarded-For"));
        }

        [Theory]
        [InlineData(1, "11.111.111.11.12345", "10.0.0.1", 99)] // Invalid
        public async Task XForwardedForFirstValueIsInvalid(int limit, string header, string expectedIp, int expectedPort)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedFor,
                        ForwardLimit = limit,
                    });
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-For"] = header;
                c.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.1");
                c.Connection.RemotePort = 99;
            });

            Assert.Equal(expectedIp, context.Connection.RemoteIpAddress.ToString());
            Assert.Equal(expectedPort, context.Connection.RemotePort);
            Assert.False(context.Request.Headers.ContainsKey("X-Original-For"));
            Assert.True(context.Request.Headers.ContainsKey("X-Forwarded-For"));
            Assert.Equal(header, context.Request.Headers["X-Forwarded-For"]);
        }

        [Theory]
        [InlineData(1, "11.111.111.11:12345", "11.111.111.11", 12345, "", false)]
        [InlineData(1, "11.111.111.11:12345", "11.111.111.11", 12345, "", true)]
        [InlineData(10, "11.111.111.11:12345", "11.111.111.11", 12345, "", false)]
        [InlineData(10, "11.111.111.11:12345", "11.111.111.11", 12345, "", true)]
        [InlineData(1, "12.112.112.12:23456, 11.111.111.11:12345", "11.111.111.11", 12345, "12.112.112.12:23456", false)]
        [InlineData(1, "12.112.112.12:23456, 11.111.111.11:12345", "11.111.111.11", 12345, "12.112.112.12:23456", true)]
        [InlineData(2, "12.112.112.12:23456, 11.111.111.11:12345", "12.112.112.12", 23456, "", false)]
        [InlineData(2, "12.112.112.12:23456, 11.111.111.11:12345", "12.112.112.12", 23456, "", true)]
        [InlineData(10, "12.112.112.12:23456, 11.111.111.11:12345", "12.112.112.12", 23456, "", false)]
        [InlineData(10, "12.112.112.12:23456, 11.111.111.11:12345", "12.112.112.12", 23456, "", true)]
        [InlineData(10, "12.112.112.12.23456, 11.111.111.11:12345", "11.111.111.11", 12345, "12.112.112.12.23456", false)] // Invalid 2nd value
        [InlineData(10, "12.112.112.12.23456, 11.111.111.11:12345", "11.111.111.11", 12345, "12.112.112.12.23456", true)] // Invalid 2nd value
        [InlineData(10, "13.113.113.13:34567, 12.112.112.12.23456, 11.111.111.11:12345", "11.111.111.11", 12345, "13.113.113.13:34567,12.112.112.12.23456", false)] // Invalid 2nd value
        [InlineData(10, "13.113.113.13:34567, 12.112.112.12.23456, 11.111.111.11:12345", "11.111.111.11", 12345, "13.113.113.13:34567,12.112.112.12.23456", true)] // Invalid 2nd value
        [InlineData(2, "13.113.113.13:34567, 12.112.112.12:23456, 11.111.111.11:12345", "12.112.112.12", 23456, "13.113.113.13:34567", false)]
        [InlineData(2, "13.113.113.13:34567, 12.112.112.12:23456, 11.111.111.11:12345", "12.112.112.12", 23456, "13.113.113.13:34567", true)]
        [InlineData(3, "13.113.113.13:34567, 12.112.112.12:23456, 11.111.111.11:12345", "13.113.113.13", 34567, "", false)]
        [InlineData(3, "13.113.113.13:34567, 12.112.112.12:23456, 11.111.111.11:12345", "13.113.113.13", 34567, "", true)]
        public async Task XForwardedForForwardLimit(int limit, string header, string expectedIp, int expectedPort, string remainingHeader, bool requireSymmetry)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    var options = new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedFor,
                        RequireHeaderSymmetry = requireSymmetry,
                        ForwardLimit = limit,
                    };
                    options.KnownProxies.Clear();
                    options.KnownNetworks.Clear();
                    app.UseForwardedHeaders(options);
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-For"] = header;
                c.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.1");
                c.Connection.RemotePort = 99;
            });

            Assert.Equal(expectedIp, context.Connection.RemoteIpAddress.ToString());
            Assert.Equal(expectedPort, context.Connection.RemotePort);
            Assert.Equal(remainingHeader, context.Request.Headers["X-Forwarded-For"].ToString());
        }

        [Theory]
        [InlineData("11.111.111.11", false)]
        [InlineData("127.0.0.1", true)]
        [InlineData("127.0.1.1", true)]
        [InlineData("::1", true)]
        [InlineData("::", false)]
        public async Task XForwardedForLoopback(string originalIp, bool expectForwarded)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedFor,
                    });
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-For"] = "10.0.0.1:1234";
                c.Connection.RemoteIpAddress = IPAddress.Parse(originalIp);
                c.Connection.RemotePort = 99;
            });

            if (expectForwarded)
            {
                Assert.Equal("10.0.0.1", context.Connection.RemoteIpAddress.ToString());
                Assert.Equal(1234, context.Connection.RemotePort);
                Assert.True(context.Request.Headers.ContainsKey("X-Original-For"));
                Assert.Equal(new IPEndPoint(IPAddress.Parse(originalIp), 99).ToString(),
                    context.Request.Headers["X-Original-For"]);
            }
            else
            {
                Assert.Equal(originalIp, context.Connection.RemoteIpAddress.ToString());
                Assert.Equal(99, context.Connection.RemotePort);
                Assert.False(context.Request.Headers.ContainsKey("X-Original-For"));
            }
        }

        [Theory]
        [InlineData(1, "11.111.111.11:12345", "20.0.0.1", "10.0.0.1", 99, false)]
        [InlineData(1, "11.111.111.11:12345", "20.0.0.1", "10.0.0.1", 99, true)]
        [InlineData(1, "", "10.0.0.1", "10.0.0.1", 99, false)]
        [InlineData(1, "", "10.0.0.1", "10.0.0.1", 99, true)]
        [InlineData(1, "11.111.111.11:12345", "10.0.0.1", "11.111.111.11", 12345, false)]
        [InlineData(1, "11.111.111.11:12345", "10.0.0.1", "11.111.111.11", 12345, true)]
        [InlineData(1, "12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1", "11.111.111.11", 12345, false)]
        [InlineData(1, "12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1", "11.111.111.11", 12345, true)]
        [InlineData(1, "12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11", "11.111.111.11", 12345, false)]
        [InlineData(1, "12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11", "11.111.111.11", 12345, true)]
        [InlineData(2, "12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11", "12.112.112.12", 23456, false)]
        [InlineData(2, "12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11", "12.112.112.12", 23456, true)]
        [InlineData(1, "12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11,12.112.112.12", "11.111.111.11", 12345, false)]
        [InlineData(1, "12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11,12.112.112.12", "11.111.111.11", 12345, true)]
        [InlineData(2, "12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11,12.112.112.12", "12.112.112.12", 23456, false)]
        [InlineData(2, "12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11,12.112.112.12", "12.112.112.12", 23456, true)]
        [InlineData(3, "13.113.113.13:34567, 12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11,12.112.112.12", "13.113.113.13", 34567, false)]
        [InlineData(3, "13.113.113.13:34567, 12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11,12.112.112.12", "13.113.113.13", 34567, true)]
        [InlineData(3, "13.113.113.13:34567, 12.112.112.12;23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11,12.112.112.12", "11.111.111.11", 12345, false)] // Invalid 2nd IP
        [InlineData(3, "13.113.113.13:34567, 12.112.112.12;23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11,12.112.112.12", "11.111.111.11", 12345, true)] // Invalid 2nd IP
        [InlineData(3, "13.113.113.13;34567, 12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11,12.112.112.12", "12.112.112.12", 23456, false)] // Invalid 3rd IP
        [InlineData(3, "13.113.113.13;34567, 12.112.112.12:23456, 11.111.111.11:12345", "10.0.0.1,11.111.111.11,12.112.112.12", "12.112.112.12", 23456, true)] // Invalid 3rd IP
        public async Task XForwardedForForwardKnownIps(int limit, string header, string knownIPs, string expectedIp, int expectedPort, bool requireSymmetry)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    var options = new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedFor,
                        RequireHeaderSymmetry = requireSymmetry,
                        ForwardLimit = limit,
                    };
                    foreach (var ip in knownIPs.Split(',').Select(text => IPAddress.Parse(text)))
                    {
                        options.KnownProxies.Add(ip);
                    }
                    app.UseForwardedHeaders(options);
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-For"] = header;
                c.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.1");
                c.Connection.RemotePort = 99;
            });

            Assert.Equal(expectedIp, context.Connection.RemoteIpAddress.ToString());
            Assert.Equal(expectedPort, context.Connection.RemotePort);
        }

        [Fact]
        public async Task XForwardedForOverrideBadIpDoesntChangeRemoteIp()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedFor
                    });
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-For"] = "BAD-IP";
            });

            Assert.Null(context.Connection.RemoteIpAddress);
        }

        [Fact]
        public async Task XForwardedHostOverrideChangesRequestHost()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedHost
                    });
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-Host"] = "testhost";
            });

            Assert.Equal("testhost", context.Request.Host.ToString());
        }

        public static TheoryData<string> HostHeaderData
        {
            get
            {
                return new TheoryData<string>() {
                    "z",
                    "1",
                    "y:1",
                    "1:1",
                    "[ABCdef]",
                    "[abcDEF]:0",
                    "[abcdef:127.2355.1246.114]:0",
                    "[::1]:80",
                    "127.0.0.1:80",
                    "900.900.900.900:9523547852",
                    "foo",
                    "foo:234",
                    "foo.bar.baz",
                    "foo.BAR.baz:46245",
                    "foo.ba-ar.baz:46245",
                    "-foo:1234",
                    "xn--c1yn36f:134",
                    "-",
                    "_",
                    "~",
                    "!",
                    "$",
                    "'",
                    "(",
                    ")",
                };
            }
        }

        [Theory]
        [MemberData(nameof(HostHeaderData))]
        public async Task XForwardedHostAllowsValidCharacters(string host)
        {
            var assertsExecuted = false;

            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedHost
                    });
                    app.Run(context =>
                    {
                        Assert.Equal(host, context.Request.Host.ToString());
                        assertsExecuted = true;
                        return Task.FromResult(0);
                    });
                });
            var server = new TestServer(builder);

            await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-Host"] = host;
            });
            Assert.True(assertsExecuted);
        }

        public static TheoryData<string> HostHeaderInvalidData
        {
            get
            {
                // see https://tools.ietf.org/html/rfc7230#section-5.4
                var data = new TheoryData<string>() {
                    "", // Empty
                    "[]", // Too short
                    "[::]", // Too short
                    "[ghijkl]", // Non-hex
                    "[afd:adf:123", // Incomplete
                    "[afd:adf]123", // Missing :
                    "[afd:adf]:", // Missing port digits
                    "[afd adf]", // Space
                    "[ad-314]", // dash
                    ":1234", // Missing host
                    "a:b:c", // Missing []
                    "::1", // Missing []
                    "::", // Missing everything
                    "abcd:1abcd", // Letters in port
                    "abcd:1.2", // Dot in port
                    "1.2.3.4:", // Missing port digits
                    "1.2 .4", // Space
                };

                // These aren't allowed anywhere in the host header
                var invalid = "\"#%*+/;<=>?@[]\\^`{}|";
                foreach (var ch in invalid)
                {
                    data.Add(ch.ToString());
                }

                invalid = "!\"#$%&'()*+,/;<=>?@[]\\^_`{}|~-";
                foreach (var ch in invalid)
                {
                    data.Add("[abd" + ch + "]:1234");
                }

                invalid = "!\"#$%&'()*+/;<=>?@[]\\^_`{}|~:abcABC-.";
                foreach (var ch in invalid)
                {
                    data.Add("a.b.c:" + ch);
                }

                return data;
            }
        }

        [Theory]
        [MemberData(nameof(HostHeaderInvalidData))]
        public async Task XForwardedHostFailsForInvalidCharacters(string host)
        {
            var assertsExecuted = false;

            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedHost
                    });
                    app.Run(context =>
                    {
                        Assert.NotEqual(host, context.Request.Host.Value);
                        assertsExecuted = true;
                        return Task.FromResult(0);
                    });
                });
            var server = new TestServer(builder);

            await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-Host"] = host;
            });
            Assert.True(assertsExecuted);
        }

        [Theory]
        [InlineData("localHost", "localhost")]
        [InlineData("localHost", "*")] // Any - Used by HttpSys
        [InlineData("localHost", "[::]")] // IPv6 Any - This is what Kestrel reports when binding to *
        [InlineData("localHost", "0.0.0.0")] // IPv4 Any
        [InlineData("localhost:9090", "example.com;localHost")]
        [InlineData("example.com:443", "example.com;localhost")]
        [InlineData("localHost:80", "localhost;")]
        [InlineData("foo.eXample.com:443", "*.exampLe.com")]
        [InlineData("f.eXample.com:443", "*.exampLe.com")]
        [InlineData("127.0.0.1", "127.0.0.1")]
        [InlineData("127.0.0.1:443", "127.0.0.1")]
        [InlineData("xn--c1yn36f:443", "xn--c1yn36f")]
        [InlineData("xn--c1yn36f:443", "點看")]
        [InlineData("[::ABC]", "[::aBc]")]
        [InlineData("[::1]:80", "[::1]")]
        public async Task XForwardedHostAllowsSpecifiedHost(string host, string allowedHost)
        {
            bool assertsExecuted = false;
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedHost,
                        AllowedHosts = allowedHost.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    });
                    app.Run(context =>
                    {
                        Assert.Equal(host, context.Request.Headers[HeaderNames.Host]);
                        assertsExecuted = true;
                        return Task.FromResult(0);
                    });
                });
            var server = new TestServer(builder);
            var response = await server.SendAsync(ctx =>
            {
                ctx.Request.Headers["X-forwarded-Host"] = host;
            });
            Assert.True(assertsExecuted);
        }

        [Theory]
        [InlineData("example.com", "localhost")]
        [InlineData("localhost:9090", "example.com;")]
        [InlineData(";", "example.com;localhost")]
        [InlineData(";:80", "example.com;localhost")]
        [InlineData(":80", "localhost")]
        [InlineData(":", "localhost")]
        [InlineData("example.com:443", "*.example.com")]
        [InlineData(".example.com:443", "*.example.com")]
        [InlineData("foo.com:443", "*.example.com")]
        [InlineData("foo.example.com.bar:443", "*.example.com")]
        [InlineData(".com:443", "*.com")]
        // Unicode in the host shouldn't be allowed without punycode anyways. This match fails because the middleware converts
        // its input to punycode.
        [InlineData("點看", "點看")]
        [InlineData("[::1", "[::1]")]
        [InlineData("[::1:80", "[::1]")]
        public async Task XForwardedHostFailsMismatchedHosts(string host, string allowedHost)
        {
            bool assertsExecuted = false;
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedHost,
                        AllowedHosts = new[] { allowedHost }
                    });
                    app.Run(context =>
                    {
                        Assert.NotEqual<string>(host, context.Request.Headers[HeaderNames.Host]);
                        assertsExecuted = true;
                        return Task.FromResult(0);
                    });
                });
            var server = new TestServer(builder);
            var response = await server.SendAsync(ctx =>
            {
                ctx.Request.Headers["X-forwarded-Host"] = host;
            });
            Assert.True(assertsExecuted);
        }

        [Fact]
        public async Task XForwardedHostStopsAtFirstUnspecifiedHost()
        {
            bool assertsExecuted = false;
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedHost,
                        ForwardLimit = 10,
                        AllowedHosts = new[] { "bar.com", "*.foo.com" }
                    });
                    app.Run(context =>
                    {
                        Assert.Equal("bar.foo.com:432", context.Request.Headers[HeaderNames.Host]);
                        assertsExecuted = true;
                        return Task.FromResult(0);
                    });
                });
            var server = new TestServer(builder);
            var response = await server.SendAsync(ctx =>
            {
                ctx.Request.Headers["X-forwarded-Host"] = "stuff:523, bar.foo.com:432, bar.com:80";
            });
            Assert.True(assertsExecuted);
        }

        [Theory]
        [InlineData(0, "h1", "http")]
        [InlineData(1, "", "http")]
        [InlineData(1, "h1", "h1")]
        [InlineData(3, "h1", "h1")]
        [InlineData(1, "h2, h1", "h1")]
        [InlineData(2, "h2, h1", "h2")]
        [InlineData(10, "h3, h2, h1", "h3")]
        public async Task XForwardedProtoOverrideChangesRequestProtocol(int limit, string header, string expected)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedProto,
                        ForwardLimit = limit,
                    });
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-Proto"] = header;
            });

            Assert.Equal(expected, context.Request.Scheme);
        }

        public static TheoryData<string> ProtoHeaderData
        {
            get
            {
                // ALPHA *( ALPHA / DIGIT / "+" / "-" / "." )
                return new TheoryData<string>() {
                    "z",
                    "Z",
                    "1",
                    "y+",
                    "1-",
                    "a.",
                };
            }
        }

        [Theory]
        [MemberData(nameof(ProtoHeaderData))]
        public async Task XForwardedProtoAcceptsValidProtocols(string scheme)
        {
            var assertsExecuted = false;

            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedProto
                    });
                    app.Run(context =>
                    {
                        Assert.Equal(scheme, context.Request.Scheme);
                        assertsExecuted = true;
                        return Task.FromResult(0);
                    });
                });
            var server = new TestServer(builder);

            await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-Proto"] = scheme;
            });
            Assert.True(assertsExecuted);
        }

        public static TheoryData<string> ProtoHeaderInvalidData
        {
            get
            {
                // ALPHA *( ALPHA / DIGIT / "+" / "-" / "." )
                var data = new TheoryData<string>() {
                    "a b", // Space
                };

                // These aren't allowed anywhere in the scheme header
                var invalid = "!\"#$%&'()*/:;<=>?@[]\\^_`{}|~";
                foreach (var ch in invalid)
                {
                    data.Add(ch.ToString());
                }

                return data;
            }
        }

        [Theory]
        [MemberData(nameof(ProtoHeaderInvalidData))]
        public async Task XForwardedProtoRejectsInvalidProtocols(string scheme)
        {
            var assertsExecuted = false;

            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedProto,
                    });
                    app.Run(context =>
                    {
                        Assert.Equal("http", context.Request.Scheme);
                        assertsExecuted = true;
                        return Task.FromResult(0);
                    });
                });
            var server = new TestServer(builder);

            await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-Proto"] = scheme;
            });
            Assert.True(assertsExecuted);
        }

        [Theory]
        [InlineData(0, "h1", "::1", "http")]
        [InlineData(1, "", "::1", "http")]
        [InlineData(1, "h1", "::1", "h1")]
        [InlineData(3, "h1", "::1", "h1")]
        [InlineData(3, "h2, h1", "::1", "http")]
        [InlineData(5, "h2, h1", "::1, ::1", "h2")]
        [InlineData(10, "h3, h2, h1", "::1, ::1, ::1", "h3")]
        [InlineData(10, "h3, h2, h1", "::1, badip, ::1", "h1")]
        public async Task XForwardedProtoOverrideLimitedByXForwardedForCount(int limit, string protoHeader, string forHeader, string expected)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor,
                        RequireHeaderSymmetry = true,
                        ForwardLimit = limit,
                    });
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-Proto"] = protoHeader;
                c.Request.Headers["X-Forwarded-For"] = forHeader;
            });

            Assert.Equal(expected, context.Request.Scheme);
        }

        [Theory]
        [InlineData(0, "h1", "::1", "http")]
        [InlineData(1, "", "::1", "http")]
        [InlineData(1, "h1", "", "h1")]
        [InlineData(1, "h1", "::1", "h1")]
        [InlineData(3, "h1", "::1", "h1")]
        [InlineData(3, "h1", "::1, ::1", "h1")]
        [InlineData(3, "h2, h1", "::1", "h2")]
        [InlineData(5, "h2, h1", "::1, ::1", "h2")]
        [InlineData(10, "h3, h2, h1", "::1, ::1, ::1", "h3")]
        [InlineData(10, "h3, h2, h1", "::1, badip, ::1", "h1")]
        public async Task XForwardedProtoOverrideCanBeIndependentOfXForwardedForCount(int limit, string protoHeader, string forHeader, string expected)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor,
                        RequireHeaderSymmetry = false,
                        ForwardLimit = limit,
                    });
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-Proto"] = protoHeader;
                c.Request.Headers["X-Forwarded-For"] = forHeader;
            });

            Assert.Equal(expected, context.Request.Scheme);
        }

        [Theory]
        [InlineData("", "", "::1", false, "http")]
        [InlineData("h1", "", "::1", false, "http")]
        [InlineData("h1", "F::", "::1", false, "h1")]
        [InlineData("h1", "F::", "E::", false, "h1")]
        [InlineData("", "", "::1", true, "http")]
        [InlineData("h1", "", "::1", true, "http")]
        [InlineData("h1", "F::", "::1", true, "h1")]
        [InlineData("h1", "", "F::", true, "http")]
        [InlineData("h1", "E::", "F::", true, "http")]
        [InlineData("h2, h1", "", "::1", true, "http")]
        [InlineData("h2, h1", "F::, D::", "::1", true, "h1")]
        [InlineData("h2, h1", "E::, D::", "F::", true, "http")]
        public async Task XForwardedProtoOverrideLimitedByLoopback(string protoHeader, string forHeader, string remoteIp, bool loopback, string expected)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    var options = new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor,
                        RequireHeaderSymmetry = true,
                        ForwardLimit = 5,
                    };
                    if (!loopback)
                    {
                        options.KnownNetworks.Clear();
                        options.KnownProxies.Clear();
                    }
                    app.UseForwardedHeaders(options);
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-Proto"] = protoHeader;
                c.Request.Headers["X-Forwarded-For"] = forHeader;
                c.Connection.RemoteIpAddress = IPAddress.Parse(remoteIp);
            });

            Assert.Equal(expected, context.Request.Scheme);
        }

        [Fact]
        public void AllForwardsDisabledByDefault()
        {
            var options = new ForwardedHeadersOptions();
            Assert.True(options.ForwardedHeaders == ForwardedHeaders.None);
            Assert.Equal(1, options.ForwardLimit);
            Assert.Single(options.KnownNetworks);
            Assert.Single(options.KnownProxies);
        }

        [Fact]
        public async Task AllForwardsEnabledChangeRequestRemoteIpHostandProtocol()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.All
                    });
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-Proto"] = "Protocol";
                c.Request.Headers["X-Forwarded-For"] = "11.111.111.11";
                c.Request.Headers["X-Forwarded-Host"] = "testhost";
            });

            Assert.Equal("11.111.111.11", context.Connection.RemoteIpAddress.ToString());
            Assert.Equal("testhost", context.Request.Host.ToString());
            Assert.Equal("Protocol", context.Request.Scheme);
        }

        [Fact]
        public async Task AllOptionsDisabledRequestDoesntChange()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.None
                    });
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-Proto"] = "Protocol";
                c.Request.Headers["X-Forwarded-For"] = "11.111.111.11";
                c.Request.Headers["X-Forwarded-Host"] = "otherhost";
            });

            Assert.Null(context.Connection.RemoteIpAddress);
            Assert.Equal("localhost", context.Request.Host.ToString());
            Assert.Equal("http", context.Request.Scheme);
        }

        [Fact]
        public async Task PartiallyEnabledForwardsPartiallyChangesRequest()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                    });
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-Proto"] = "Protocol";
                c.Request.Headers["X-Forwarded-For"] = "11.111.111.11";
            });

            Assert.Equal("11.111.111.11", context.Connection.RemoteIpAddress.ToString());
            Assert.Equal("localhost", context.Request.Host.ToString());
            Assert.Equal("Protocol", context.Request.Scheme);
        }

        [Theory]
        [InlineData("22.33.44.55,::ffff:127.0.0.1", "", "", "22.33.44.55")]
        [InlineData("22.33.44.55,::ffff:172.123.142.121", "172.123.142.121", "", "22.33.44.55")]
        [InlineData("22.33.44.55,::ffff:172.123.142.121", "::ffff:172.123.142.121", "", "22.33.44.55")]
        [InlineData("22.33.44.55,::ffff:172.123.142.121,172.32.24.23", "", "172.0.0.0/8", "22.33.44.55")]
        [InlineData("2a00:1450:4009:802::200e,2a02:26f0:2d:183::356e,::ffff:172.123.142.121,172.32.24.23", "", "172.0.0.0/8,2a02:26f0:2d:183::1/64", "2a00:1450:4009:802::200e")]
        [InlineData("22.33.44.55,2a02:26f0:2d:183::356e,::ffff:127.0.0.1", "2a02:26f0:2d:183::356e", "", "22.33.44.55")]
        public async Task XForwardForIPv4ToIPv6Mapping(string forHeader, string knownProxies, string knownNetworks, string expectedRemoteIp)
        {
            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor,
                ForwardLimit = null,
            };

            foreach (var knownProxy in knownProxies.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                var proxy = IPAddress.Parse(knownProxy);
                options.KnownProxies.Add(proxy);
            }
            foreach (var knownNetwork in knownNetworks.Split(new string[] { "," }, options:StringSplitOptions.RemoveEmptyEntries))
            {
                var knownNetworkParts = knownNetwork.Split('/');
                var networkIp = IPAddress.Parse(knownNetworkParts[0]);
                var prefixLength = int.Parse(knownNetworkParts[1]);
                options.KnownNetworks.Add(new IPNetwork(networkIp, prefixLength));
            }

            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseForwardedHeaders(options);
                });
            var server = new TestServer(builder);

            var context = await server.SendAsync(c =>
            {
                c.Request.Headers["X-Forwarded-For"] = forHeader;
            });

            Assert.Equal(expectedRemoteIp, context.Connection.RemoteIpAddress.ToString());
        }
    }
}
