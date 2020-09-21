using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Security;
using MimeKit;
using OpenTicket.Domain.MailClient;

namespace OpenTicket.MailAdapter
{
    public class Pop3Client : IMailClient
    {
        private MailKit.Net.Pop3.Pop3Client _client;
        private readonly EmailAccount _account;
        private bool _disposed;

        public Pop3Client(EmailAccount account) =>
            _account = account ?? throw new ArgumentNullException(nameof(account));

        public void Dispose()
        {
            if (_disposed || _client == null || !_client.IsConnected) return;
            _client.Disconnect(true);
            _client.Dispose();
            _disposed = true;
        }

        public async Task InitializeConnectionAsync(CancellationToken cancellationToken)
        {
            _client = new MailKit.Net.Pop3.Pop3Client();
            if (_account.UseSecureConnection)
            {
                _client.SslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls12;
                _client.ServerCertificateValidationCallback = (sender, certificate, chain, errorPolicies) =>
                    errorPolicies != SslPolicyErrors.RemoteCertificateNotAvailable;
                await _client.ConnectAsync(_account.ServerAddress, _account.ServerPort, true, cancellationToken);
            }
            else
            {
                _client.SslProtocols = SslProtocols.None;
                await _client.ConnectAsync(_account.ServerAddress, _account.ServerPort, SecureSocketOptions.None,
                    cancellationToken);
            }
        }

        public async Task AuthenticateAsync(CancellationToken cancellationToken)
        {
            await _client.AuthenticateAsync(_account.UserId, _account.Secret, cancellationToken);
        }

        public IEnumerable<IMailMessage> FetchNewMessages()
        {
            int totalNewMessage = _client.GetMessageCount();
            for (int i = 0; i < totalNewMessage; i++)
            {
                var mimeMessage = _client.GetMessage(i);
                yield return new Pop3Message(i, mimeMessage);
            }
        }

        public async Task DeleteAsync(IMailMessage message, CancellationToken cancellationToken)
        {
            if (!(message is Pop3Message pop3Message))
                throw new InvalidOperationException("Only support deleting message fetched from this client");
            await _client.DeleteMessageAsync(pop3Message.Index, cancellationToken);
        }

        private class Pop3Message : MailMessageAdapter
        {
            internal int Index { get; }
            public Pop3Message(int index, MimeMessage inner) : base(inner) => Index = index;
        }
    }
}
