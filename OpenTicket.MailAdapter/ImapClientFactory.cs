using OpenTicket.Domain.MailClient;

namespace OpenTicket.MailAdapter
{
    public class ImapClientFactory : IMailClientFactory
    {
        public MailProtocolType SupportedProtocol { get; } = MailProtocolType.Imap;

        public IMailClient Build(EmailAccount account) => new ImapClient(account);
    }
}