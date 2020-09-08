using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenTicket.Web.Models
{
    public class OAuth2AuthRequest
    {
        public static OAuth2AuthRequest Create(string tenant, string clientId, Uri redirect, params string[] scopes)
        {
            string codeVerifier = Guid.NewGuid().ToString().Replace('-', '\0');
            var request = new OAuth2AuthRequest
            {
                AuthUri = M365Helper.BuildAuthorizeUri(tenant),
                ClientId = clientId,
                Redirect = redirect,
                CodeChallenge = EncryptionHelper.CreateChallengeCode(codeVerifier),
                State = EncryptionHelper.Encrypt($"{tenant}#{clientId}#{codeVerifier}")
            };
            if (scopes?.Length > 0)
                request.Scopes.AddRange(scopes);
            var uniqueScopes = request.Scopes.Distinct().ToArray();
            request.Scopes.Clear();
            request.Scopes.AddRange(uniqueScopes);

            return request;
        }

        public static (string Tenant, string ClientId, string Verifier) ParseState(string state)
        {
            var plainState = EncryptionHelper.Decrypt(state);
            var parsedState = plainState.Split('#');
            if (parsedState.Length != 3) return (null, null, null);
            return (parsedState[0], parsedState[1], parsedState[2]);
        }

        public Uri AuthUri { get; private set; }
        public string ClientId { get; private set; }
        public string ResponseType { get; } = "code";
        public Uri Redirect { get; private set; }
        public List<string> Scopes { get; } = new List<string> {"openid"};
        public string ResponseMode { get; } = "form_post";
        public string State { get; private set; }
        public string Prompt { get; } = "consent";
        public string CodeChallenge { get; private set; }
        public string CodeChallengeMethod { get; } = "S256";
    }
}