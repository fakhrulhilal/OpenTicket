using OpenTicket.Domain.MailClient;

namespace OpenTicket.MailAdapter
{
    public class Pop3ClientFactory : IMailClientFactory
    {
        public MailProtocolType SupportedProtocol { get; } = MailProtocolType.Pop3;
        public IMailClient Build(EmailAccount account) => new Pop3Client(account);
    }
}
