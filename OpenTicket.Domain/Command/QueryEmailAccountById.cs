using MediatR;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class QueryEmailAccountById : IRequest<QueryEmailAccountById.EmailAccount>
    {
        public QueryEmailAccountById(int id) => Id = id;

        public int Id { get; }

        public class EmailAccount
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string UserId { get; set; }
            public string Secret { get; set; }
            public int ServerPort { get; set; }
            public string ServerAddress { get; set; }
            public string MailBox { get; set; }
            public bool UseSecureConnection { get; set; }
            public MailProtocolType Protocol { get; set; }
        }
    }
}
