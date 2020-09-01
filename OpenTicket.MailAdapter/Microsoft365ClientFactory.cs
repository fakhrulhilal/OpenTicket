using OpenTicket.Domain.MailClient;
using System;

namespace OpenTicket.MailAdapter
{
    public class Microsoft365ClientFactory : IMailClientFactory
    {
        public MailProtocolType SupportedProtocol { get; } = MailProtocolType.M365;
        public IMailClient Build(EmailAccount account)
        {
            throw new NotImplementedException();
        }
    }
}
