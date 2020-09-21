using System;
using OpenTicket.Data.Entity;
using OpenTicket.Domain.MailClient;
using EmailAccount = OpenTicket.Domain.MailClient.EmailAccount;
using MailProtocolType = OpenTicket.Domain.MailClient.MailProtocolType;

namespace OpenTicket.MailAdapter
{
    public class Microsoft365ClientFactory : IMailClientFactory
    {
        private readonly OpenTicketDbContext _db;

        public Microsoft365ClientFactory(OpenTicketDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }
        
        public MailProtocolType SupportedProtocol { get; } = MailProtocolType.M365;
        public IMailClient Build(EmailAccount account) => new Microsoft365Client(account, _db);
    }
}
