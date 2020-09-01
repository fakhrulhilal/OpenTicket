namespace OpenTicket.Domain.MailClient
{
    public interface IMailClientFactory
    {
        MailProtocolType SupportedProtocol { get; }
        IMailClient Build(EmailAccount account);
    }
}
