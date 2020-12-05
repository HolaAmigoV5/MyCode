namespace Microsoft.AspNetCore.Antiforgery
{
    internal interface IAntiforgeryFeature
    {
        AntiforgeryToken CookieToken { get; set; }

        bool HaveDeserializedCookieToken { get; set; }

        bool HaveDeserializedRequestToken { get; set; }

        bool HaveGeneratedNewCookieToken { get; set; }

        bool HaveStoredNewCookieToken { get; set; }

        AntiforgeryToken NewCookieToken { get; set; }

        string NewCookieTokenString { get; set; }

        AntiforgeryToken NewRequestToken { get; set; }

        string NewRequestTokenString { get; set; }

        AntiforgeryToken RequestToken { get; set; }
    }
}
