// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Test.Shared;
using Microsoft.AspNetCore.Testing;
using Microsoft.AspNetCore.Testing.xunit;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Microsoft.AspNetCore.DataProtection.XmlEncryption
{
    public class DpapiXmlEncryptionTests
    {
        [ConditionalTheory]
        [ConditionalRunTestOnlyOnWindows]
        [InlineData(true)]
        [InlineData(false)]
        public void Encrypt_CurrentUserOrLocalMachine_Decrypt_RoundTrips(bool protectToLocalMachine)
        {
            // Arrange
            var originalXml = XElement.Parse(@"<mySecret value='265ee4ea-ade2-43b1-b706-09b259e58b6b' />");
            var encryptor = new DpapiXmlEncryptor(protectToLocalMachine, NullLoggerFactory.Instance);
            var decryptor = new DpapiXmlDecryptor();

            // Act & assert - run through encryptor and make sure we get back an obfuscated element
            var encryptedXmlInfo = encryptor.Encrypt(originalXml);
            Assert.Equal(typeof(DpapiXmlDecryptor), encryptedXmlInfo.DecryptorType);
            Assert.DoesNotContain("265ee4ea-ade2-43b1-b706-09b259e58b6b", encryptedXmlInfo.EncryptedElement.ToString(), StringComparison.OrdinalIgnoreCase);

            // Act & assert - run through decryptor and make sure we get back the original value
            var roundTrippedElement = decryptor.Decrypt(encryptedXmlInfo.EncryptedElement);
            XmlAssert.Equal(originalXml, roundTrippedElement);
        }
    }
}
