using MediatR;
using System;
using System.Collections.Generic;

namespace OpenTicket.Domain.Command
{
    public class QueryEmailAccounts : PageableQuery, IRequest<IEnumerable<QueryEmailAccounts.EmailAccount>>
    {
        public class EmailAccount : MailClient.EmailAccount
        {
            public int Id { get; set; }
            public DateTime? LastUpdateAccessToken { get; set; }
        }
    }
}
