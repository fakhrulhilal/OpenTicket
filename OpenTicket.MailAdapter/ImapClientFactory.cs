using OpenTicket.Domain.MailClient;
using System;

namespace OpenTicket.MailAdapter
{
    public class ImapClientFactory : IMailClientFactory
    {
        public MailProtocolType SupportedProtocol { get; } = MailProtocolType.Imap;
        public IMailClient Build(EmailAccount account)
        {
            throw new NotImplementedException();
        }
    }
}
