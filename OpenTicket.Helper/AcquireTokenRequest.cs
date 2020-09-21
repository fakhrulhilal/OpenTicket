namespace OpenTicket.Helper
{
    public class AcquireTokenRequest
    {
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string ResponseCode { get; set; }
        public string RedirectUri { get; set; }
        public string CodeVerifier { get; set; }
    }
}