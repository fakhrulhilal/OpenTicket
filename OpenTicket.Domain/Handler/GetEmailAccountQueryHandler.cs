using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;
using OpenTicket.Domain.Command;

namespace OpenTicket.Domain.Handler
{
    public class GetEmailAccountQueryHandler : IRequestHandler<QueryEmailAccounts,
            IEnumerable<QueryEmailAccounts.EmailAccount>>
    {
        private readonly OpenTicketDbContext _db;
        private readonly IMapper _mapper;

        public GetEmailAccountQueryHandler(OpenTicketDbContext db, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public Task<IEnumerable<QueryEmailAccounts.EmailAccount>> Handle(QueryEmailAccounts request,
            CancellationToken cancellationToken)
        {
            var offset = (request.CurrentPage - 1) * request.PageSize;
            var result = _db.EmailAccounts.AsNoTracking()
                .Skip(offset).Take(request.PageSize)
                .Select(ea => _mapper.Map<QueryEmailAccounts.EmailAccount>(ea)).ToArray();
            return Task.FromResult(result.AsEnumerable());
        }
    }
}