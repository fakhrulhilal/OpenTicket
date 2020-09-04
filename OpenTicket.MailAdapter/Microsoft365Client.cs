using System.Threading;
using System.Threading.Tasks;
using MailKit;
using MailKit.Security;
using OpenTicket.Domain.MailClient;

namespace OpenTicket.MailAdapter
{
    public class Microsoft365Client : ImapClient
    {
        public Microsoft365Client(EmailAccount account) : base(account)
        { }

        public override async Task AuthenticateAsync(CancellationToken cancellationToken)
        {
            var auth = new SaslMechanismOAuth2(Account.UserId, Account.AccessToken);
            await Client.AuthenticateAsync(auth, cancellationToken);
            Mailbox = await Client.GetFolderAsync(Account.MailBox ?? "INBOX", cancellationToken);
            await Mailbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken);
        }
    }
}
