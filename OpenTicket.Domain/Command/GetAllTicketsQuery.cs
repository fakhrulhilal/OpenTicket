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
    public class GetAllTicketsQuery : PageableQuery, IRequest<IEnumerable<GetAllTicketsQuery.Result>>
    {
        public class Result
        {
            public DateTime CreatedDateTime { get; set; }
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string Title { get; set; }
        }

        public class Handler : IRequestHandler<GetAllTicketsQuery, IEnumerable<Result>>
        {
            private readonly OpenTicketDbContext _db;
            private readonly IMapper _mapper;

            public Handler(OpenTicketDbContext db, IMapper mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<IEnumerable<Result>> Handle(GetAllTicketsQuery request,
                CancellationToken cancellationToken) =>
                await _db.Tickets.AsNoTracking()
                    .Include(t => t.Customer)
                    .OrderByDescending(t => t.Id)
                    .Select(t => _mapper.Map<Result>(t))
                    .ToArrayAsync(cancellationToken);
        }
    }
}