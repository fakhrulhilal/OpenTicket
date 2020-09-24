using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class GetTenantByIdentifierQuery : IRequest<GetTenantByIdentifierQuery.Result>
    {
        private readonly string _identifier;

        public GetTenantByIdentifierQuery(string identifier) => _identifier = identifier;

        public class Result
        {
            public string Identifier { get; set; }
            public string ClientId { get; set; }
            public string Secret { get; set; }
        }

        public class Handler : IRequestHandler<GetTenantByIdentifierQuery, Result>
        {
            private readonly OpenTicketDbContext _db;
            private readonly IMapper _mapper;

            public Handler(OpenTicketDbContext db, IMapper mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }
        
            public async Task<Result> Handle(GetTenantByIdentifierQuery request, CancellationToken cancellationToken)
            {
                var emailAccount = await _db.ExternalAccounts.AsNoTracking()
                    .FirstOrDefaultAsync(ea => ea.Identifier == request._identifier, cancellationToken);
                return _mapper.Map<Result>(emailAccount);
            }
        }
    }
}