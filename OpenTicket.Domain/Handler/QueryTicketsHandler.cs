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
    public class QueryTicketsHandler : IRequestHandler<QueryTickets, IEnumerable<QueryTickets.Ticket>>
    {
        private readonly OpenTicketDbContext _db;
        private readonly IMapper _mapper;

        public QueryTicketsHandler(OpenTicketDbContext db, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }
        
        public async Task<IEnumerable<QueryTickets.Ticket>> Handle(QueryTickets request, CancellationToken cancellationToken) =>
            await _db.Tickets.AsNoTracking()
                .Include(t => t.Customer)
                .OrderByDescending(t => t.Id)
                .Select(t => _mapper.Map<QueryTickets.Ticket>(t))
                .ToArrayAsync(cancellationToken);
    }
}
