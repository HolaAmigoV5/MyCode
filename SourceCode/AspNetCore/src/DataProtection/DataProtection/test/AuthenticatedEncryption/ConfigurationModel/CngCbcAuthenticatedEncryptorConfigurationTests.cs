// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;

namespace Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel
{
    public class CngCbcAuthenticatedEncryptorConfigurationTests
    {
        [Fact]
        public void CreateNewDescriptor_CreatesUniqueCorrectlySizedMasterKey()
        {
            // Arrange
            var configuration = new CngCbcAuthenticatedEncryptorConfiguration();

            // Act
            var masterKey1 = ((CngCbcAuthenticatedEncryptorDescriptor)configuration.CreateNewDescriptor()).MasterKey;
            var masterKey2 = ((CngCbcAuthenticatedEncryptorDescriptor)configuration.CreateNewDescriptor()).MasterKey;

            // Assert
            SecretAssert.NotEqual(masterKey1, masterKey2);
            SecretAssert.LengthIs(512 /* bits */, masterKey1);
            SecretAssert.LengthIs(512 /* bits */, masterKey2);
        }

        [Fact]
        public void CreateNewDescriptor_PropagatesOptions()
        {
            // Arrange
            var configuration = new CngCbcAuthenticatedEncryptorConfiguration();

            // Act
            var descriptor = (CngCbcAuthenticatedEncryptorDescriptor)configuration.CreateNewDescriptor();

            // Assert
            Assert.Equal(configuration, descriptor.Configuration);
        }
    }
}
