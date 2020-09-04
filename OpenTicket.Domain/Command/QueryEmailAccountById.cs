using MediatR;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class QueryEmailAccountById : IRequest<QueryEmailAccountById.EmailAccount>
    {
        public QueryEmailAccountById(int id) => Id = id;

        public int Id { get; }

        public class EmailAccount : MailClient.EmailAccount
        {
            public int Id { get; set; }
        }
    }
}
