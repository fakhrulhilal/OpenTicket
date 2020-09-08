using System;
using Newtonsoft.Json;

namespace OpenTicket.Web.Models
{
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        public string Scope { get; set; }
        
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        
        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        public string Error { get; set; }
        
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
        
        [JsonProperty("error_codes")]
        public int[] ErrorCodes { get; set; }
        
        public DateTime TimeStamp { get; set; }
        
        [JsonProperty("trace_id")]
        public Guid TraceId { get; set; }
        
        [JsonProperty("correlation_id")]
        public Guid CorrelationId { get; set; }
    }
}