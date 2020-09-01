using System.Collections.Generic;

namespace OpenTicket.Domain.MailClient
{
    public interface IMailMessage
    {
        string Subject { get; }
        string HtmlBody { get; }
        IMailAddress From { get; }
        IEnumerable<IMailAddress> To { get; }
    }
}