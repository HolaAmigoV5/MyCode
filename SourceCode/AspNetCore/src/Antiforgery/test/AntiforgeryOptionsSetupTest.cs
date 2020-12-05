// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.AspNetCore.Antiforgery.Internal
{
    public class AntiforgeryOptionsSetupTest
    {
        [Theory]
        [InlineData("HelloWorldApp", ".AspNetCore.Antiforgery.tGmK82_ckDw")]
        [InlineData("TodoCalendar", ".AspNetCore.Antiforgery.7mK1hBEBwYs")]
        public void AntiforgeryOptionsSetup_SetsDefaultCookieName_BasedOnApplicationId(
            string applicationId,
            string expectedCookieName)
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddAntiforgery();
            serviceCollection
                .AddDataProtection()
                .SetApplicationName(applicationId);

            var services = serviceCollection.BuildServiceProvider();
            var options = services.GetRequiredService<IOptions<AntiforgeryOptions>>();

            // Act
            var cookieName = options.Value.Cookie.Name;

            // Assert
            Assert.Equal(expectedCookieName, cookieName);
        }

        [Fact]
        public void AntiforgeryOptionsSetup_UserOptionsSetup_CanSetCookieName()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.Configure<AntiforgeryOptions>(o =>
            {
                Assert.Null(o.Cookie.Name);
                o.Cookie.Name = "antiforgery";
            });
            serviceCollection.AddAntiforgery();
            serviceCollection
                .AddDataProtection()
                .SetApplicationName("HelloWorldApp");

            var services = serviceCollection.BuildServiceProvider();
            var options = services.GetRequiredService<IOptions<AntiforgeryOptions>>();

            // Act
            var cookieName = options.Value.Cookie.Name;

            // Assert
            Assert.Equal("antiforgery", cookieName);
        }

        [Fact]
        public void AntiforgeryOptions_SetsCookieSecurePolicy_ToNone_ByDefault()
        {
            // Arrange & Act
            var options = new AntiforgeryOptions();

            // Assert
            Assert.Equal(CookieSecurePolicy.None, options.Cookie.SecurePolicy);
        }
    }
}
