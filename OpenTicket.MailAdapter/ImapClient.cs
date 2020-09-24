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
        private IMailFolder _mailbox;
        private bool _disposed;

        public ImapClient(EmailAccount account) =>
            Account = account ?? throw new ArgumentNullException(nameof(account));

        public void Dispose()
        {
            if (_disposed) return;
            _mailbox?.Expunge();
            if (Client != null && Client.IsConnected)
            {
                Client.Disconnect(true);
                Client.Dispose();
            }
            _disposed = true;
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
            await Client.AuthenticateAsync(Account.Username, Account.Password, cancellationToken);
        }

        public IEnumerable<IMailMessage> FetchNewMessages()
        {
            if (_mailbox == null)
            {
                _mailbox = Client.GetFolder(Account.MailBox ?? "INBOX");
                _mailbox.Open(FolderAccess.ReadWrite);
            }
            var ids = _mailbox.Search(SearchQuery.All);
            foreach (var id in ids)
            {
                var message = _mailbox.GetMessage(id);
                yield return new ImapMessage(id, message);
            }
        }

        public async Task DeleteAsync(IMailMessage message, CancellationToken cancellationToken)
        {
            if (!(message is ImapMessage imapMessage))
                throw new InvalidOperationException("Only support deleting message fetched through this client");
            await _mailbox.AddFlagsAsync(imapMessage.Id, MessageFlags.Deleted, true, cancellationToken);
        }

        private class ImapMessage : MailMessageAdapter
        {
            internal UniqueId Id { get; }
            public ImapMessage(UniqueId id, MimeMessage inner) : base(inner) => Id = id;
        }
    }
}
