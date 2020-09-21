using System;
using System.Text.Json.Serialization;

namespace OpenTicket.Helper
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        public string Scope { get; set; }
        
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }

        public string Error { get; set; }
        
        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }
        
        [JsonPropertyName("error_codes")]
        public int[] ErrorCodes { get; set; }
        
        public DateTime TimeStamp { get; set; }
        
        [JsonPropertyName("trace_id")]
        public Guid TraceId { get; set; }
        
        [JsonPropertyName("correlation_id")]
        public Guid CorrelationId { get; set; }
    }
}