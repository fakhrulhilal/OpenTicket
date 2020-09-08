using System;

namespace OpenTicket.Web.Models
{
    internal static class M365Helper
    {
        private static string BuildBaseUri(string tenant) => $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0";
        public static Uri BuildAuthorizeUri(string tenant) => new Uri($"{BuildBaseUri(tenant)}/authorize");
        public static Uri BuildTokenUri(string tenant) => new Uri($"{BuildBaseUri(tenant)}/token");
    }
}