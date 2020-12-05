// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
// <OWNER>Microsoft</OWNER>
// 

//
// Utils.cs
// 
// 04/01/2002
// 

namespace System.Security.Cryptography.Xml
{
    using Microsoft.Win32;
    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Xml;

    [Serializable]
    internal enum CertUsageType {
        Verification = 0,
        Decryption   = 1
    }

    internal class MyXmlDocument : XmlDocument {
        protected override XmlAttribute CreateDefaultAttribute(string prefix, string localName, string namespaceURI) {
            return this.CreateAttribute(prefix, localName, namespaceURI);
        }
    }

    internal class Utils {
        private Utils () {}

        private static bool HasNamespace (XmlElement element, string prefix, string value) {
            if (IsCommittedNamespace(element, prefix, value)) return true;
            if (element.Prefix == prefix && element.NamespaceURI == value) return true;
            return false;
        }

        // A helper function that determines if a namespace node is a committed attribute
        internal static bool IsCommittedNamespace (XmlElement element, string prefix, string value) {
            if (element == null)
                throw new ArgumentNullException("element");

            string name = ((prefix.Length > 0) ? "xmlns:" + prefix : "xmlns");
            if (element.HasAttribute(name) && element.GetAttribute(name) == value) return true;
            return false;
        }

        internal static bool IsRedundantNamespace (XmlElement element, string prefix, string value) {
            if (element == null)
                throw new ArgumentNullException("element");

            XmlNode ancestorNode = ((XmlNode)element).ParentNode;
            while (ancestorNode != null) {
                XmlElement ancestorElement = ancestorNode as XmlElement;
                if (ancestorElement != null) 
                    if (HasNamespace(ancestorElement, prefix, value)) return true;
                ancestorNode = ancestorNode.ParentNode;
            }

            return false;
        }

        internal static string GetAttribute (XmlElement element, string localName, string namespaceURI) {
            string s = (element.HasAttribute(localName) ? element.GetAttribute(localName) : null);
            if (s == null && element.HasAttribute(localName, namespaceURI))
                s = element.GetAttribute(localName, namespaceURI);
            return s;
        }

        internal static bool HasAttribute (XmlElement element, string localName, string namespaceURI) {
            return element.HasAttribute(localName) || element.HasAttribute(localName, namespaceURI);
        }

        internal static bool IsNamespaceNode (XmlNode n) {
            return n.NodeType == XmlNodeType.Attribute && (n.Prefix.Equals("xmlns") || (n.Prefix.Length == 0 && n.LocalName.Equals("xmlns")));
        }

        internal static bool IsXmlNamespaceNode (XmlNode n) {
            return n.NodeType == XmlNodeType.Attribute && n.Prefix.Equals("xml");
        }

        // We consider xml:space style attributes as default namespace nodes since they obey the same propagation rules
        internal static bool IsDefaultNamespaceNode (XmlNode n) {
            bool b1 = n.NodeType == XmlNodeType.Attribute && n.Prefix.Length == 0 && n.LocalName.Equals("xmlns");
            bool b2 = IsXmlNamespaceNode(n);
            return b1 || b2;
        }

        internal static bool IsEmptyDefaultNamespaceNode (XmlNode n) {
            return IsDefaultNamespaceNode(n) && n.Value.Length == 0;
        }

        internal static string GetNamespacePrefix (XmlAttribute a) {
            Debug.Assert(IsNamespaceNode(a) || IsXmlNamespaceNode(a));
            return a.Prefix.Length == 0 ? string.Empty : a.LocalName;
        }

        internal static bool HasNamespacePrefix (XmlAttribute a, string nsPrefix) {
            return GetNamespacePrefix(a).Equals(nsPrefix);
        }

        internal static bool IsNonRedundantNamespaceDecl (XmlAttribute a, XmlAttribute nearestAncestorWithSamePrefix) {
            if (nearestAncestorWithSamePrefix == null)
                return !IsEmptyDefaultNamespaceNode(a);
            else
                return !nearestAncestorWithSamePrefix.Value.Equals(a.Value);
        }

        internal static bool IsXmlPrefixDefinitionNode (XmlAttribute a) {
            return false;
//            return a.Prefix.Equals("xmlns") && a.LocalName.Equals("xml") && a.Value.Equals(NamespaceUrlForXmlPrefix);
        }

        internal static String DiscardWhiteSpaces (string inputBuffer) {
            return DiscardWhiteSpaces(inputBuffer, 0, inputBuffer.Length);
        }


        internal static String DiscardWhiteSpaces (string inputBuffer, int inputOffset, int inputCount) {
            int i, iCount = 0;
            for (i=0; i<inputCount; i++)
                if (Char.IsWhiteSpace(inputBuffer[inputOffset + i])) iCount++;
            char[] rgbOut = new char[inputCount - iCount];
            iCount = 0;
            for (i=0; i<inputCount; i++)
                if (!Char.IsWhiteSpace(inputBuffer[inputOffset + i])) {
                    rgbOut[iCount++] = inputBuffer[inputOffset + i];
                }
            return new String(rgbOut);
        }

        internal static void SBReplaceCharWithString (StringBuilder sb, char oldChar, string newString) {
            int i = 0;
            int newStringLength = newString.Length;
            while (i < sb.Length) {
                if (sb[i] == oldChar) {
                    sb.Remove(i,1);
                    sb.Insert(i,newString);
                    i += newStringLength;
                } else i++;
            }
        }

        internal static XmlReader PreProcessStreamInput (Stream inputStream, XmlResolver xmlResolver, string baseUri) {
            XmlReaderSettings settings = GetSecureXmlReaderSettings(xmlResolver);
            XmlReader reader = XmlReader.Create(inputStream, settings, baseUri);
            return reader;
        }

        [SuppressMessage("Microsoft.Security.Xml", "CA3069:ReviewDtdProcessingAssignment", Justification= "DTD risks are mitigated by URI restrictions and expansion limits")]
        internal static XmlReaderSettings GetSecureXmlReaderSettings(XmlResolver xmlResolver)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = xmlResolver;
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.MaxCharactersFromEntities = GetMaxCharactersFromEntities();
            settings.MaxCharactersInDocument = GetMaxCharactersInDocument();
            return settings;
        }

        private static int? xmlDsigSearchDepth = null;
        /// <summary>
        /// Function get the XML Dsig recursion limit. This function defines the
        /// default limit in case, limit is not defined by developer or admin then
        /// it returns the default value.
        /// </summary>
        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static int GetXmlDsigSearchDepth() {
            if (xmlDsigSearchDepth.HasValue) {
                return xmlDsigSearchDepth.Value;
            }
            //Keeping the default recursion limit to 20. It should be
            //within limits of real world scenarios. Keeping this number low
            //will preserve some stack space
            long maxXmlDsigSearchDepth = GetNetFxSecurityRegistryValue("SignedDigitalSignatureXmlMaxDepth", 20);

            xmlDsigSearchDepth = (int)maxXmlDsigSearchDepth;
            return xmlDsigSearchDepth.Value;
        }

        private static long? maxCharactersFromEntities = null;
        // Allow machine admins to specify an entity expansion limit. This is used to prevent
        // entity expansion denial of service attacks.
        // Falls back to a default if none is specified.
        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static long GetMaxCharactersFromEntities() {
            if (maxCharactersFromEntities.HasValue) {
                return maxCharactersFromEntities.Value;
            }

            long maxCharacters = GetNetFxSecurityRegistryValue("SignedXmlMaxCharactersFromEntities", (long)1e7);

            maxCharactersFromEntities = maxCharacters;
            return maxCharactersFromEntities.Value;
        }

        private static bool s_readMaxCharactersInDocument = false;
        private static long s_maxCharactersInDocument = 0;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static long GetMaxCharactersInDocument()
        {
            // Allow machine administrators to specify a maximum document load size for SignedXml.
            if (s_readMaxCharactersInDocument)
            {
                return s_maxCharactersInDocument;
            }

            // The default value, 0, is "no limit"
            long maxCharacters = GetNetFxSecurityRegistryValue("SignedXmlMaxCharactersInDocument", 0);

            s_maxCharactersInDocument = maxCharacters;
            Thread.MemoryBarrier();
            s_readMaxCharactersInDocument = true;

            return s_maxCharactersInDocument;
        }

        private static bool? s_allowAmbiguousReferenceTarget = null;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static bool AllowAmbiguousReferenceTargets()
        {
            // Allow machine administrators to specify that the legacy behavior of matching the first element
            // in an ambiguous reference situation should be persisted. The default behavior is to throw in that
            // situation, but a REG_DWORD or REG_QWORD value of 1 will revert.
            if (s_allowAmbiguousReferenceTarget.HasValue)
            {
                return s_allowAmbiguousReferenceTarget.Value;
            }

            long numericValue = GetNetFxSecurityRegistryValue("SignedXmlAllowAmbiguousReferenceTargets", 0);
            bool allowAmbiguousReferenceTarget = numericValue != 0;

            s_allowAmbiguousReferenceTarget = allowAmbiguousReferenceTarget;
            return s_allowAmbiguousReferenceTarget.Value;
        }

        private static bool? s_allowDetachedSignature = null;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static bool AllowDetachedSignature()
        {
            // Allow machine administrators to specify that detached signatures can be processed.
            // The default behavior is to throw when processing a detached signature,
            // but a REG_DWORD or REG_QWORD value of 1 will revert.
            if (s_allowDetachedSignature.HasValue)
            {
                return s_allowDetachedSignature.Value;
            }

            long numericValue = GetNetFxSecurityRegistryValue("SignedXmlAllowDetachedSignature", 0);
            bool allowDetachedSignature = numericValue != 0;

            s_allowDetachedSignature = allowDetachedSignature;
            return s_allowDetachedSignature.Value;
        }

        private static bool s_readRequireNCNameIdentifier = false;
        private static bool s_requireNCNameIdentifier = true;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static bool RequireNCNameIdentifier()
        {
            if (s_readRequireNCNameIdentifier)
            {
                return s_requireNCNameIdentifier;
            }

            long numericValue = GetNetFxSecurityRegistryValue("SignedXmlRequireNCNameIdentifier", 1);
            bool requireNCName = numericValue != 0;

            s_requireNCNameIdentifier = requireNCName;
            Thread.MemoryBarrier();
            s_readRequireNCNameIdentifier = true;

            return s_requireNCNameIdentifier;
        }

        private static bool s_readMaxTransformsPerReference = false;
        private static long s_maxTransformsPerReference = 10;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static long GetMaxTransformsPerReference()
        {
            // Allow machine administrators to specify a maximum number of Transforms per Reference in SignedXML.
            if (s_readMaxTransformsPerReference)
            {
                return s_maxTransformsPerReference;
            }

            long maxTransforms = GetNetFxSecurityRegistryValue("SignedXmlMaxTransformsPerReference", 10);

            s_maxTransformsPerReference = maxTransforms;
            Thread.MemoryBarrier();
            s_readMaxTransformsPerReference = true;

            return s_maxTransformsPerReference;
        }

        private static bool s_readMaxReferencesPerSignedInfo = false;
        private static long s_maxReferencesPerSignedInfo = 100;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static long GetMaxReferencesPerSignedInfo()
        {
            // Allow machine administrators to specify a maximum number of References per SignedInfo/Signature in SignedXML.
            if (s_readMaxReferencesPerSignedInfo)
            {
                return s_maxReferencesPerSignedInfo;
            }

            long maxReferences = GetNetFxSecurityRegistryValue("SignedXmlMaxReferencesPerSignedInfo", 100);

            s_maxReferencesPerSignedInfo = maxReferences;
            Thread.MemoryBarrier();
            s_readMaxReferencesPerSignedInfo = true;

            return s_maxReferencesPerSignedInfo;
        }

        private static bool s_readAllowAdditionalSignatureNodes = false;
        private static bool s_allowAdditionalSignatureNodes = false;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static bool GetAllowAdditionalSignatureNodes()
        {
            // Allow machine administrators to specify whether SignedXML should allow Signature nodes that don't conform to the spec.
            if (s_readAllowAdditionalSignatureNodes)
            {
                return s_allowAdditionalSignatureNodes;
            }

            long numericValue = GetNetFxSecurityRegistryValue("SignedXmlAllowAdditionalSignatureNodes", 0);
            bool allowAdditionalSignatureNodes = numericValue != 0;

            s_allowAdditionalSignatureNodes = allowAdditionalSignatureNodes;
            Thread.MemoryBarrier();
            s_readAllowAdditionalSignatureNodes = true;

            return s_allowAdditionalSignatureNodes;
        }

        private static bool s_readSkipSignatureAttributeEnforcement = false;
        private static bool s_skipSignatureAttributeEnforcement = false;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static bool GetSkipSignatureAttributeEnforcement()
        {
            // Allow machine administrators to specify whether SignedXML should skip enforcement of
            // spec Attribute rules in Signature nodes.
            if (s_readSkipSignatureAttributeEnforcement)
            {
                return s_skipSignatureAttributeEnforcement;
            }

            long numericValue = GetNetFxSecurityRegistryValue("SignedXmlSkipSignatureAttributeEnforcement", 0);
            bool skipSignatureAttributeEnforcement = numericValue != 0;

            s_skipSignatureAttributeEnforcement = skipSignatureAttributeEnforcement;
            Thread.MemoryBarrier();
            s_readSkipSignatureAttributeEnforcement = true;

            return s_skipSignatureAttributeEnforcement;
        }

        private static bool s_readAllowBareTypeReference = false;
        private static bool s_allowBareTypeReference = false;

        internal static bool VerifyAttributes(XmlElement element, string expectedAttrName)
        {
            return VerifyAttributes(element, expectedAttrName == null ? null : new string[] { expectedAttrName });
        }

        internal static bool VerifyAttributes(XmlElement element, string[] expectedAttrNames)
        {
            if (!GetSkipSignatureAttributeEnforcement())
            {
                foreach (XmlAttribute attr in element.Attributes)
                {
                    // There are a few Xml Special Attributes that are always allowed on any node. Make sure we allow those here.
                    bool attrIsAllowed = attr.Name == "xmlns" || attr.Name.StartsWith("xmlns:") || attr.Name == "xml:space" || attr.Name == "xml:lang" || attr.Name == "xml:base";
                    int expectedInd = 0;
                    while (!attrIsAllowed && expectedAttrNames != null && expectedInd < expectedAttrNames.Length)
                    {
                        attrIsAllowed = attr.Name == expectedAttrNames[expectedInd];
                        expectedInd++;
                    }
                    if (!attrIsAllowed)
                        return false;
                }
            }
            return true;
        }

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static bool GetAllowBareTypeReference()
        {
            if (s_readAllowBareTypeReference)
            {
                return s_allowBareTypeReference;
            }

            long numericValue = GetNetFxSecurityRegistryValue("CryptoXmlAllowBareTypeReference", 0);
            bool allowBareReference = numericValue != 0;

            s_allowBareTypeReference = allowBareReference;
            Thread.MemoryBarrier();
            s_readAllowBareTypeReference = true;

            return s_allowBareTypeReference;
        }

        private static bool s_readLeaveCipherValueUnchecked = false;
        private static bool s_leaveCipherValueUnchecked = false;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static bool GetLeaveCipherValueUnchecked()
        {
            if (s_readLeaveCipherValueUnchecked)
            {
                return s_leaveCipherValueUnchecked;
            }

            long numericValue = GetNetFxSecurityRegistryValue("EncryptedXmlLeaveCipherValueUnchecked", 0);
            bool leaveCipherValueUnchecked = numericValue != 0;

            s_leaveCipherValueUnchecked = leaveCipherValueUnchecked;
            Thread.MemoryBarrier();
            s_readLeaveCipherValueUnchecked = true;

            return s_leaveCipherValueUnchecked;
        }

        private static readonly char[] s_invalidChars = new char[] { ',', '`', '[', '*', '&' };

        internal static T CreateFromName<T>(string key) where T : class
        {
            if (GetAllowBareTypeReference())
            {
                return CryptoConfig.CreateFromName(key) as T;
            }

            if (key == null || key.IndexOfAny(s_invalidChars) >= 0)
            {
                return null;
            }

            try
            {
                return CryptoConfig.CreateFromName(key) as T;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static long GetNetFxSecurityRegistryValue(string regValueName, long defaultValue)
        {
            try {
                using (RegistryKey securityRegKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework\Security", false)) {
                    if (securityRegKey != null) {
                        object regValue = securityRegKey.GetValue(regValueName);
                        if (regValue != null) {
                            RegistryValueKind valueKind = securityRegKey.GetValueKind(regValueName);
                            if (valueKind == RegistryValueKind.DWord || valueKind == RegistryValueKind.QWord) {
                                return Convert.ToInt64(regValue, CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }
            }
            catch (SecurityException) { /* we could not open the key - that's fine, we can proceed with the default value */ }

            return defaultValue;
        }

        [SuppressMessage("Microsoft.Security.Xml", "CA3069:ReviewDtdProcessingAssignment", Justification= "Required for re-parsing documents which were user-loaded with DtdProcessing.Parse")]
        internal static XmlDocument PreProcessDocumentInput (XmlDocument document, XmlResolver xmlResolver, string baseUri) {
            if (document == null)
                throw new ArgumentNullException("document");

            MyXmlDocument doc = new MyXmlDocument();
            doc.PreserveWhitespace = document.PreserveWhitespace;

            // Normalize the document
            using (TextReader stringReader = new StringReader(document.OuterXml)) {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.XmlResolver = xmlResolver;
                settings.DtdProcessing = DtdProcessing.Parse;
                settings.MaxCharactersFromEntities = GetMaxCharactersFromEntities();
                settings.MaxCharactersInDocument = GetMaxCharactersInDocument();
                XmlReader reader = XmlReader.Create(stringReader, settings, baseUri);
                doc.Load(reader);
            }
            return doc;
        }

        [SuppressMessage("Microsoft.Security.Xml", "CA3069:ReviewDtdProcessingAssignment", Justification= "Required for re-parsing elements which were user-loaded with DtdProcessing.Parse")]
        internal static XmlDocument PreProcessElementInput (XmlElement elem, XmlResolver xmlResolver, string baseUri) {
            if (elem == null)
                throw new ArgumentNullException("elem");

            MyXmlDocument doc = new MyXmlDocument();
            doc.PreserveWhitespace = true;
            // Normalize the document
            using (TextReader stringReader = new StringReader(elem.OuterXml)) {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.XmlResolver = xmlResolver;
                settings.DtdProcessing = DtdProcessing.Parse;
                settings.MaxCharactersFromEntities = GetMaxCharactersFromEntities();
                settings.MaxCharactersInDocument = GetMaxCharactersInDocument();
                XmlReader reader = XmlReader.Create(stringReader, settings, baseUri);
                doc.Load(reader);
            }
            return doc;
        }

        internal static XmlDocument DiscardComments (XmlDocument document) {
            XmlNodeList nodeList = document.SelectNodes("//comment()");
            if (nodeList != null) {
                foreach (XmlNode node1 in nodeList) {
                    node1.ParentNode.RemoveChild(node1);
                }
            }
            return document;
        }

        internal static XmlNodeList AllDescendantNodes (XmlNode node, bool includeComments) {
            CanonicalXmlNodeList nodeList = new CanonicalXmlNodeList();
            CanonicalXmlNodeList elementList = new CanonicalXmlNodeList();
            CanonicalXmlNodeList attribList = new CanonicalXmlNodeList();
            CanonicalXmlNodeList namespaceList = new CanonicalXmlNodeList();

            int index = 0;
            elementList.Add(node);

            do {
                XmlNode rootNode = (XmlNode) elementList[index];
                // Add the children nodes
                XmlNodeList childNodes = rootNode.ChildNodes;
                if (childNodes != null) {
                    foreach (XmlNode node1 in childNodes) {
                        if (includeComments || (!(node1 is XmlComment))) {
                            elementList.Add(node1);
                        }
                    }
                }
                // Add the attribute nodes
                XmlAttributeCollection attribNodes = rootNode.Attributes;
                if (attribNodes != null) {
                    foreach (XmlNode attribNode in rootNode.Attributes) {
                        if (attribNode.LocalName == "xmlns" || attribNode.Prefix == "xmlns")
                            namespaceList.Add(attribNode);
                        else
                            attribList.Add(attribNode);
                    }
                }
                index++;
            } while (index < elementList.Count);
            foreach (XmlNode elementNode in elementList) {
                nodeList.Add(elementNode);
            }
            foreach (XmlNode attribNode in attribList) {
                nodeList.Add(attribNode);
            }
            foreach (XmlNode namespaceNode in namespaceList) {
                nodeList.Add(namespaceNode);
            }

            return nodeList;
        }

        internal static bool NodeInList (XmlNode node, XmlNodeList nodeList) {
            foreach (XmlNode nodeElem in nodeList) {
                if (nodeElem == node) return true;
            }
            return false;
        }

        internal static string GetIdFromLocalUri (string uri, out bool discardComments) {
            string idref = uri.Substring(1);
            // initialize the return value
            discardComments = true;

            // Deal with XPointer of type #xpointer(id("ID")). Other XPointer support isn't handled here and is anyway optional 
            if (idref.StartsWith("xpointer(id(", StringComparison.Ordinal)) {
                int startId = idref.IndexOf("id(", StringComparison.Ordinal);
                int endId = idref.IndexOf(")", StringComparison.Ordinal);
                if (endId < 0 || endId < startId + 3) 
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidReference"));
                idref = idref.Substring(startId + 3, endId - startId - 3);
                idref = idref.Replace("\'", "");
                idref = idref.Replace("\"", "");
                discardComments = false;
            }
            return idref;
        }

        internal static string ExtractIdFromLocalUri (string uri) {
            string idref = uri.Substring(1);

            // Deal with XPointer of type #xpointer(id("ID")). Other XPointer support isn't handled here and is anyway optional 
            if (idref.StartsWith("xpointer(id(", StringComparison.Ordinal)) {
                int startId = idref.IndexOf("id(", StringComparison.Ordinal);
                int endId = idref.IndexOf(")", StringComparison.Ordinal);
                if (endId < 0 || endId < startId + 3) 
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidReference"));
                idref = idref.Substring(startId + 3, endId - startId - 3);
                idref = idref.Replace("\'", "");
                idref = idref.Replace("\"", "");
            }
            return idref;
        }

        // This removes all children of an element.
        internal static void RemoveAllChildren (XmlElement inputElement) {
            XmlNode child = inputElement.FirstChild;
            XmlNode sibling = null;

            while (child != null) {
                sibling = child.NextSibling;
                inputElement.RemoveChild(child);
                child = sibling;
            }
        }

        // Writes one stream (starting from the current position) into 
        // an output stream, connecting them up and reading until 
        // hitting the end of the input stream.  
        // returns the number of bytes copied
        internal static long Pump (Stream input, Stream output) { 
            // Use MemoryStream's WriteTo(Stream) method if possible
            MemoryStream inputMS = input as MemoryStream;
            if (inputMS != null && inputMS.Position == 0) {
                inputMS.WriteTo(output);
                return inputMS.Length;
            }

            const int count = 4096;
            byte[] bytes = new byte[count];
            int numBytes;
            long totalBytes = 0;

            while((numBytes = input.Read(bytes, 0, count)) > 0) {
                output.Write(bytes, 0, numBytes);
                totalBytes += numBytes;
            }

            return totalBytes;
        }

        internal static Hashtable TokenizePrefixListString (string s) {
            Hashtable set = new Hashtable();
            if (s != null) {
                string[] prefixes = s.Split(null);
                foreach (string prefix in prefixes) {
                    if (prefix.Equals("#default")) {
                        set.Add(string.Empty, true);
                    } else if (prefix.Length > 0) {
                        set.Add(prefix, true);
                    }
                }
            }
            return set;
        }

        internal static string EscapeWhitespaceData(string data) {
            StringBuilder sb = new StringBuilder();
            sb.Append(data);
            Utils.SBReplaceCharWithString(sb, (char) 13,"&#xD;");
            return sb.ToString();;
        }

        internal static string EscapeTextData(string data) {
            StringBuilder sb = new StringBuilder();
            sb.Append(data);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            SBReplaceCharWithString(sb, (char) 13, "&#xD;");
            return sb.ToString();;
        }

        internal static string EscapeCData(string data) {
            return EscapeTextData(data);
        }

        internal static string EscapeAttributeValue(string value) {
            StringBuilder sb = new StringBuilder();
            sb.Append(value);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace("\"", "&quot;");
            SBReplaceCharWithString(sb, (char) 9, "&#x9;");
            SBReplaceCharWithString(sb, (char) 10, "&#xA;");
            SBReplaceCharWithString(sb, (char) 13, "&#xD;");
            return sb.ToString();
        }

        internal static XmlDocument GetOwnerDocument (XmlNodeList nodeList) {
            foreach (XmlNode node in nodeList) {
                if (node.OwnerDocument != null)
                    return node.OwnerDocument;
            }
            return null;
        }

        internal static void AddNamespaces (XmlElement elem, CanonicalXmlNodeList namespaces) {
            if (namespaces != null) {
                foreach (XmlNode attrib in namespaces) {
                    string name = ((attrib.Prefix.Length > 0) ? attrib.Prefix + ":" + attrib.LocalName : attrib.LocalName);
                    // Skip the attribute if one with the same qualified name already exists
                    if (elem.HasAttribute(name) || (name.Equals("xmlns") && elem.Prefix.Length == 0)) continue;
                    XmlAttribute nsattrib = (XmlAttribute) elem.OwnerDocument.CreateAttribute(name);
                    nsattrib.Value = attrib.Value;
                    elem.SetAttributeNode(nsattrib);
                }
            }
        }

        internal static void AddNamespaces (XmlElement elem, Hashtable namespaces) {
            if (namespaces != null) {
                foreach (string key in namespaces.Keys) {
                    if (elem.HasAttribute(key)) continue;
                    XmlAttribute nsattrib = (XmlAttribute) elem.OwnerDocument.CreateAttribute(key);
                    nsattrib.Value = namespaces[key] as string;
                    elem.SetAttributeNode(nsattrib);
                }
            }
        }

        // This method gets the attributes that should be propagated 
        internal static CanonicalXmlNodeList GetPropagatedAttributes (XmlElement elem) {
            if (elem == null)
                return null;

            CanonicalXmlNodeList namespaces = new CanonicalXmlNodeList();
            XmlNode ancestorNode = elem;

            if (ancestorNode == null) return null;

            bool bDefNamespaceToAdd = true;

            while (ancestorNode != null) {
                XmlElement ancestorElement = ancestorNode as XmlElement;
                if (ancestorElement == null) {
                    ancestorNode = ancestorNode.ParentNode;
                    continue;
                }
                if (!Utils.IsCommittedNamespace(ancestorElement, ancestorElement.Prefix, ancestorElement.NamespaceURI)) {
                    // Add the namespace attribute to the collection if needed
                    if (!Utils.IsRedundantNamespace(ancestorElement, ancestorElement.Prefix, ancestorElement.NamespaceURI)) {
                        string name = ((ancestorElement.Prefix.Length > 0) ? "xmlns:" + ancestorElement.Prefix : "xmlns");
                        XmlAttribute nsattrib = elem.OwnerDocument.CreateAttribute(name);
                        nsattrib.Value = ancestorElement.NamespaceURI;
                        namespaces.Add(nsattrib);
                    }
                }
                if (ancestorElement.HasAttributes) {
                    XmlAttributeCollection attribs = ancestorElement.Attributes;
                    foreach (XmlAttribute attrib in attribs) {
                        // Add a default namespace if necessary
                        if (bDefNamespaceToAdd && attrib.LocalName == "xmlns") {
                            XmlAttribute nsattrib = elem.OwnerDocument.CreateAttribute("xmlns");
                            nsattrib.Value = attrib.Value;
                            namespaces.Add(nsattrib);
                            bDefNamespaceToAdd = false;
                            continue;
                        }
                        // retain the declarations of type 'xml:*' as well
                        if (attrib.Prefix == "xmlns" || attrib.Prefix == "xml") {
                            namespaces.Add(attrib);
                            continue;
                        }
                        if (attrib.NamespaceURI.Length > 0) {
                            if (!Utils.IsCommittedNamespace(ancestorElement, attrib.Prefix, attrib.NamespaceURI)) {
                                // Add the namespace attribute to the collection if needed
                                if (!Utils.IsRedundantNamespace(ancestorElement, attrib.Prefix, attrib.NamespaceURI)) {
                                    string name = ((attrib.Prefix.Length > 0) ? "xmlns:" + attrib.Prefix : "xmlns");
                                    XmlAttribute nsattrib = elem.OwnerDocument.CreateAttribute(name);
                                    nsattrib.Value = attrib.NamespaceURI;
                                    namespaces.Add(nsattrib);
                                }
                            }
                        }
                    }
                }
                ancestorNode = ancestorNode.ParentNode;
            }

            return namespaces;
        }

        // output of this routine is always big endian
        internal static byte[] ConvertIntToByteArray (int dwInput) {
            byte[] rgbTemp = new byte[8]; // int can never be greater than Int64
            int t1;  // t1 is remaining value to account for
            int t2;  // t2 is t1 % 256
            int i = 0;

            if (dwInput == 0) return new byte[1]; 
            t1 = dwInput; 
            while (t1 > 0) {
                t2 = t1 % 256;
                rgbTemp[i] = (byte) t2;
                t1 = (t1 - t2)/256;
                i++;
            }
            // Now, copy only the non-zero part of rgbTemp and reverse
            byte[] rgbOutput = new byte[i];
            // copy and reverse in one pass
            for (int j = 0; j < i; j++) {
                rgbOutput[j] = rgbTemp[i-j-1];
            }
            return rgbOutput;
        }

        internal static int GetHexArraySize (byte[] hex) {
            int index = hex.Length;
            while (index-- > 0) {
                if (hex[index] != 0)
                    break;
            }
            return index + 1;
        }

        [System.Security.SecuritySafeCritical]
        internal static X509Certificate2Collection BuildBagOfCerts (KeyInfoX509Data keyInfoX509Data, CertUsageType certUsageType) {
            X509Certificate2Collection collection = new X509Certificate2Collection();
            ArrayList decryptionIssuerSerials = (certUsageType == CertUsageType.Decryption ? new ArrayList() : null);
            if (keyInfoX509Data.Certificates != null) {
                foreach (X509Certificate2 certificate in keyInfoX509Data.Certificates) {
                    switch (certUsageType) {
                    case CertUsageType.Verification:
                        collection.Add(certificate);
                        break;
                    case CertUsageType.Decryption:
                        decryptionIssuerSerials.Add(new X509IssuerSerial(certificate.IssuerName.Name, certificate.SerialNumber));
                        break;
                    }
                }
            }

            if (keyInfoX509Data.SubjectNames == null && keyInfoX509Data.IssuerSerials == null &&
                keyInfoX509Data.SubjectKeyIds == null && decryptionIssuerSerials == null)
                return collection;

            // Open LocalMachine and CurrentUser "Other People"/"My" stores.

            // Assert OpenStore since we are not giving back any certificates to the user.
            StorePermission sp = new StorePermission(StorePermissionFlags.OpenStore);
            sp.Assert();

            X509Store[] stores = new X509Store[2];
            string storeName = (certUsageType == CertUsageType.Verification ? "AddressBook" : "My");
            stores[0] = new X509Store(storeName, StoreLocation.CurrentUser);
            stores[1] = new X509Store(storeName, StoreLocation.LocalMachine);

            for (int index=0; index < stores.Length; index++) {
                if (stores[index] != null) {
                    X509Certificate2Collection filters = null;
                    // We don't care if we can't open the store.
                    try {
                        stores[index].Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                        filters = stores[index].Certificates;
                        stores[index].Close();
                        if (keyInfoX509Data.SubjectNames != null) {
                            foreach (string subjectName in keyInfoX509Data.SubjectNames) {
                                filters = filters.Find(X509FindType.FindBySubjectDistinguishedName, subjectName, false);
                            }
                        }
                        if (keyInfoX509Data.IssuerSerials != null) {
                            foreach (X509IssuerSerial issuerSerial in keyInfoX509Data.IssuerSerials) {
                                filters = filters.Find(X509FindType.FindByIssuerDistinguishedName, issuerSerial.IssuerName, false);
                                filters = filters.Find(X509FindType.FindBySerialNumber, issuerSerial.SerialNumber, false);
                            }
                        }
                        if (keyInfoX509Data.SubjectKeyIds != null) {
                            foreach (byte[] ski in keyInfoX509Data.SubjectKeyIds) {
                                string hex = X509Utils.EncodeHexString(ski);
                                filters = filters.Find(X509FindType.FindBySubjectKeyIdentifier, hex, false);
                            }
                        }
                        if (decryptionIssuerSerials != null) {
                            foreach (X509IssuerSerial issuerSerial in decryptionIssuerSerials) {
                                filters = filters.Find(X509FindType.FindByIssuerDistinguishedName, issuerSerial.IssuerName, false);
                                filters = filters.Find(X509FindType.FindBySerialNumber, issuerSerial.SerialNumber, false);
                            }
                        }
                    }
                    catch (CryptographicException) {}

                    if (filters != null) 
                        collection.AddRange(filters);
                }
            }

            return collection;
        }
    }
}