namespace OpenTicket.Helper
{
    public static class M365Helper
    {
        public static string[] EmailScopes =>
            new[] { "openid", "email", "offline_access", "https://outlook.office.com/IMAP.AccessAsUser.All" };

        private static string BuildBaseUri(string tenant) => $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0";
        public static string BuildAuthorizeUri(string tenant) => $"{BuildBaseUri(tenant)}/authorize";
        public static string BuildTokenUri(string tenant) => $"{BuildBaseUri(tenant)}/token";
    }
}