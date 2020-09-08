using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;
using OpenTicket.Domain.Command;

namespace OpenTicket.Domain.Handler
{
    public class QueryTenantByClientIdHandler : IRequestHandler<QueryTenantByClientId, QueryTenantByClientId.Tenant>
    {
        private readonly OpenTicketDbContext _db;
        private readonly IMapper _mapper;

        public QueryTenantByClientIdHandler(OpenTicketDbContext db, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }
        
        public async Task<QueryTenantByClientId.Tenant> Handle(QueryTenantByClientId request, CancellationToken cancellationToken)
        {
            var emailAccount = await _db.EmailAccounts.AsNoTracking()
                .FirstOrDefaultAsync(ea => ea.Username == request.ClientId, cancellationToken);
            return _mapper.Map<QueryTenantByClientId.Tenant>(emailAccount);
        }
    }
}