using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenTicket.Helper
{
    public static class OAuth2Helper
    {
        public static OAuth2AuthRequest CreateRequest(string email, string tenant, string clientId, string redirect, string[] scopes)
        {
            string codeVerifier = Guid.NewGuid().ToString().Replace('-', '\0');
            var request = new OAuth2AuthRequest
            {
                AuthUri = M365Helper.BuildAuthorizeUri(tenant),
                ClientId = clientId,
                Redirect = redirect,
                CodeChallenge = EncryptionHelper.CreateChallengeCode(codeVerifier),
                State = EncryptionHelper.Encrypt($"{tenant}#{clientId}#{codeVerifier}#{email}"),
                Scopes = string.Join(" ", scopes)
            };

            return request;
        }

        public static OAuth2State ParseState(string state)
        {
            var plainState = EncryptionHelper.Decrypt(state);
            var parsedState = plainState.Split('#');
            if (parsedState.Length != 4) return null;
            return new OAuth2State
            {
                Tenant = parsedState[0],
                ClientId = parsedState[1],
                CodeVerifier = parsedState[2],
                Email = parsedState[3]
            };
        }

        public static async Task<TokenResponse> AcquireTokenAsync(AcquireTokenRequest request)
        {
            using (var httpClient = new HttpClient())
            {
                var postBody = new Dictionary<string, string>
                {
                    ["client_id"] = request.ClientId,
                    ["grant_type"] = "authorization_code",
                    ["scope"] = string.Join(" ", M365Helper.EmailScopes),
                    ["code"] = request.ResponseCode,
                    ["redirect_uri"] = request.RedirectUri,
                    ["client_secret"] = request.Secret,
                    ["code_verifier"] = request.CodeVerifier
                };
                var httpRequest =
                    new HttpRequestMessage(HttpMethod.Post, M365Helper.BuildTokenUri(request.Tenant))
                    {
                        Content = new FormUrlEncodedContent(postBody)
                    };
                var httpResponse = await httpClient.SendAsync(httpRequest);
                string responseString = await httpResponse.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseString);
                return tokenResponse;
            }
        }

        public static async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            using (var httpClient = new HttpClient())
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, M365Helper.BuildTokenUri(request.Tenant))
                {
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["client_id"] = request.ClientId,
                        ["scope"] = string.Join(" ", request.Scopes),
                        ["refresh_token"] = request.RefreshToken,
                        ["grant_type"] = "refresh_token",
                        ["client_secret"] = request.Secret
                    })
                };
                var response = await httpClient.SendAsync(httpRequest);
                var content = await response.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<TokenResponse>(content);
                return token;
            }
        }
    }
}

    