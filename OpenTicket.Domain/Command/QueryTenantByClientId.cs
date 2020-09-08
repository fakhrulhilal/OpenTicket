using MediatR;

namespace OpenTicket.Domain.Command
{
    public class QueryTenantByClientId : IRequest<QueryTenantByClientId.Tenant>
    {
        public class Tenant
        {
            public string ClientId { get; set; }
            public string Secret { get; set; }
            public string TenantId { get; set; }
        }

        public QueryTenantByClientId(string clientId)
        {
            ClientId = clientId;
        }

        public string ClientId { get; }
    }
}