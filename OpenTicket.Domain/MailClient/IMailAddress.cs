namespace OpenTicket.Domain.MailClient
{
    public interface IMailAddress
    {
        string DisplayName { get; }
        string Address { get; }
    }
}
