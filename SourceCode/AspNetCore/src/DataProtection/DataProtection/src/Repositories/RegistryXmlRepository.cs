// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace Microsoft.AspNetCore.DataProtection.Repositories
{
    /// <summary>
    /// An XML repository backed by the Windows registry.
    /// </summary>
    public class RegistryXmlRepository : IXmlRepository
    {
        private static readonly Lazy<RegistryKey> _defaultRegistryKeyLazy = new Lazy<RegistryKey>(GetDefaultHklmStorageKey);

        private readonly ILogger _logger;

        /// <summary>
        /// Creates a <see cref="RegistryXmlRepository"/> with keys stored in the given registry key.
        /// </summary>
        /// <param name="registryKey">The registry key in which to persist key material.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public RegistryXmlRepository(RegistryKey registryKey, ILoggerFactory loggerFactory)
        {
            if (registryKey == null)
            {
                throw new ArgumentNullException(nameof(registryKey));
            }

            RegistryKey = registryKey;
            _logger = loggerFactory.CreateLogger<RegistryXmlRepository>();
        }

        /// <summary>
        /// The default key storage directory, which currently corresponds to
        /// "HKLM\SOFTWARE\Microsoft\ASP.NET\4.0.30319.0\AutoGenKeys\{SID}".
        /// </summary>
        /// <remarks>
        /// This property can return null if no suitable default registry key can
        /// be found, such as the case when this application is not hosted inside IIS.
        /// </remarks>
        public static RegistryKey DefaultRegistryKey => _defaultRegistryKeyLazy.Value;

        /// <summary>
        /// The registry key into which key material will be written.
        /// </summary>
        public RegistryKey RegistryKey { get; }

        public virtual IReadOnlyCollection<XElement> GetAllElements()
        {
            // forces complete enumeration
            return GetAllElementsCore().ToList().AsReadOnly();
        }

        private IEnumerable<XElement> GetAllElementsCore()
        {
            // Note: Inability to parse any value is considered a fatal error (since the value may contain
            // revocation information), and we'll fail the entire operation rather than return a partial
            // set of elements. If a file contains well-formed XML but its contents are meaningless, we
            // won't fail that operation here. The caller is responsible for failing as appropriate given
            // that scenario.

            foreach (string valueName in RegistryKey.GetValueNames())
            {
                var element = ReadElementFromRegKey(RegistryKey, valueName);
                if (element != null)
                {
                    yield return element;
                }
            }
        }

        private static RegistryKey GetDefaultHklmStorageKey()
        {
            try
            {
                var registryView = IntPtr.Size == 4 ? RegistryView.Registry32 : RegistryView.Registry64;
                // Try reading the auto-generated machine key from HKLM
                using (var hklmBaseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
                {
                    // Even though this is in HKLM, WAS ensures that applications hosted in IIS are properly isolated.
                    // See APP_POOL::EnsureSharedMachineKeyStorage in WAS source for more info.
                    // The version number will need to change if IIS hosts Core CLR directly.
                    var aspnetAutoGenKeysBaseKeyName = string.Format(
                        CultureInfo.InvariantCulture,
                        @"SOFTWARE\Microsoft\ASP.NET\4.0.30319.0\AutoGenKeys\{0}",
                        WindowsIdentity.GetCurrent().User.Value);

                    var aspnetBaseKey = hklmBaseKey.OpenSubKey(aspnetAutoGenKeysBaseKeyName, writable: true);
                    if (aspnetBaseKey != null)
                    {
                        using (aspnetBaseKey)
                        {
                            // We'll create a 'DataProtection' subkey under the auto-gen keys base
                            return aspnetBaseKey.OpenSubKey("DataProtection", writable: true)
                                ?? aspnetBaseKey.CreateSubKey("DataProtection");
                        }
                    }
                    return null; // couldn't find the auto-generated machine key
                }
            }
            catch
            {
                // swallow all errors; they're not fatal
                return null;
            }
        }

        private static bool IsSafeRegistryValueName(string filename)
        {
            // Must be non-empty and contain only a-zA-Z0-9, hyphen, and underscore.
            return (!String.IsNullOrEmpty(filename) && filename.All(c =>
                c == '-'
                || c == '_'
                || ('0' <= c && c <= '9')
                || ('A' <= c && c <= 'Z')
                || ('a' <= c && c <= 'z')));
        }

        private XElement ReadElementFromRegKey(RegistryKey regKey, string valueName)
        {
            _logger.ReadingDataFromRegistryKeyValue(regKey, valueName);

            var data = regKey.GetValue(valueName) as string;
            return (!String.IsNullOrEmpty(data)) ? XElement.Parse(data) : null;
        }

        public virtual void StoreElement(XElement element, string friendlyName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!IsSafeRegistryValueName(friendlyName))
            {
                var newFriendlyName = Guid.NewGuid().ToString();
                _logger.NameIsNotSafeRegistryValueName(friendlyName, newFriendlyName);
                friendlyName = newFriendlyName;
            }

            StoreElementCore(element, friendlyName);
        }

        private void StoreElementCore(XElement element, string valueName)
        {
            // Technically calls to RegSetValue* and RegGetValue* are atomic, so we don't have to worry about
            // another thread trying to read this value while we're writing it. There's still a small risk of
            // data corruption if power is lost while the registry file is being flushed to the file system,
            // but the window for that should be small enough that we shouldn't have to worry about it.
            RegistryKey.SetValue(valueName, element.ToString(), RegistryValueKind.String);
        }
    }
}
