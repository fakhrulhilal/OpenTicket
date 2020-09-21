using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Security;
using OpenTicket.Helper;
using OpenTicket.Domain.MailClient;

namespace OpenTicket.MailAdapter
{
    public class Microsoft365Client : ImapClient
    {
        private readonly Data.Entity.OpenTicketDbContext _db;

        public Microsoft365Client(EmailAccount account, Data.Entity.OpenTicketDbContext db) : base(account)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public override async Task AuthenticateAsync(CancellationToken cancellationToken)
        {
            var emailAccount = _db.EmailAccounts.FirstOrDefault(ea => ea.Email == Account.Email);
            if (emailAccount == null)
                throw new InvalidOperationException($"No email account with address: {Account.Email}");
            
            if (string.IsNullOrEmpty(emailAccount.AccessToken) || string.IsNullOrEmpty(emailAccount.RefreshToken))
                throw new InvalidOperationException($"No token granted for email account: {emailAccount.Email}");

            var refreshTokenRequest = new RefreshTokenRequest
            {
                ClientId = emailAccount.Username,
                Secret = emailAccount.Password,
                Tenant = emailAccount.Email.Split('@')[1],
                RefreshToken = emailAccount.RefreshToken
            };
            refreshTokenRequest.Scopes.AddRange(M365Helper.EmailScopes);
            var token = await OAuth2Helper.RefreshTokenAsync(refreshTokenRequest);
            emailAccount.LastUpdateAccessToken = DateTime.UtcNow;
            _db.EmailAccounts.Update(emailAccount);
            await _db.SaveChangesAsync(cancellationToken);
            var auth = new SaslMechanismOAuth2(emailAccount.Email, token.AccessToken);
            await Client.AuthenticateAsync(auth, cancellationToken);
        }
    }
}
