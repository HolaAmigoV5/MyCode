// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Net.Http.Headers
{
    public partial class CacheControlHeaderValue
    {
        public static readonly string MaxAgeString;
        public static readonly string MaxStaleString;
        public static readonly string MinFreshString;
        public static readonly string MustRevalidateString;
        public static readonly string NoCacheString;
        public static readonly string NoStoreString;
        public static readonly string NoTransformString;
        public static readonly string OnlyIfCachedString;
        public static readonly string PrivateString;
        public static readonly string ProxyRevalidateString;
        public static readonly string PublicString;
        public static readonly string SharedMaxAgeString;
        public CacheControlHeaderValue() { }
        public System.Collections.Generic.IList<Microsoft.Net.Http.Headers.NameValueHeaderValue> Extensions { get { throw null; } }
        public System.TimeSpan? MaxAge { get { throw null; } set { } }
        public bool MaxStale { get { throw null; } set { } }
        public System.TimeSpan? MaxStaleLimit { get { throw null; } set { } }
        public System.TimeSpan? MinFresh { get { throw null; } set { } }
        public bool MustRevalidate { get { throw null; } set { } }
        public bool NoCache { get { throw null; } set { } }
        public System.Collections.Generic.ICollection<Microsoft.Extensions.Primitives.StringSegment> NoCacheHeaders { get { throw null; } }
        public bool NoStore { get { throw null; } set { } }
        public bool NoTransform { get { throw null; } set { } }
        public bool OnlyIfCached { get { throw null; } set { } }
        public bool Private { get { throw null; } set { } }
        public System.Collections.Generic.ICollection<Microsoft.Extensions.Primitives.StringSegment> PrivateHeaders { get { throw null; } }
        public bool ProxyRevalidate { get { throw null; } set { } }
        public bool Public { get { throw null; } set { } }
        public System.TimeSpan? SharedMaxAge { get { throw null; } set { } }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static Microsoft.Net.Http.Headers.CacheControlHeaderValue Parse(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(Microsoft.Extensions.Primitives.StringSegment input, out Microsoft.Net.Http.Headers.CacheControlHeaderValue parsedValue) { throw null; }
    }
    public partial class ContentDispositionHeaderValue
    {
        public ContentDispositionHeaderValue(Microsoft.Extensions.Primitives.StringSegment dispositionType) { }
        public System.DateTimeOffset? CreationDate { get { throw null; } set { } }
        public Microsoft.Extensions.Primitives.StringSegment DispositionType { get { throw null; } set { } }
        public Microsoft.Extensions.Primitives.StringSegment FileName { get { throw null; } set { } }
        public Microsoft.Extensions.Primitives.StringSegment FileNameStar { get { throw null; } set { } }
        public System.DateTimeOffset? ModificationDate { get { throw null; } set { } }
        public Microsoft.Extensions.Primitives.StringSegment Name { get { throw null; } set { } }
        public System.Collections.Generic.IList<Microsoft.Net.Http.Headers.NameValueHeaderValue> Parameters { get { throw null; } }
        public System.DateTimeOffset? ReadDate { get { throw null; } set { } }
        public long? Size { get { throw null; } set { } }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static Microsoft.Net.Http.Headers.ContentDispositionHeaderValue Parse(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public void SetHttpFileName(Microsoft.Extensions.Primitives.StringSegment fileName) { }
        public void SetMimeFileName(Microsoft.Extensions.Primitives.StringSegment fileName) { }
        public override string ToString() { throw null; }
        public static bool TryParse(Microsoft.Extensions.Primitives.StringSegment input, out Microsoft.Net.Http.Headers.ContentDispositionHeaderValue parsedValue) { throw null; }
    }
    public static partial class ContentDispositionHeaderValueIdentityExtensions
    {
        public static bool IsFileDisposition(this Microsoft.Net.Http.Headers.ContentDispositionHeaderValue header) { throw null; }
        public static bool IsFormDisposition(this Microsoft.Net.Http.Headers.ContentDispositionHeaderValue header) { throw null; }
    }
    public partial class ContentRangeHeaderValue
    {
        public ContentRangeHeaderValue(long length) { }
        public ContentRangeHeaderValue(long from, long to) { }
        public ContentRangeHeaderValue(long from, long to, long length) { }
        public long? From { get { throw null; } }
        public bool HasLength { get { throw null; } }
        public bool HasRange { get { throw null; } }
        public long? Length { get { throw null; } }
        public long? To { get { throw null; } }
        public Microsoft.Extensions.Primitives.StringSegment Unit { get { throw null; } set { } }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static Microsoft.Net.Http.Headers.ContentRangeHeaderValue Parse(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(Microsoft.Extensions.Primitives.StringSegment input, out Microsoft.Net.Http.Headers.ContentRangeHeaderValue parsedValue) { throw null; }
    }
    public partial class CookieHeaderValue
    {
        public CookieHeaderValue(Microsoft.Extensions.Primitives.StringSegment name) { }
        public CookieHeaderValue(Microsoft.Extensions.Primitives.StringSegment name, Microsoft.Extensions.Primitives.StringSegment value) { }
        public Microsoft.Extensions.Primitives.StringSegment Name { get { throw null; } set { } }
        public Microsoft.Extensions.Primitives.StringSegment Value { get { throw null; } set { } }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static Microsoft.Net.Http.Headers.CookieHeaderValue Parse(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public static System.Collections.Generic.IList<Microsoft.Net.Http.Headers.CookieHeaderValue> ParseList(System.Collections.Generic.IList<string> inputs) { throw null; }
        public static System.Collections.Generic.IList<Microsoft.Net.Http.Headers.CookieHeaderValue> ParseStrictList(System.Collections.Generic.IList<string> inputs) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(Microsoft.Extensions.Primitives.StringSegment input, out Microsoft.Net.Http.Headers.CookieHeaderValue parsedValue) { throw null; }
        public static bool TryParseList(System.Collections.Generic.IList<string> inputs, out System.Collections.Generic.IList<Microsoft.Net.Http.Headers.CookieHeaderValue> parsedValues) { throw null; }
        public static bool TryParseStrictList(System.Collections.Generic.IList<string> inputs, out System.Collections.Generic.IList<Microsoft.Net.Http.Headers.CookieHeaderValue> parsedValues) { throw null; }
    }
    public partial class EntityTagHeaderValue
    {
        public EntityTagHeaderValue(Microsoft.Extensions.Primitives.StringSegment tag) { }
        public EntityTagHeaderValue(Microsoft.Extensions.Primitives.StringSegment tag, bool isWeak) { }
        public static Microsoft.Net.Http.Headers.EntityTagHeaderValue Any { get { throw null; } }
        public bool IsWeak { get { throw null; } }
        public Microsoft.Extensions.Primitives.StringSegment Tag { get { throw null; } }
        public bool Compare(Microsoft.Net.Http.Headers.EntityTagHeaderValue other, bool useStrongComparison) { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static Microsoft.Net.Http.Headers.EntityTagHeaderValue Parse(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public static System.Collections.Generic.IList<Microsoft.Net.Http.Headers.EntityTagHeaderValue> ParseList(System.Collections.Generic.IList<string> inputs) { throw null; }
        public static System.Collections.Generic.IList<Microsoft.Net.Http.Headers.EntityTagHeaderValue> ParseStrictList(System.Collections.Generic.IList<string> inputs) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(Microsoft.Extensions.Primitives.StringSegment input, out Microsoft.Net.Http.Headers.EntityTagHeaderValue parsedValue) { throw null; }
        public static bool TryParseList(System.Collections.Generic.IList<string> inputs, out System.Collections.Generic.IList<Microsoft.Net.Http.Headers.EntityTagHeaderValue> parsedValues) { throw null; }
        public static bool TryParseStrictList(System.Collections.Generic.IList<string> inputs, out System.Collections.Generic.IList<Microsoft.Net.Http.Headers.EntityTagHeaderValue> parsedValues) { throw null; }
    }
    public static partial class HeaderNames
    {
        public const string Accept = "Accept";
        public const string AcceptCharset = "Accept-Charset";
        public const string AcceptEncoding = "Accept-Encoding";
        public const string AcceptLanguage = "Accept-Language";
        public const string AcceptRanges = "Accept-Ranges";
        public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";
        public const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
        public const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        public const string AccessControlExposeHeaders = "Access-Control-Expose-Headers";
        public const string AccessControlMaxAge = "Access-Control-Max-Age";
        public const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        public const string AccessControlRequestMethod = "Access-Control-Request-Method";
        public const string Age = "Age";
        public const string Allow = "Allow";
        public const string Authority = ":authority";
        public const string Authorization = "Authorization";
        public const string CacheControl = "Cache-Control";
        public const string Connection = "Connection";
        public const string ContentDisposition = "Content-Disposition";
        public const string ContentEncoding = "Content-Encoding";
        public const string ContentLanguage = "Content-Language";
        public const string ContentLength = "Content-Length";
        public const string ContentLocation = "Content-Location";
        public const string ContentMD5 = "Content-MD5";
        public const string ContentRange = "Content-Range";
        public const string ContentSecurityPolicy = "Content-Security-Policy";
        public const string ContentSecurityPolicyReportOnly = "Content-Security-Policy-Report-Only";
        public const string ContentType = "Content-Type";
        public const string Cookie = "Cookie";
        public const string Date = "Date";
        public const string ETag = "ETag";
        public const string Expect = "Expect";
        public const string Expires = "Expires";
        public const string From = "From";
        public const string Host = "Host";
        public const string IfMatch = "If-Match";
        public const string IfModifiedSince = "If-Modified-Since";
        public const string IfNoneMatch = "If-None-Match";
        public const string IfRange = "If-Range";
        public const string IfUnmodifiedSince = "If-Unmodified-Since";
        public const string LastModified = "Last-Modified";
        public const string Location = "Location";
        public const string MaxForwards = "Max-Forwards";
        public const string Method = ":method";
        public const string Origin = "Origin";
        public const string Path = ":path";
        public const string Pragma = "Pragma";
        public const string ProxyAuthenticate = "Proxy-Authenticate";
        public const string ProxyAuthorization = "Proxy-Authorization";
        public const string Range = "Range";
        public const string Referer = "Referer";
        public const string RetryAfter = "Retry-After";
        public const string Scheme = ":scheme";
        public const string Server = "Server";
        public const string SetCookie = "Set-Cookie";
        public const string Status = ":status";
        public const string StrictTransportSecurity = "Strict-Transport-Security";
        public const string TE = "TE";
        public const string Trailer = "Trailer";
        public const string TransferEncoding = "Transfer-Encoding";
        public const string Upgrade = "Upgrade";
        public const string UserAgent = "User-Agent";
        public const string Vary = "Vary";
        public const string Via = "Via";
        public const string Warning = "Warning";
        public const string WebSocketSubProtocols = "Sec-WebSocket-Protocol";
        public const string WWWAuthenticate = "WWW-Authenticate";
    }
    public static partial class HeaderQuality
    {
        public const double Match = 1;
        public const double NoMatch = 0;
    }
    public static partial class HeaderUtilities
    {
        public static bool ContainsCacheDirective(Microsoft.Extensions.Primitives.StringValues cacheControlDirectives, string targetDirectives) { throw null; }
        public static Microsoft.Extensions.Primitives.StringSegment EscapeAsQuotedString(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public static string FormatDate(System.DateTimeOffset dateTime) { throw null; }
        public static string FormatDate(System.DateTimeOffset dateTime, bool quoted) { throw null; }
        public static string FormatNonNegativeInt64(long value) { throw null; }
        public static bool IsQuoted(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public static Microsoft.Extensions.Primitives.StringSegment RemoveQuotes(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public static bool TryParseDate(Microsoft.Extensions.Primitives.StringSegment input, out System.DateTimeOffset result) { throw null; }
        public static bool TryParseNonNegativeInt32(Microsoft.Extensions.Primitives.StringSegment value, out int result) { throw null; }
        public static bool TryParseNonNegativeInt64(Microsoft.Extensions.Primitives.StringSegment value, out long result) { throw null; }
        public static bool TryParseSeconds(Microsoft.Extensions.Primitives.StringValues headerValues, string targetValue, out System.TimeSpan? value) { throw null; }
        public static Microsoft.Extensions.Primitives.StringSegment UnescapeAsQuotedString(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
    }
    public partial class MediaTypeHeaderValue
    {
        public MediaTypeHeaderValue(Microsoft.Extensions.Primitives.StringSegment mediaType) { }
        public MediaTypeHeaderValue(Microsoft.Extensions.Primitives.StringSegment mediaType, double quality) { }
        public Microsoft.Extensions.Primitives.StringSegment Boundary { get { throw null; } set { } }
        public Microsoft.Extensions.Primitives.StringSegment Charset { get { throw null; } set { } }
        public System.Text.Encoding Encoding { get { throw null; } set { } }
        public System.Collections.Generic.IEnumerable<Microsoft.Extensions.Primitives.StringSegment> Facets { get { throw null; } }
        public bool IsReadOnly { get { throw null; } }
        public bool MatchesAllSubTypes { get { throw null; } }
        public bool MatchesAllSubTypesWithoutSuffix { get { throw null; } }
        public bool MatchesAllTypes { get { throw null; } }
        public Microsoft.Extensions.Primitives.StringSegment MediaType { get { throw null; } set { } }
        public System.Collections.Generic.IList<Microsoft.Net.Http.Headers.NameValueHeaderValue> Parameters { get { throw null; } }
        public double? Quality { get { throw null; } set { } }
        public Microsoft.Extensions.Primitives.StringSegment SubType { get { throw null; } }
        public Microsoft.Extensions.Primitives.StringSegment SubTypeWithoutSuffix { get { throw null; } }
        public Microsoft.Extensions.Primitives.StringSegment Suffix { get { throw null; } }
        public Microsoft.Extensions.Primitives.StringSegment Type { get { throw null; } }
        public Microsoft.Net.Http.Headers.MediaTypeHeaderValue Copy() { throw null; }
        public Microsoft.Net.Http.Headers.MediaTypeHeaderValue CopyAsReadOnly() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public bool IsSubsetOf(Microsoft.Net.Http.Headers.MediaTypeHeaderValue otherMediaType) { throw null; }
        public static Microsoft.Net.Http.Headers.MediaTypeHeaderValue Parse(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public static System.Collections.Generic.IList<Microsoft.Net.Http.Headers.MediaTypeHeaderValue> ParseList(System.Collections.Generic.IList<string> inputs) { throw null; }
        public static System.Collections.Generic.IList<Microsoft.Net.Http.Headers.MediaTypeHeaderValue> ParseStrictList(System.Collections.Generic.IList<string> inputs) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(Microsoft.Extensions.Primitives.StringSegment input, out Microsoft.Net.Http.Headers.MediaTypeHeaderValue parsedValue) { throw null; }
        public static bool TryParseList(System.Collections.Generic.IList<string> inputs, out System.Collections.Generic.IList<Microsoft.Net.Http.Headers.MediaTypeHeaderValue> parsedValues) { throw null; }
        public static bool TryParseStrictList(System.Collections.Generic.IList<string> inputs, out System.Collections.Generic.IList<Microsoft.Net.Http.Headers.MediaTypeHeaderValue> parsedValues) { throw null; }
    }
    public partial class MediaTypeHeaderValueComparer : System.Collections.Generic.IComparer<Microsoft.Net.Http.Headers.MediaTypeHeaderValue>
    {
        internal MediaTypeHeaderValueComparer() { }
        public static Microsoft.Net.Http.Headers.MediaTypeHeaderValueComparer QualityComparer { get { throw null; } }
        public int Compare(Microsoft.Net.Http.Headers.MediaTypeHeaderValue mediaType1, Microsoft.Net.Http.Headers.MediaTypeHeaderValue mediaType2) { throw null; }
    }
    public partial class NameValueHeaderValue
    {
        public NameValueHeaderValue(Microsoft.Extensions.Primitives.StringSegment name) { }
        public NameValueHeaderValue(Microsoft.Extensions.Primitives.StringSegment name, Microsoft.Extensions.Primitives.StringSegment value) { }
        public bool IsReadOnly { get { throw null; } }
        public Microsoft.Extensions.Primitives.StringSegment Name { get { throw null; } }
        public Microsoft.Extensions.Primitives.StringSegment Value { get { throw null; } set { } }
        public Microsoft.Net.Http.Headers.NameValueHeaderValue Copy() { throw null; }
        public Microsoft.Net.Http.Headers.NameValueHeaderValue CopyAsReadOnly() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public static Microsoft.Net.Http.Headers.NameValueHeaderValue Find(System.Collections.Generic.IList<Microsoft.Net.Http.Headers.NameValueHeaderValue> values, Microsoft.Extensions.Primitives.StringSegment name) { throw null; }
        public override int GetHashCode() { throw null; }
        public Microsoft.Extensions.Primitives.StringSegment GetUnescapedValue() { throw null; }
        public static Microsoft.Net.Http.Headers.NameValueHeaderValue Parse(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public static System.Collections.Generic.IList<Microsoft.Net.Http.Headers.NameValueHeaderValue> ParseList(System.Collections.Generic.IList<string> input) { throw null; }
        public static System.Collections.Generic.IList<Microsoft.Net.Http.Headers.NameValueHeaderValue> ParseStrictList(System.Collections.Generic.IList<string> input) { throw null; }
        public void SetAndEscapeValue(Microsoft.Extensions.Primitives.StringSegment value) { }
        public override string ToString() { throw null; }
        public static bool TryParse(Microsoft.Extensions.Primitives.StringSegment input, out Microsoft.Net.Http.Headers.NameValueHeaderValue parsedValue) { throw null; }
        public static bool TryParseList(System.Collections.Generic.IList<string> input, out System.Collections.Generic.IList<Microsoft.Net.Http.Headers.NameValueHeaderValue> parsedValues) { throw null; }
        public static bool TryParseStrictList(System.Collections.Generic.IList<string> input, out System.Collections.Generic.IList<Microsoft.Net.Http.Headers.NameValueHeaderValue> parsedValues) { throw null; }
    }
    public partial class RangeConditionHeaderValue
    {
        public RangeConditionHeaderValue(Microsoft.Net.Http.Headers.EntityTagHeaderValue entityTag) { }
        public RangeConditionHeaderValue(System.DateTimeOffset lastModified) { }
        public RangeConditionHeaderValue(string entityTag) { }
        public Microsoft.Net.Http.Headers.EntityTagHeaderValue EntityTag { get { throw null; } }
        public System.DateTimeOffset? LastModified { get { throw null; } }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static Microsoft.Net.Http.Headers.RangeConditionHeaderValue Parse(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(Microsoft.Extensions.Primitives.StringSegment input, out Microsoft.Net.Http.Headers.RangeConditionHeaderValue parsedValue) { throw null; }
    }
    public partial class RangeHeaderValue
    {
        public RangeHeaderValue() { }
        public RangeHeaderValue(long? from, long? to) { }
        public System.Collections.Generic.ICollection<Microsoft.Net.Http.Headers.RangeItemHeaderValue> Ranges { get { throw null; } }
        public Microsoft.Extensions.Primitives.StringSegment Unit { get { throw null; } set { } }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static Microsoft.Net.Http.Headers.RangeHeaderValue Parse(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(Microsoft.Extensions.Primitives.StringSegment input, out Microsoft.Net.Http.Headers.RangeHeaderValue parsedValue) { throw null; }
    }
    public partial class RangeItemHeaderValue
    {
        public RangeItemHeaderValue(long? from, long? to) { }
        public long? From { get { throw null; } }
        public long? To { get { throw null; } }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public override string ToString() { throw null; }
    }
    public enum SameSiteMode
    {
        Lax = 1,
        None = 0,
        Strict = 2,
    }
    public partial class SetCookieHeaderValue
    {
        public SetCookieHeaderValue(Microsoft.Extensions.Primitives.StringSegment name) { }
        public SetCookieHeaderValue(Microsoft.Extensions.Primitives.StringSegment name, Microsoft.Extensions.Primitives.StringSegment value) { }
        public Microsoft.Extensions.Primitives.StringSegment Domain { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute]set { } }
        public System.DateTimeOffset? Expires { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute]set { } }
        public bool HttpOnly { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute]set { } }
        public System.TimeSpan? MaxAge { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute]set { } }
        public Microsoft.Extensions.Primitives.StringSegment Name { get { throw null; } set { } }
        public Microsoft.Extensions.Primitives.StringSegment Path { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute]set { } }
        public Microsoft.Net.Http.Headers.SameSiteMode SameSite { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute]set { } }
        public bool Secure { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } [System.Runtime.CompilerServices.CompilerGeneratedAttribute]set { } }
        public Microsoft.Extensions.Primitives.StringSegment Value { get { throw null; } set { } }
        public void AppendToStringBuilder(System.Text.StringBuilder builder) { }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static Microsoft.Net.Http.Headers.SetCookieHeaderValue Parse(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public static System.Collections.Generic.IList<Microsoft.Net.Http.Headers.SetCookieHeaderValue> ParseList(System.Collections.Generic.IList<string> inputs) { throw null; }
        public static System.Collections.Generic.IList<Microsoft.Net.Http.Headers.SetCookieHeaderValue> ParseStrictList(System.Collections.Generic.IList<string> inputs) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(Microsoft.Extensions.Primitives.StringSegment input, out Microsoft.Net.Http.Headers.SetCookieHeaderValue parsedValue) { throw null; }
        public static bool TryParseList(System.Collections.Generic.IList<string> inputs, out System.Collections.Generic.IList<Microsoft.Net.Http.Headers.SetCookieHeaderValue> parsedValues) { throw null; }
        public static bool TryParseStrictList(System.Collections.Generic.IList<string> inputs, out System.Collections.Generic.IList<Microsoft.Net.Http.Headers.SetCookieHeaderValue> parsedValues) { throw null; }
    }
    public partial class StringWithQualityHeaderValue
    {
        public StringWithQualityHeaderValue(Microsoft.Extensions.Primitives.StringSegment value) { }
        public StringWithQualityHeaderValue(Microsoft.Extensions.Primitives.StringSegment value, double quality) { }
        public double? Quality { get { throw null; } }
        public Microsoft.Extensions.Primitives.StringSegment Value { get { throw null; } }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static Microsoft.Net.Http.Headers.StringWithQualityHeaderValue Parse(Microsoft.Extensions.Primitives.StringSegment input) { throw null; }
        public static System.Collections.Generic.IList<Microsoft.Net.Http.Headers.StringWithQualityHeaderValue> ParseList(System.Collections.Generic.IList<string> input) { throw null; }
        public static System.Collections.Generic.IList<Microsoft.Net.Http.Headers.StringWithQualityHeaderValue> ParseStrictList(System.Collections.Generic.IList<string> input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(Microsoft.Extensions.Primitives.StringSegment input, out Microsoft.Net.Http.Headers.StringWithQualityHeaderValue parsedValue) { throw null; }
        public static bool TryParseList(System.Collections.Generic.IList<string> input, out System.Collections.Generic.IList<Microsoft.Net.Http.Headers.StringWithQualityHeaderValue> parsedValues) { throw null; }
        public static bool TryParseStrictList(System.Collections.Generic.IList<string> input, out System.Collections.Generic.IList<Microsoft.Net.Http.Headers.StringWithQualityHeaderValue> parsedValues) { throw null; }
    }
    public partial class StringWithQualityHeaderValueComparer : System.Collections.Generic.IComparer<Microsoft.Net.Http.Headers.StringWithQualityHeaderValue>
    {
        internal StringWithQualityHeaderValueComparer() { }
        public static Microsoft.Net.Http.Headers.StringWithQualityHeaderValueComparer QualityComparer { get { throw null; } }
        public int Compare(Microsoft.Net.Http.Headers.StringWithQualityHeaderValue stringWithQuality1, Microsoft.Net.Http.Headers.StringWithQualityHeaderValue stringWithQuality2) { throw null; }
    }
}
