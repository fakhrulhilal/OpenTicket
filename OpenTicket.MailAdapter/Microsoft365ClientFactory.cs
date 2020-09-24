using OpenTicket.Domain.MailClient;
using EmailAccount = OpenTicket.Domain.MailClient.EmailAccount;
using MailProtocolType = OpenTicket.Domain.MailClient.MailProtocolType;

namespace OpenTicket.MailAdapter
{
    public class Microsoft365ClientFactory : IMailClientFactory
    {
        public MailProtocolType SupportedProtocol { get; } = MailProtocolType.M365;
        public IMailClient Build(EmailAccount account) => new Microsoft365Client(account);
    }
}
