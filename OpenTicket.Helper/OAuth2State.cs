namespace OpenTicket.Helper
{
    public class OAuth2State
    {
        public string Tenant { get; internal set; }
        public string Email { get; internal set; }
        public string CodeVerifier { get; internal set; }
        public string ClientId { get; internal set; }
    }
}