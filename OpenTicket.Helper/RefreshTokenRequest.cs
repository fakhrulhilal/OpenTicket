using System.Collections.Generic;

namespace OpenTicket.Helper
{
    public class RefreshTokenRequest
    {
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string RefreshToken { get; set; }
        public string Secret { get; set; }
        public List<string> Scopes { get; } = new List<string>();
    }
}