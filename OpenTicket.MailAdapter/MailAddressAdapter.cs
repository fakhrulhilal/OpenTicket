using MimeKit;
using OpenTicket.Domain.MailClient;

namespace OpenTicket.MailAdapter
{
    public class MailAddressAdapter : IMailAddress
    {
        private readonly MailboxAddress _inner;

        public MailAddressAdapter(MailboxAddress inner) => _inner = inner;
        public string DisplayName => _inner?.Name;
        public string Address => _inner.Address;
    }
}
