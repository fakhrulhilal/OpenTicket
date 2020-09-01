using OpenTicket.Domain.MailClient;
using System;

namespace OpenTicket.MailAdapter
{
    public class Pop3ClientFactory : IMailClientFactory
    {
        public MailProtocolType SupportedProtocol { get; } = MailProtocolType.Pop3;
        public IMailClient Build(EmailAccount account)
        {
            throw new NotImplementedException();
        }
    }
}
