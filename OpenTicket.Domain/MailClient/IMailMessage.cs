using System.Collections.Generic;

namespace OpenTicket.Domain.MailClient
{
    public interface IMailMessage
    {
        string Subject { get; }
        string Body { get; }
        IMailAddress From { get; }
        IEnumerable<IMailAddress> To { get; }
    }
}