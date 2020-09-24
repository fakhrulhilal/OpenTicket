using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class QueryEmailAccounts : PageableQuery, IRequest<IEnumerable<QueryEmailAccounts.Result>>
    {
        public bool? IsActive { get; set; }

        public class Result : MailClient.EmailAccount
        {
            public int Id { get; set; }
            public string Tenant { get; set; }
            public string ClientId { get; set; }
            public string Secret { get; set; }
            public DateTime? LastUpdateAccessToken { get; set; }
        }

        public class Handler : IRequestHandler<QueryEmailAccounts, IEnumerable<Result>>
        {
            private readonly OpenTicketDbContext _db;
            private readonly IMapper _mapper;

            public Handler(OpenTicketDbContext db, IMapper mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public Task<IEnumerable<Result>> Handle(QueryEmailAccounts request,
                CancellationToken cancellationToken)
            {
                var offset = (request.CurrentPage - 1) * request.PageSize;
                if (offset < 0) offset = 0;
                var emails = _db.EmailAccounts.AsNoTracking().Where(e => !e.DraftId.HasValue);
                if (request.IsActive.HasValue)
                    emails = emails.Where(e => e.IsActive == request.IsActive.Value);
                var result = emails
                    .Include(e => e.ExternalAccount)
                    .Skip(offset).Take(request.PageSize)
                    .Select(ea => _mapper.Map<Result>(ea)).ToArray();
                return Task.FromResult(result.AsEnumerable());
            }
        }
    }
}