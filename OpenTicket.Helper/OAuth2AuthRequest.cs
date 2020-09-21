using System.Collections.Generic;

namespace OpenTicket.Helper
{
    public class OAuth2AuthRequest
    {
        public string AuthUri { get; internal set; }
        public string ClientId { get; internal set; }
        public string ResponseType { get; } = "code";
        public string Redirect { get; internal set; }
        public string Scopes { get; set; }
        public string ResponseMode { get; } = "form_post";
        public string State { get; internal set; }
        public string Prompt { get; } = "consent";
        public string CodeChallenge { get; internal set; }
        public string CodeChallengeMethod { get; } = "S256";
    }
}