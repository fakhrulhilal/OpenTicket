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
    public class GetAllExternalAccountQuery : IRequest<IEnumerable<GetAllExternalAccountQuery.Result>>
    {
        public MailProtocolType? Protocol { get; set; }

        public class Handler : IRequestHandler<GetAllExternalAccountQuery, IEnumerable<Result>>
        {
            private readonly OpenTicketDbContext _db;
            private readonly IMapper _mapper;

            public Handler(OpenTicketDbContext db, IMapper mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public Task<IEnumerable<Result>> Handle(GetAllExternalAccountQuery request,
                CancellationToken cancellationToken)
            {
                var externalAccounts = _db.ExternalAccounts.AsNoTracking();
                if (request.Protocol.HasValue)
                    externalAccounts = externalAccounts.Where(ea => ea.Protocol == request.Protocol.Value);
                var result = externalAccounts.Select(entity => _mapper.Map<Result>(entity)).ToArray();
                return Task.FromResult(result.AsEnumerable());
            }
        }

        public class Result
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public MailProtocolType Protocol { get; set; }
            public string Identifier { get; set; }
        }
    }
}