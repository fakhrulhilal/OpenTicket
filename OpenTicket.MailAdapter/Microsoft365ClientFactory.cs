using OpenTicket.Domain.MailClient;

namespace OpenTicket.MailAdapter
{
    public class Microsoft365ClientFactory : IMailClientFactory
    {
        public MailProtocolType SupportedProtocol { get; } = MailProtocolType.M365;
        public IMailClient Build(EmailAccount account) => new Microsoft365Client(account);
    }
}
