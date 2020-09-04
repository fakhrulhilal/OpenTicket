using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using MailKit;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using OpenTicket.Domain.MailClient;

namespace OpenTicket.MailAdapter
{
    public class ImapClient : IMailClient
    {
        protected MailKit.Net.Imap.ImapClient Client;
        protected readonly EmailAccount Account;
        protected IMailFolder Mailbox;

        public ImapClient(EmailAccount account) =>
            Account = account ?? throw new ArgumentNullException(nameof(account));

        public void Dispose()
        {
            Mailbox.Expunge();
            Client.Disconnect(true);
            Client.Dispose();
        }

        public async Task InitializeConnectionAsync(CancellationToken cancellationToken)
        {
            Client = new MailKit.Net.Imap.ImapClient();
            if (Account.UseSecureConnection)
            {
                Client.SslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls12;
                Client.ServerCertificateValidationCallback = (sender, certificate, chain, errorPolicies) =>
                    errorPolicies != SslPolicyErrors.RemoteCertificateNotAvailable;
                await Client.ConnectAsync(Account.ServerAddress, Account.ServerPort, true, cancellationToken);
            }
            else
            {
                Client.SslProtocols = SslProtocols.None;
                await Client.ConnectAsync(Account.ServerAddress, Account.ServerPort, SecureSocketOptions.None,
                    cancellationToken);
            }
        }

        public virtual async Task AuthenticateAsync(CancellationToken cancellationToken)
        {
            await Client.AuthenticateAsync(Account.UserId, Account.Secret, cancellationToken);
            Mailbox = await Client.GetFolderAsync(Account.MailBox ?? "INBOX", cancellationToken);
            await Mailbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken);
        }

        public IEnumerable<IMailMessage> FetchNewMessages()
        {
            var ids = Mailbox.Search(SearchQuery.All);
            foreach (var id in ids)
            {
                var message = Mailbox.GetMessage(id);
                yield return new ImapMessage(id, message);
            }
        }

        public async Task DeleteAsync(IMailMessage message, CancellationToken cancellationToken)
        {
            if (!(message is ImapMessage imapMessage))
                throw new InvalidOperationException("Only support deleting message fetched through this client");
            await Mailbox.AddFlagsAsync(imapMessage.Id, MessageFlags.Deleted, true, cancellationToken);
        }

        private class ImapMessage : MailMessageAdapter
        {
            internal UniqueId Id { get; }
            public ImapMessage(UniqueId id, MimeMessage inner) : base(inner) => Id = id;
        }
    }
}
