// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Moq;
using Xunit;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;

namespace Microsoft.AspNetCore.DataProtection.KeyManagement
{
    public class KeyTests
    {
        [Fact]
        public void Ctor_Properties()
        {
            // Arrange
            var keyId = Guid.NewGuid();
            var creationDate = DateTimeOffset.Now;
            var activationDate = creationDate.AddDays(2);
            var expirationDate = creationDate.AddDays(90);
            var descriptor = Mock.Of<IAuthenticatedEncryptorDescriptor>();
            var encryptorFactory = Mock.Of<IAuthenticatedEncryptorFactory>();

            // Act
            var key = new Key(keyId, creationDate, activationDate, expirationDate, descriptor, new[] { encryptorFactory });

            // Assert
            Assert.Equal(keyId, key.KeyId);
            Assert.Equal(creationDate, key.CreationDate);
            Assert.Equal(activationDate, key.ActivationDate);
            Assert.Equal(expirationDate, key.ExpirationDate);
            Assert.Same(descriptor, key.Descriptor);
        }

        [Fact]
        public void SetRevoked_Respected()
        {
            // Arrange
            var now = DateTimeOffset.UtcNow;
            var encryptorFactory = Mock.Of<IAuthenticatedEncryptorFactory>();
            var key = new Key(Guid.Empty, now, now, now, new Mock<IAuthenticatedEncryptorDescriptor>().Object, new[] { encryptorFactory });

            // Act & assert
            Assert.False(key.IsRevoked);
            key.SetRevoked();
            Assert.True(key.IsRevoked);
        }
    }
}
