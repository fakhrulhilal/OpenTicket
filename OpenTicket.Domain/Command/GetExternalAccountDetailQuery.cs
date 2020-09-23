using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class GetExternalAccountDetailQuery : IRequest<GetExternalAccountDetailQuery.Result>
    {
        public GetExternalAccountDetailQuery(int id) => _id = id;

        private readonly int _id;

        public class Handler : IRequestHandler<GetExternalAccountDetailQuery, Result>
        {
            private readonly OpenTicketDbContext _db;
            private readonly IMapper _mapper;

            public Handler(OpenTicketDbContext db, IMapper mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<Result> Handle(GetExternalAccountDetailQuery request,
                CancellationToken cancellationToken)
            {
                var externalAccount = await _db.ExternalAccounts.AsNoTracking()
                    .FirstOrDefaultAsync(ea => ea.Id == request._id, cancellationToken);
                return _mapper.Map<Result>(externalAccount);
            }
        }

        public class Result : EditExternalAccountCommand { }
    }
}