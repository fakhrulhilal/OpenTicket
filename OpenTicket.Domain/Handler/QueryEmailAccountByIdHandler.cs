using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OpenTicket.Data.Entity;
using OpenTicket.Domain.Command;

namespace OpenTicket.Domain.Handler
{
    public class QueryEmailAccountByIdHandler : IRequestHandler<QueryEmailAccountById, QueryEmailAccountById.EmailAccount>
    {
        private readonly OpenTicketDbContext _db;
        private readonly IMapper _mapper;

        public QueryEmailAccountByIdHandler(OpenTicketDbContext db, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<QueryEmailAccountById.EmailAccount> Handle(QueryEmailAccountById request, CancellationToken cancellationToken)
        {
            var emailAccount = await _db.EmailAccounts.FindAsync(request.Id);
            return _mapper.Map<QueryEmailAccountById.EmailAccount>(emailAccount);
        }
    }
}
