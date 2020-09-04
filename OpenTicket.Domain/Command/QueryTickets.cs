using System;
using System.Collections.Generic;
using MediatR;

namespace OpenTicket.Domain.Command
{
    public class QueryTickets : PageableQuery, IRequest<IEnumerable<QueryTickets.Ticket>>
    {
        public class Ticket
        {
            public DateTime CreatedDateTime { get; set; }
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string Title { get; set; }
        }
    }
}
