using System.Collections.Generic;
using System.Linq;
using MimeKit;
using OpenTicket.Domain.MailClient;

namespace OpenTicket.MailAdapter
{
    public class MailMessageAdapter : IMailMessage
    {
        private readonly MimeMessage _inner;

        protected MailMessageAdapter(MimeMessage inner) => _inner = inner;

        public string Subject => _inner.Subject;
        public string Body => _inner.HtmlBody ?? _inner.TextBody;
        public IMailAddress From => new MailAddressAdapter(_inner.From.Cast<MailboxAddress>().FirstOrDefault());
        public IEnumerable<IMailAddress> To =>
            _inner.To.Cast<MailboxAddress>().Select(address => new MailAddressAdapter(address));
    }
}
