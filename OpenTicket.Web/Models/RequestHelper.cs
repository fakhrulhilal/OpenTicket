using System.Web;

namespace OpenTicket.Web.Models
{
    internal static class RequestHelper
    {
        internal static string GetBaseUrl(this HttpRequestBase request) =>
            $"{request.Url.Scheme}://{request.Url.Host}:{request.Url.Port}";
    }
}